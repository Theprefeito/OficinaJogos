using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player_Charge : MonoBehaviour
{
    [Header("Configurações do Charge")]
    public float maxPullDistance = 300f;
    public float maxChargeForce = 50f;

    [Header("Configurações da Arrancada")]
    public float arrancadaDeceleration = 30f;
    [Tooltip("Tag dos objetos que podem ser destruídos durante o Charge")]
    public string destructibleTag = "Destructible"; // Tag configurável para os objetos que podem ser destruídos

    [Header("Ui")]
    public Slider chargeSlider;
    public Image sliderFillImage;

    private float currentChargePercent;
    private float currentChargeForce;

    // Referências do GameObject
    private Movement movement;
    private CharacterController controller;
    private Player_AnimatorController animPlayer;

    [Header("Controle de Estados")]
    private Vector2 clickStartPosition;
    private bool isHoldingCharge = false;
    public bool isCharging = false;

    // Física Interna do Charge
    private Vector3 arrancadaDirection;
    private float verticalVelocity = -2f;

    void Start()
    {
        movement = GetComponent<Movement>();
        controller = GetComponent<CharacterController>();
        animPlayer = FindAnyObjectByType<Player_AnimatorController>();
    }

    void Update()
    {
        HandleChargeInput(); // Botões do charge
        FillChangeColor();   // Atualiza a cor do slider

        if (isCharging)
        {
            UpdateArrancadaMovement();
        }

        if (isHoldingCharge)
        {
            chargeSlider.value = currentChargeForce;
        }
        else
        {
            chargeSlider.value = 0f;
        }
    }

    private void HandleChargeInput()
    {
        var mouse = Mouse.current;
        if (mouse == null || isCharging) return;

        // Quando o botão esquerdo do mouse é pressionado, inicia o Charge
        if (mouse.leftButton.wasPressedThisFrame && animPlayer.currentState != Player_AnimatorController.AnimState.Jump)
        {
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
            ReleaseCharge();
        }
    }

    private void StartCharge(Vector2 mousePos)
    {
        isHoldingCharge = true;
        clickStartPosition = mousePos;

        // Desativa o Movement.cs para congelar o input padrão sem mexer no código dele
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

        // Mantém o player firme no chão enquanto carrega
        if (controller.isGrounded)
        {
            controller.Move(Vector3.down * 2f * Time.deltaTime);
        }
    }

    private void ReleaseCharge()
    {
        isHoldingCharge = false;

        // Trava o mouse de volta para o controle da câmera
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Só dispara se tiver carregado mais de 5%
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

        // Define a direção da arrancada
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

        // Seta a velocidade da animação
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
        // Colisão com Objetos Destruíveis no Charge
        if (isCharging && hit.gameObject.CompareTag(destructibleTag))
        {
            Destroy(hit.gameObject);
        }

        // Colisão com Inimigos
        if (hit.gameObject.CompareTag("Enemy"))
        {
            if (isCharging)
            {
                // Tenta pegar o script KeyEnemy do objeto colidido
                KeyEnemy keyEnemy = hit.gameObject.GetComponent<KeyEnemy>();

                if (keyEnemy != null)
                {
                    // Se tiver o script KeyEnemy, chama a função Die()
                    keyEnemy.Die();
                }
                else
                {
                    // Se não tiver o script KeyEnemy, apenas destrói o objeto normalmente
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

    // Getters públicos
    public float GetChargePercent() => currentChargePercent;
    public float GetChargeForce() => currentChargeForce;
    public bool IsCharging() => isHoldingCharge;
}