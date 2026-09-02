using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSaveSpawn : MonoBehaviour
{
    //Estudar como usar os playerPrefs sem pesar na memória
    
    
    private Movement movement;
    private Player_Charge charge;

    void Start()
    {
        movement = GetComponent<Movement>();
        charge = GetComponent<Player_Charge>();


        StartCoroutine(EsperarACena()); 
        
    }

  
    void Update()
    {

       

    }
    
    private void SpawnPlayer()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0 && PositionManager.Instance.SavedPosition == true) 
        {
                Debug.Log(transform.position);
                
                movement.enabled = false;
                charge.enabled = false;
            
                transform.position = PositionManager.Instance.position;
                transform.rotation = PositionManager.Instance.rotation;
               
                PositionManager.Instance.SavedPosition = false;
                
                movement.enabled = true;
                charge.enabled = true;
        }
    }
    
    
    private IEnumerator EsperarACena()
    {
        yield return null;
        
        SpawnPlayer();
    }
    
}
