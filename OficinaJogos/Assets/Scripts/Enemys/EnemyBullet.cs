using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("Configurações da Bala")]
    [SerializeField] private float speed = 20f;
    [SerializeField] private int damage = 1;
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
        if (!other.CompareTag("Player")) return;

        // A vida pode estar no objeto pai do collider que foi atingido
        PlayerHealth health = other.GetComponentInParent<PlayerHealth>();

        // Se o player estiver invulnerável (ex.: Charge), o TakeDamage bloqueia sozinho
        if (health != null)
        {
            health.TakeDamage(damage, gameObject);
        }

        // A bala some em qualquer caso
        Destroy(gameObject);
    }
}