using UnityEngine;

public class Player_AnimatorController : MonoBehaviour
{
    private Animator animator;
    public bool tanoChao;
    public enum AnimState
    {
        Idle,
        Run,
        Jump,
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
            case AnimState.Jump:
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
        
        if(currentState == AnimState.Run)
        {
            OnRun();
        }
       
        if(currentState == AnimState.Jump)
        {
            OnJump();
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
        animator.SetBool("IsJump", false);
    }

    private void OnRun()
    {
        
        animator.SetBool("IsRun", true);
        animator.SetBool("IsJump", false);        
    }

     private void OnJump()
    {
        
        animator.SetBool("IsJump", true);
        animator.SetBool("IsRun", false);
    }



}
