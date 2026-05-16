using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Interactor : MonoBehaviour
{
    [Header("Configurações de Interação")]
    public float interactionDistance = 3f;
    public bool canInteract = false;

    private Camera playerCamera;
    private RaycastHit hit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastInteraction();
    }

    private void RaycastInteraction() //void resposavel por verificar se o jogador pode interagir com algo usando um raycast
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward); //Criando um novo ray a parti da posição da camera ate sua frente

        if(Physics.Raycast(ray, out hit, interactionDistance)) //Verificando se o ray colidiu com algo dentro da distancia de interação, o out serve para armazenar as informações do objeto colidido na variavel hit
        {
            Debug.DrawLine(ray.origin, ray.direction * hit.distance, Color.green);

            if(hit.collider.CompareTag("Interagivel"))
            {
                canInteract = true;



                // Aqui você pode adicionar a lógica para interagir com o objeto
                //Debug.Log("Objeto Interagível Detectado: " + hit.collider.name);
            }

        }
        else
        {
            canInteract = false;
        }
    }

    public void InputInteract(InputAction.CallbackContext context) //Input para interagir com o objeto observado 
    {
        if (context.performed && canInteract)
        {
            // Aqui você pode adicionar a lógica para interagir com o objeto
            Debug.Log("Interagindo com o objeto: " + hit.collider.name);
        }
    }
           
    

}
