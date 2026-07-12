using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform cameraTransform;

    [Header("Configurações de Movimento")]
    public float maxSpeed = 8f;
    public float acceleration = 12f;
    public float deceleration = 10f;
    public float derrapadaDeceleration = 25f;
    private Vector2 directionInput;

    [Tooltip("Velocidade de rotação básica quando o personagem está lento.")]
    public float baseTurnSpeed = 900f;

    [Tooltip("Velocidade de rotação mínima quando o personagem está correndo na velocidade máxima.")]
    public float minTurnSpeed = 300f;

    [Header("Configurações de Pulo e Gravidade")]
    public float jumpForce = 8f;
    public float sideFlipJumpForce = 11f;
    public float sideFlipBackwardForce = 5f;
    public float gravity = 20f;

    [Header("Coyote Time")]
    [Tooltip("Tempo em segundos que o jogador ainda pode pular após sair do chão.")]
    public float coyoteDuration = 0.3f;
    private float coyoteCounter;

    //Componentes
    private CharacterController controller;
    private Vector3 currentVelocity;
    private float verticalVelocity;

    // Estados
    private bool derrapando = false;
    private bool isSideFlipping = false;
    private Vector3 sideFlipDirection;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        BasicMovement();
        Jump();

        Vector3 finalMotion = currentVelocity + Vector3.up * verticalVelocity;
        controller.Move(finalMotion * Time.deltaTime);
    }

    #region Basic Movement
    public void analogicMove(InputAction.CallbackContext context) //Serve pro novo Input System
    {
        directionInput = context.ReadValue<Vector2>();
    }

    void BasicMovement()
    {
        if (isSideFlipping)
        {
            if (controller.isGrounded) //Define se quando o jogador estiver no chão, ele não estará mais realizando o side flip
            {
                isSideFlipping = false;
            }

            return;
        }

        Vector3 direction = new Vector3(directionInput.x, 0f, directionInput.y).normalized; //Pega o input e transforma em movimento 3D
        Vector3 cameraForward = cameraTransform.forward; //Define o movimento Z da camera
        Vector3 cameraRight = cameraTransform.right; //Define o movimento X da 
        

        cameraForward.y = 0; //Trava o eixo Y
        cameraRight.y = 0; //Trava o eixo Y

        cameraForward.Normalize(); //Impede bug da verticalidade da camera, normalizando o vetor
        cameraRight.Normalize(); //Impede bug da verticalidade da camera, normalizando o vetor

        Vector3 targetDir = (cameraForward * direction.z + cameraRight * direction.x).normalized;

        if (targetDir.magnitude > 0.1f && controller.isGrounded)
        {
            float dotProduct = Vector3.Dot(currentVelocity.normalized, targetDir);

            if (dotProduct < -0.5f && currentVelocity.magnitude > (maxSpeed * 0.6f))
            {
                derrapando = true;
            }
        }

        if (derrapando) //Faz o derrapamento do personagem
        {
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, derrapadaDeceleration * Time.deltaTime); // Faz a desaceleração do personagem enquanto ele está derrapando

            if (currentVelocity.magnitude < 0.5f) //Quando a velocidade do personagem for menor que 0.5, ele não estará mais derrapando
            {
                derrapando = false;
            }
        }
        else if (targetDir.magnitude > 0.1f)
        {
            Vector3 targetVelocity = targetDir * maxSpeed;

            currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, acceleration * Time.deltaTime);

            float speedPercent = currentVelocity.magnitude / maxSpeed;
            float currentTurnSpeed = Mathf.Lerp(baseTurnSpeed, minTurnSpeed, speedPercent);

            Quaternion targetRotation = Quaternion.LookRotation(targetDir);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, currentTurnSpeed * Time.deltaTime);
        }
        else
        {
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, deceleration * Time.deltaTime);
        }
    }
    #endregion

    #region Jumping
   

    void Jump()
    {
        // Se o personagem estiver no chão reseta o coyoteCounter e a velocidade vertical para um valor pequeno negativo para manter o personagem no chão
        if (controller.isGrounded)
        {
            verticalVelocity = -0.5f; // Mantém o personagem no chão
            coyoteCounter = coyoteDuration; // Resete
        }
        else //Caso o evento anterior não aconteça
        {
            verticalVelocity -= gravity * Time.deltaTime; // Aplica a gravidade
            coyoteCounter -= Time.deltaTime; // Define a tolerância do coyote time

            if (isSideFlipping) //Caso ele esteja realizando o side flip, aplica a força de recuo
            {
                currentVelocity = Vector3.MoveTowards(
                    currentVelocity,
                    sideFlipDirection * (sideFlipBackwardForce * 0.5f),
                    Time.deltaTime * 2f);
            }
        }

        // Apenas  a execução do Pulo
        if (Input.GetButtonDown("Jump") && coyoteCounter > 0f)
        {
            if (derrapando)
            {
                isSideFlipping = true;
                derrapando = false;

                sideFlipDirection = -transform.forward;

                verticalVelocity = sideFlipJumpForce;
                currentVelocity = sideFlipDirection * sideFlipBackwardForce;

                transform.rotation = Quaternion.LookRotation(sideFlipDirection);
            }
            else
            {
                verticalVelocity = jumpForce;
            }

            coyoteCounter = 0f; // Zera para evitar múltiplos pulos no ar dentro da mesma janela
        }
    }
    #endregion

    public bool IsSideFlipping() => isSideFlipping; // Método público para verificar se o personagem está realizando um side flip

    public bool Derrapando() => derrapando; // Método público para verificar se o personagem está derrapando
}