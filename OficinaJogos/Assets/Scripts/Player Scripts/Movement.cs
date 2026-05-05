using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.EventSystems.StandaloneInputModule;

public class Movement : MonoBehaviour
{
       
    public float speed;
    
    private Rigidbody rb;
    
    private Vector2 direction;

    public Transform camPosition;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Essas duas linhas são momentanias, elas servem para travar o cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Cria o movimento
        Vector3 mover = new Vector3(direction.x, 0, direction.y);
        // Converte o movimento para ser relativo à rotação do jogador
        Vector3 relativeMove = transform.TransformDirection(mover);

        //Aplica o movimento usando a posição atual + o deslocamento relativo
        rb.MovePosition(rb.position + relativeMove * speed * Time.fixedDeltaTime);

        if (camPosition != null)
        {
            // Olha pra cam
            rb.transform.rotation = Quaternion.Euler(0, camPosition.eulerAngles.y, 0);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();
    }
}
