using UnityEngine;

public class Movement : MonoBehaviour
{
   
    
    public float speed;
    
    private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W))
            {
               rb.linearVelocity = new Vector2(rb.linearVelocity.x, speed);
            }
    }
}
