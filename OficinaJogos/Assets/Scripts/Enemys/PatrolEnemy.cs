using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
   
    [SerializeField] private Transform[] waypoints;
    public int wayIndex;
    public float speedEnemy;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wayIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, waypoints[wayIndex].position, speedEnemy * Time.deltaTime);
       
        if (transform.position == waypoints[wayIndex].position)
        {
            wayIndex += 1;

            if (wayIndex >= waypoints.Length)
            {
                wayIndex = 0;
            }
        }

    
    }

    
    
    
}
