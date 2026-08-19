using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSaveSpawn : MonoBehaviour
{
    private Transform playerTr;
    private Quaternion playerRotation;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTr = GetComponent<Transform>();
        playerRotation = GetComponent<Transform>().rotation;
    }

    /*// Update is called once per frame
    void Update()
    {

        if (SceneManager.GetActiveScene().name == "TestScene") // Esse tá bugando 
        {
            playerTr.position = PositionManager.Instance.position;
            playerTr.rotation = PositionManager.Instance.rotation;
            
            PositionManager.Instance.SavedPosition = false;
        }

    }
    */
}
