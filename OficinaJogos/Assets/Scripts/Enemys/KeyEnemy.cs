using UnityEngine;
using System;

public class KeyEnemy : MonoBehaviour
{
    // Evento que avisa quando o inimigo morre
    public event Action OnEnemyDied;

    /// <summary>
    /// Chama este método para matar o inimigo (ex: ao levar o Charge)
    /// </summary>
    public void Die()
    {
        // Dispara o evento avisando quem estiver ouvindo
        OnEnemyDied?.Invoke();

        // Destrói o inimigo
        Destroy(gameObject);
    }
}