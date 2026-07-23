using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;


public class Recenter : MonoBehaviour
{

    private CinemachineOrbitalFollow recenter;
    private CinemachineInputAxisController inputsMouse;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        recenter = GetComponent<CinemachineOrbitalFollow>();
        inputsMouse = GetComponent<CinemachineInputAxisController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RecenterCamera(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            recenter.HorizontalAxis.Recentering.Enabled = true;
            inputsMouse.enabled = false;
        }
        else
        {
            recenter.HorizontalAxis.Recentering.Enabled = false;
            inputsMouse.enabled = true;
        }
    }
    
    
}
