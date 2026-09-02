using UnityEngine;

public class LocalOfSave : MonoBehaviour
{
   
  
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    
    

   void OnTriggerEnter(Collider collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }
       
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Bateu");
                    
            PositionManager.Instance.position = collision.transform.position;
            PositionManager.Instance.rotation = collision.transform.rotation;
        
            PositionManager.Instance.SavedPosition = true;
            
            
        }
    }
    
    
    
}
