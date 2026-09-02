using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDash : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform cameraTransform;

    [Header("Configurações do Dash")]
    [SerializeField] private float dashSpeed = 25f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    private CharacterController controller;

    private Vector2 directionInput;

    private bool isDashing = false;
    private bool canDash = true;

    private Vector3 dashDirection;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        if (controller == null)
        {
            Debug.LogError("PlayerDash: O Player não possui um CharacterController!");
        }

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    // Recebe o movimento do Input System
    public void OnMove(InputAction.CallbackContext context)
    {
        directionInput = context.ReadValue<Vector2>();
    }

    // Recebe o comando de Dash
    public void OnDash(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (!canDash || isDashing)
            return;

        StartDash();
    }

    private void StartDash()
    {
        canDash = false;
        isDashing = true;

        // Verifica se o jogador está apertando alguma direção
        if (directionInput.magnitude > 0.1f)
        {
            Vector3 direction = new Vector3(
                directionInput.x,
                0f,
                directionInput.y
            ).normalized;

            // Direção baseada na câmera
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight = cameraTransform.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;

            cameraForward.Normalize();
            cameraRight.Normalize();

            dashDirection =
                cameraForward * direction.z +
                cameraRight * direction.x;

            dashDirection.Normalize();
        }
        else
        {
            // Se nenhuma direção estiver sendo apertada,
            // o Dash acontece para frente.
            dashDirection = transform.forward;
        }

        // Faz o personagem olhar para a direção do Dash
        transform.rotation = Quaternion.LookRotation(dashDirection);

        StartCoroutine(DashCoroutine());
        print("player usou o dash");
    }

    private IEnumerator DashCoroutine()
    {
        float timer = 0f;

        while (timer < dashDuration)
        {
            controller.Move(
                dashDirection * dashSpeed * Time.deltaTime
            );

            timer += Time.deltaTime;

            yield return null;
        }

        isDashing = false;

        // Começa o cooldown
        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    // Retorna se o jogador está atualmente dando Dash
    public bool IsDashing()
    {
        return isDashing;
    }
}