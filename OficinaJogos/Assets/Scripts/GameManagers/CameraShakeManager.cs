using Unity.Cinemachine;
using UnityEngine;

public class CameraShakeManager : MonoBehaviour
{
   
    [SerializeField] public float shakeAmount;
    public static CameraShakeManager Instance;
   
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
        }
    }
    
  
  public void TremerCam(CinemachineImpulseSource impulseSource)
  {
      impulseSource.GenerateImpulseWithForce(shakeAmount);
  }
  
}
