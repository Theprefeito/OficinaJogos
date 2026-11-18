using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class PatrolEnemy : MonoBehaviour
{
    Rigidbody rbEnemy;
    [Header("Points To Patrol")]
    [SerializeField] private Transform[] waypoints;

    public int wayIndex;
    [Header("Values for patrol")]
    [SerializeField] public float distanceToWaypoint;
    [Header("Enemy Stats")]
    [SerializeField] public float speedEnemy;
    [SerializeField] private Transform target;
    [SerializeField] public int enemyWaitTime;
    [SerializeField] private int contactDamage = 1;
    public bool isWaiting;
    public bool seguindo;

    private CinemachineImpulseSource impulseSource;

    void Start()
    {
        rbEnemy = GetComponent<Rigidbody>();
        wayIndex = 0;

        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void FixedUpdate()
    {
        CheckPatrolMovement();
    }

    private void EnemyPatrolMovement()
    {
        Vector3 posicao = Vector3.MoveTowards(transform.position, waypoints[wayIndex].position, speedEnemy * Time.fixedDeltaTime);
        rbEnemy.MovePosition(posicao);
    }

    private void WaypointPatrol()
    {
        if (Vector3.Distance(transform.position, waypoints[wayIndex].position) <= distanceToWaypoint && !isWaiting)
        {
            StartCoroutine(WaitTime());
        }
    }

    private IEnumerator WaitTime()
    {
        isWaiting = true;
        yield return new WaitForSeconds(enemyWaitTime);

        wayIndex += 1;

        if (wayIndex >= waypoints.Length)
        {
            wayIndex = 0;
        }

        isWaiting = false;
    }

    private void CheckPatrolMovement()
    {
        if (!isWaiting)
        {
            EnemyPatrolMovement();
            WaypointPatrol();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        PlayerHealth health = collision.gameObject.GetComponentInParent<PlayerHealth>();

        // Se o player estiver em Charge (invulnerável), o TakeDamage é bloqueado automaticamente.
        // Não precisa mais checar isCharging aqui.
        if (health != null)
        {
            health.TakeDamage(contactDamage, gameObject);
        }
    }
}