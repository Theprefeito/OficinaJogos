using System.Collections;
using UnityEngine;

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

    [SerializeField] public int enemyWaitTime;
    public bool isWaiting;

    void Start()
    {
        rbEnemy = GetComponent<Rigidbody>();
        wayIndex = 0;
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
        if (collision.gameObject.CompareTag("Player"))
        {
            Player_Charge playerCharge = collision.gameObject.GetComponent<Player_Charge>();
            Player_AnimatorController playerAnim = collision.gameObject.GetComponent<Player_AnimatorController>();

            // Se o player estiver dando o charge, o inimigo é destruído
            if (playerCharge != null && playerCharge.isCharging)
            {
                Destroy(gameObject); // Opcional: Destrói o inimigo se levar o charge
            }
            // Se o player NÃO estiver em charge, ele morre
            else if (playerAnim != null)
            {
                playerAnim.AnimDeath();
            }
        }
    }
}