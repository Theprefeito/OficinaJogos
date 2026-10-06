using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("Configurações da Bala")]
    [SerializeField] private float speed = 20f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifeTime = 5f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rb.linearVelocity = transform.forward * speed;

        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player_AnimatorController playerAnimator = other.GetComponent<Player_AnimatorController>();
            playerAnimator?.AnimDeath(); // Chama a animação de morte do jogador
        }
    }

}