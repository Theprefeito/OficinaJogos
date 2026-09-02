using UnityEngine;


public class PositionManager : MonoBehaviour
{
    [Header("Position")]
    public static PositionManager Instance;

    public Vector3 position;
    public Quaternion rotation;

    [Header("Scene")]
    
    
    public bool SavedPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            // return;
        }
    }

   
    
}
