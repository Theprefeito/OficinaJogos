using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player_Charge : MonoBehaviour
{
    [Header("Configura��es do Charge")]
    public float maxPullDistance = 300f;
    public float maxChargeForce = 50f;
    private AudioSource audioSource;

    [Header("Audio")]
    public AudioClip deathEnemyClip;
    public AudioClip runSound;
    public AudioClip chargeSound;

    [Header("Configura��es da Arrancada")]
    public float arrancadaDeceleration = 30f;
    [Tooltip("Tag dos objetos que podem ser destru�dos durante o Charge")]
    public string destructibleTag = "Destructible"; // Tag configur�vel para os objetos que podem ser destru�dos

    [Header("Ui")]
    public Slider chargeSlider;
    public Image sliderFillImage;

    private float currentChargePercent;
    private float currentChargeForce;

    // Refer�ncias do GameObject
    private Movement movement;
    private CharacterController controller;
    private Player_AnimatorController animPlayer;

    [Header("Controle de Estados")]
    private Vector2 clickStartPosition;
    private bool isHoldingCharge = false;
    public bool isCharging = false;

    public bool dontInCharger;
    
    // F�sica Interna do Charge
    private Vector3 arrancadaDirection;
    private float verticalVelocity = -2f;

    //Outros scripts do player
    private PlayerDash playerdash;
    
    private CinemachineImpulseSource impulseSource;
    void Start()
    {
        movement = GetComponent<Movement>();
        controller = GetComponent<CharacterController>();
        animPlayer = FindAnyObjectByType<Player_AnimatorController>();
        audioSource = GetComponent<AudioSource>();
        
        //Referencia ao script do Dash
        playerdash = GetComponent<PlayerDash>();
        
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void Update()
    {
        HandleChargeInput(); // Bot�es do charge
        FillChangeColor();   // Atualiza a cor do slider

        if (isCharging)
        {
            UpdateArrancadaMovement();
        }

        if (isHoldingCharge)
        {
            dontInCharger = false; //Acoplamento 1
            chargeSlider.value = currentChargeForce;
        }
        else
        {
            dontInCharger = true; //Acoplamento 1
            chargeSlider.value = 0f;
        }
    }

    private void HandleChargeInput()
    {
        var mouse = Mouse.current;
        if (mouse == null || isCharging) return;

        // Quando o bot�o esquerdo do mouse � pressionado, inicia o Charge
        if (mouse.leftButton.wasPressedThisFrame && animPlayer.currentState != Player_AnimatorController.AnimState.Jump)
        {
            audioSource.PlayOneShot(chargeSound); // Toca o som de carregamento
            StartCharge(mouse.position.ReadValue());
        }

        // Seta o valor com base na puxada do mouse
        if (isHoldingCharge && mouse.leftButton.isPressed)
        {
            UpdateCharge(mouse.position.ReadValue());
        }

        // Ao soltar realiza a arrancada
        if (isHoldingCharge && mouse.leftButton.wasReleasedThisFrame)
        {
            audioSource.Stop(); // Para o som de carregamento
            ReleaseCharge();
        }
    }

    private void StartCharge(Vector2 mousePos)
    {
        isHoldingCharge = true;
        clickStartPosition = mousePos;

        // Desativa o Movement.cs para congelar o input padr�o sem mexer no c�digo dele
        if (movement != null) movement.enabled = false;

        // Libera o cursor para capturar o movimento do mouse na tela
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (animPlayer != null)
        {
            animPlayer.currentState = Player_AnimatorController.AnimState.Idle;
        }
    }

    private void UpdateCharge(Vector2 currentMousePos)
    {
        float pullDownDistance = clickStartPosition.y - currentMousePos.y;
        float clampedPull = Mathf.Clamp(pullDownDistance, 0f, maxPullDistance);

        currentChargePercent = clampedPull / maxPullDistance;
        currentChargeForce = currentChargePercent * maxChargeForce;

        // Mant�m o player firme no ch�o enquanto carrega
        if (controller.isGrounded)
        {
            controller.Move(Vector3.down * 2f * Time.deltaTime);
        }
    }

    private void ReleaseCharge()
    {
        isHoldingCharge = false;

        // Trava o mouse de volta para o controle da c�mera
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // S� dispara se tiver carregado mais de 5%
        if (currentChargePercent > 0.05f)
        {
            StartArrancada(currentChargeForce);
        }
        else
        {
            // Se soltou sem carregar, devolve o controle imediatamente
            if (movement != null) movement.enabled = true;
        }

        currentChargePercent = 0f;
        currentChargeForce = 0f;
    }

    private void StartArrancada(float force)
    {
        isCharging = true;
        audioSource.PlayOneShot(runSound); // Toca o som de corrida

        // Define a dire��o da arrancada
        arrancadaDirection = transform.forward * force;

        if (animPlayer != null)
        {
            animPlayer.currentState = Player_AnimatorController.AnimState.Run;
        }
    }

    private void UpdateArrancadaMovement()
    {
        

        // Reduz a velocidade da arrancada gradualmente
        arrancadaDirection = Vector3.MoveTowards(arrancadaDirection, Vector3.zero, arrancadaDeceleration * Time.deltaTime);

        // Seta a velocidade da anima��o
        if (animPlayer != null && movement != null)
        {
            animPlayer.SetRunAnimationSpeed(arrancadaDirection.magnitude, movement.maxSpeed);
        }

        if (controller.isGrounded)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity -= (movement != null ? movement.gravity : 20f) * Time.deltaTime;
        }

        // Move o CharacterController manualmente
        Vector3 finalMotion = arrancadaDirection + Vector3.up * verticalVelocity;
        controller.Move(finalMotion * Time.deltaTime);

        float normalMaxSpeed = (movement != null) ? movement.maxSpeed : 8f;
        if (arrancadaDirection.magnitude <= normalMaxSpeed) // Se a velocidade da arrancada for menor que a velocidade normal do player, encerra o charge
        {
            EndCharge();
        }
    }

    private void EndCharge()
    {
        isCharging = false;
        audioSource.Stop(); // Para o som de corrida

        // Reativa o script Movement.cs para devolver o controle normal ao jogador
        if (movement != null)
        {
            movement.enabled = true;
        }
    }

    private void FillChangeColor()
    {
        if (sliderFillImage != null)
        {
            sliderFillImage.color = Color.Lerp(Color.green, Color.red, currentChargePercent);
        }
    }

    // Chamado automaticamente pela Unity quando o CharacterController colide com algo durante o Move()
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Colis�o com Objetos Destru�veis no Charge
        if (isCharging && hit.gameObject.CompareTag(destructibleTag))
        {
            audioSource.PlayOneShot(deathEnemyClip); // Toca o som de destrui��o
            Destroy(hit.gameObject);
        }

        // Colis�o com Inimigos
        if (hit.gameObject.CompareTag("Enemy"))
        {
            CameraShakeManager.Instance.TremerCam(impulseSource);
            
            if (isCharging)
            {
                // Tenta pegar o script KeyEnemy do objeto colidido
                KeyEnemy keyEnemy = hit.gameObject.GetComponent<KeyEnemy>();

                if (keyEnemy != null)
                {
                    // Se tiver o script KeyEnemy, chama a fun��o Die()
                    audioSource.PlayOneShot(deathEnemyClip); // Toca o som de destrui��o
                    keyEnemy.Die();
                }
                else
                {
                    // Se n�o tiver o script KeyEnemy, apenas destr�i o objeto normalmente
                    
                    audioSource.PlayOneShot(deathEnemyClip); // Toca o som de destrui��o
                    Destroy(hit.gameObject);
                }
            }
            else
            {
                if (animPlayer != null)
                {
                    animPlayer.AnimDeath(); // Mata o jogador se colidir sem estar em Charge
                }
            }
        }
    }

    // Getters p�blicos
    public float GetChargePercent() => currentChargePercent;
    public float GetChargeForce() => currentChargeForce;
    public bool IsCharging() => isHoldingCharge;
}