using UnityEngine;

public class Player_AnimatorController : MonoBehaviour
{
    private Animator animator;
    public enum AnimState
    {
        Idle,
        Run,
    }

    public AnimState currentState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        switch(currentState)
        {
            case AnimState.Idle:
                break;
            case AnimState.Run:
                break;
        }
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentState == AnimState.Idle)
        {
            OnIdle();
        }
        else if(currentState == AnimState.Run)
        {
            OnRun();
        }
    }

    public void SetRunAnimationSpeed(float currentSpeed, float maxSpeed)
    {
        float speedPercent = currentSpeed / maxSpeed;
        animator.speed = Mathf.Lerp(0.8f, 1.4f, speedPercent);
    }

    private void OnIdle()
    {
        
        animator.SetBool("IsRun", false);
    }

    private void OnRun()
    {
        
        animator.SetBool("IsRun", true);        
    }

    
}
