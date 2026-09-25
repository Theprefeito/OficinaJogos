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

  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        rbEnemy =  GetComponent<Rigidbody>();
        wayIndex = 0;
    }

    void FixedUpdate()
    {

      
        {
            EnemyPatrolMovement();
                    
            WaypointPatrol();
        }
        
    }
    // Update is called once per frame
    void Update()
    {
    
    }

    private void EnemyPatrolMovement()
    {
        Vector3 posicao = Vector3.MoveTowards(transform.position, waypoints[wayIndex].position, speedEnemy * Time.fixedDeltaTime);
        rbEnemy.MovePosition(posicao);
    }

    private void WaypointPatrol()
    {
        if (Vector3.Distance(transform.position, waypoints[wayIndex].position) <= distanceToWaypoint)
        {
            wayIndex += 1;

            if (wayIndex >= waypoints.Length)
            {
                wayIndex = 0;
            }
            
        }
    }
   
}
