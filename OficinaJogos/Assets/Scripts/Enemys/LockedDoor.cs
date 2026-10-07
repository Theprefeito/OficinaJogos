using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    [Header("Inimigo Chave")]
    [Tooltip("Arraste aqui o inimigo que precisa morrer para destruir este objeto")]
    [SerializeField] private KeyEnemy targetEnemy;

    private void OnEnable()
    {
        if (targetEnemy != null)
        {
            // Se inscreve no evento de morte do inimigo
            targetEnemy.OnEnemyDied += DestroyThisObject;
        }
    }

    private void OnDisable()
    {
        if (targetEnemy != null)
        {
            // Cancela a inscrição para evitar erros de memória
            targetEnemy.OnEnemyDied -= DestroyThisObject;
        }
    }

    private void DestroyThisObject()
    {
        Destroy(gameObject);
    }
}