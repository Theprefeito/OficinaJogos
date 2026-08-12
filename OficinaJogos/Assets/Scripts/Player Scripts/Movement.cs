using UnityEngine;
using UnityEngine.InputSystem;


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
    public bool TaNoGrounded;
    public Transform groundCheck;
    public LayerMask groundLayer;
  
    [Header("JumpBuffer")]
    public float bufferDistance;
    private float bufferCounter;

    [Header("Coyote Time")]
    [Tooltip("Tempo em segundos que o jogador ainda pode pular após sair do chão.")]
    public float coyoteDuration = 0.3f;
    private float coyoteCounter;

    //Componentes
    //private CharacterController controller;
    private Rigidbody rigPlayer;
    private Vector3 currentVelocity;
    private float verticalVelocity;
    private Player_AnimatorController animPlayer;

    // Estados
    private bool derrapando = false;
    private bool isSideFlipping = false;
    private Vector3 sideFlipDirection;

    void Start()
    {
        rigPlayer = GetComponent<Rigidbody>();
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        animPlayer = FindAnyObjectByType<Player_AnimatorController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        BasicMovement();
        Jump();
        TesteDeChao();
        
        Vector3 finalMotion = currentVelocity + Vector3.up * verticalVelocity;
    }


    public bool IsGrounded()
    {
        return Physics.OverlapSphere(groundCheck.position, 0.2f, groundLayer).Length > 0;
        
    }

   public void TesteDeChao()
    {
        if (IsGrounded() == true)
        {
            TaNoGrounded = true;
        }
        else 
        {
            TaNoGrounded = false;
        }
    }
        
    
    #region Basic Movement
    public void analogicMove(InputAction.CallbackContext context) //Serve pro novo Input System
    {
        directionInput = context.ReadValue<Vector2>();
    }

    void BasicMovement()
    { 
       

        Vector3 direction = new Vector3(directionInput.x, 0f, directionInput.y).normalized; //Pega o input e transforma em movimento 3D
        Vector3 cameraForward = cameraTransform.forward; //Define o movimento Z da camera
        Vector3 cameraRight = cameraTransform.right; //Define o movimento X da 
        

        cameraForward.y = 0; //Trava o eixo Y
        cameraRight.y = 0; //Trava o eixo Y

        cameraForward.Normalize(); //Impede bug da verticalidade da camera, normalizando o vetor
        cameraRight.Normalize(); //Impede bug da verticalidade da camera, normalizando o vetor
        
        if(IsGrounded() && currentVelocity.magnitude > 0.1f)
        {
            animPlayer.currentState = Player_AnimatorController.AnimState.Run;
        }
        else if(IsGrounded())
        {
            animPlayer.currentState = Player_AnimatorController.AnimState.Idle;
        }
    }
    #endregion

    #region Jumping

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            bufferCounter = bufferDistance;  //Dispara assim que aperta a tecla
        }
    }

    void Jump()
    {
        if (bufferCounter > 0)
        {
            bufferCounter -= Time.deltaTime;  //Começa a diminuir o valor assim que é disparado em "OnJump", para não pular infinitamente ou sempre antes do chão
        }
        
        
        // Se o personagem estiver no chão
        if (IsGrounded())
        {
            
            coyoteCounter = coyoteDuration;
        }
        else
        {
            // Só mude para animação de pulo se ele realmente estiver se movendo verticalmente de forma expressiva (evita micro-quedas)
            if (verticalVelocity > 0.1f || verticalVelocity < -2.5f)
            {
                animPlayer.currentState = Player_AnimatorController.AnimState.Jump;
            }

           
            coyoteCounter -= Time.deltaTime;

            if (isSideFlipping)
            {
                currentVelocity = Vector3.MoveTowards(currentVelocity, sideFlipDirection * (sideFlipBackwardForce * 0.5f), Time.deltaTime * 2f);
            }
        }

        // Execução do Pulo
        if (bufferCounter > 0f && coyoteCounter > 0f) //Adicionado o jumpBuffer para só pular quando for maior que zero e o disparo ocorrer
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

            coyoteCounter = 0f;
           
        }
    }
    #endregion

    public bool IsSideFlipping() => isSideFlipping; // Método público para verificar se o personagem está realizando um side flip

    public bool Derrapando() => derrapando; // Método público para verificar se o personagem está derrapando
}