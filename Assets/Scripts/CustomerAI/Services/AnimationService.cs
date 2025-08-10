using UnityEngine;

public class AnimationService : MonoBehaviour, IAnimationService
{
    [SerializeField] private Animator animator;
    
    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }
    
    public void PlayWalkAnimation()
    {
        animator?.SetBool("IsWalking", true);
    }
    
    public void PlayIdleAnimation()
    {
        animator?.SetBool("IsWalking", false);
    }
    
    public void PlayInteractAnimation()
    {
        animator?.SetBool("IsInteracting", true);
    }

    public void StopInteractAnimation()
    {
        animator?.SetBool("IsInteracting", false);
    }
}
