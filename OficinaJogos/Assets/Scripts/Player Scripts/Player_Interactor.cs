using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Interactor : MonoBehaviour
{
    [Header("Configuracoes de Interacao")]
    public float interactionDistance = 3f;
    public bool canInteract = false;
    public bool Cangrabable = false;
    
    [Header("Configuracoes de Grab")]
    [SerializeField] private Transform Hand;
    public bool canDrop = false;
    private GameObject grabItem;
    private Rigidbody rbItem;
    
    
    private Camera playerCamera; //Posição antiga da cabeça X0,Y1,Z0 // Nova: X0, Y0.8 ,Z0 // Mudança feita devido ao hit, se ficar muito alta o player não consegue dropar o item simplesmente clicando np G
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
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward); //Criando um novo ray a parti da posi��o da camera ate sua frente

        if(Physics.Raycast(ray, out hit, interactionDistance)) //Verificando se o ray colidiu com algo dentro da distancia de interacao, o out serve para armazenar as informacoes do objeto colidido na variavel hit
        {
            Debug.DrawLine(ray.origin, ray.direction * hit.distance, Color.green);

            if(hit.collider.CompareTag("Interagivel"))
            {
                canInteract = true;



                // Aqui voce pode adicionar a logica para interagir com o objeto
                //Debug.Log("Objeto Interagivel Detectado: " + hit.collider.name);
            }
         
            else if (hit.collider.CompareTag("Grabable"))
           {
             
               Cangrabable = true;
           }

        }
        else
        {
            canInteract = false;
            Cangrabable = false;
        }
    }

    public void InputInteract(InputAction.CallbackContext context) //Input para interagir com o objeto observado 
    {
        if (context.performed && canInteract) 
        {
            // Aqui voce pode adicionar a logica para interagir com o objeto
            
            
            Debug.Log("Interagindo com o objeto: " + hit.collider.name);
        }
     
        else if (context.performed && Cangrabable && !canDrop) //Adicionado Candrop na lógica para não pesar na memória devido a várias inoputs que podiam ocorrer indevidamente
        {
            // Aqui voce pode adicionar a logica para interagir com o objeto
            
            rbItem = hit.collider.GetComponent<Rigidbody>(); //Guarda informação do Rigdbody do item colidido
            rbItem.isKinematic = true; //Acessa o Ridbody e modifica
            
            grabItem = hit.collider.gameObject; //Guarda informação do item que foi colidido
            grabItem.transform.SetParent(Hand); //Seta o item como parente da mão
            grabItem.transform.localPosition = Vector3.zero; // Deixa igual as posições do objeto colidido com a da mão
            grabItem.transform.localRotation = Quaternion.identity; // Deixa igual as rotações do objeto colidido com a da mão
            
            canDrop = true;
            
            Debug.Log("Interagindo com o objeto: " + hit.collider.name);
        }
    }

    public void InputDrop(InputAction.CallbackContext context)
    {
        if (context.performed && canDrop)
        {
          
            rbItem = hit.collider.GetComponent<Rigidbody>(); //Guarda informação do Rigdbody do item colidido
            rbItem.isKinematic = false; //Acessa o Ridbody e modifica
            
            grabItem = hit.collider.gameObject; //Guarda informação do item que foi colidido
            grabItem.transform.SetParent(null); //Seta o item como parente de nada
           
            
            canDrop = false;
        }
    }

}
