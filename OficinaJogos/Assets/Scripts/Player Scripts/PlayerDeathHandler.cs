using UnityEngine;

/// <summary>
/// Reage ao evento Died do PlayerHealth: toca a animação de morte e desliga os controles.
/// Para adicionar algo novo na morte (som, reiniciar fase, tela de game over),
/// basta criar outro script que se inscreva em PlayerHealth.Died.
/// </summary>
[RequireComponent(typeof(PlayerHealth))]
public class PlayerDeathHandler : MonoBehaviour
{
    private PlayerHealth health;
    private Player_AnimatorController animPlayer;

    private void Awake()
    {
        health = GetComponent<PlayerHealth>();
        animPlayer = FindAnyObjectByType<Player_AnimatorController>();
    }

    private void OnEnable() => health.Died += HandleDeath;
    private void OnDisable() => health.Died -= HandleDeath;

    private void HandleDeath()
    {
        // Desliga os controles
        DisableIfExists<Movement>();
        DisableIfExists<Player_Charge>();
        DisableIfExists<PlayerDash>();

        // Garante o cursor travado caso morra no meio do carregamento do Charge
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (animPlayer != null) animPlayer.AnimDeath();
    }

    private void DisableIfExists<T>() where T : Behaviour
    {
        var component = GetComponent<T>();
        if (component != null) component.enabled = false;
    }
}
