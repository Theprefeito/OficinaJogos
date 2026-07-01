using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform cameraTransform;

    [Header("Configurações de Movimento")]
    public float maxSpeed = 8f;
    public float acceleration = 12f;
    public float deceleration = 10f;
    public float skiddingDeceleration = 25f;

    [Tooltip("Velocidade de rotação básica quando o personagem está lento.")]
    public float baseTurnSpeed = 900f;

    [Tooltip("Velocidade de rotação mínima quando o personagem está correndo na velocidade máxima.")]
    public float minTurnSpeed = 300f;

    [Header("Configurações de Pulo e Gravidade")]
    public float jumpForce = 8f;
    public float sideFlipJumpForce = 11f;
    public float sideFlipBackwardForce = 5f;
    public float gravity = 20f;

    private CharacterController controller;
    private Vector3 currentVelocity;
    private float verticalVelocity;

    // Estados
    private bool isSkidding = false;
    private bool isSideFlipping = false;
    private Vector3 sideFlipDirection;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Caso a câmera não seja atribuída no Inspector,
        // pega automaticamente a Main Camera.
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleGravityAndJump();

        Vector3 finalMotion = currentVelocity + Vector3.up * verticalVelocity;
        controller.Move(finalMotion * Time.deltaTime);
    }

    void HandleMovement()
    {
        if (isSideFlipping)
        {
            if (controller.isGrounded)
            {
                isSideFlipping = false;
            }

            return;
        }

        // Entrada do jogador
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        // Direções da câmera
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // Remove a inclinação da câmera
        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        // Movimento relativo à câmera
        Vector3 targetDir = (cameraForward * moveZ + cameraRight * moveX).normalized;

        // Verifica derrapagem
        if (targetDir.magnitude > 0.1f && controller.isGrounded)
        {
            float dotProduct = Vector3.Dot(currentVelocity.normalized, targetDir);

            if (dotProduct < -0.5f && currentVelocity.magnitude > (maxSpeed * 0.6f))
            {
                isSkidding = true;
            }
        }

        if (isSkidding)
        {
            currentVelocity = Vector3.MoveTowards(
                currentVelocity,
                Vector3.zero,
                skiddingDeceleration * Time.deltaTime);

            if (currentVelocity.magnitude < 0.5f)
            {
                isSkidding = false;
            }
        }
        else if (targetDir.magnitude > 0.1f)
        {
            Vector3 targetVelocity = targetDir * maxSpeed;

            currentVelocity = Vector3.MoveTowards(
                currentVelocity,
                targetVelocity,
                acceleration * Time.deltaTime);

            float speedPercent = currentVelocity.magnitude / maxSpeed;
            float currentTurnSpeed = Mathf.Lerp(
                baseTurnSpeed,
                minTurnSpeed,
                speedPercent);

            Quaternion targetRotation = Quaternion.LookRotation(targetDir);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                currentTurnSpeed * Time.deltaTime);
        }
        else
        {
            currentVelocity = Vector3.MoveTowards(
                currentVelocity,
                Vector3.zero,
                deceleration * Time.deltaTime);
        }
    }

    void HandleGravityAndJump()
    {
        if (controller.isGrounded)
        {
            verticalVelocity = -0.5f;

            if (Input.GetButtonDown("Jump"))
            {
                if (isSkidding)
                {
                    isSideFlipping = true;
                    isSkidding = false;

                    sideFlipDirection = -transform.forward;

                    verticalVelocity = sideFlipJumpForce;
                    currentVelocity = sideFlipDirection * sideFlipBackwardForce;

                    transform.rotation = Quaternion.LookRotation(sideFlipDirection);
                }
                else
                {
                    verticalVelocity = jumpForce;
                }
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;

            if (isSideFlipping)
            {
                currentVelocity = Vector3.MoveTowards(
                    currentVelocity,
                    sideFlipDirection * (sideFlipBackwardForce * 0.5f),
                    Time.deltaTime * 2f);
            }
        }
    }

    public bool IsSideFlipping() => isSideFlipping;

    public bool IsSkidding() => isSkidding;
}