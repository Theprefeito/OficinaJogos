using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
   
    
    public float speed;
    
    private Rigidbody rb;
    
    private Vector2 direction;

    public Transform camPosition;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 mover = new Vector3(direction.x, 0, direction.y);
        
        rb.MovePosition(rb.transform.position + mover * speed * Time.fixedDeltaTime);
        
        // Olha pra cam
        rb.transform.rotation = Quaternion.Euler(0, camPosition.rotation.eulerAngles.y, 0);
    }

    public void Move(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();
    }
}
