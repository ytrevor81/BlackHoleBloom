using UnityEngine;

public class CommonAnimationEvents : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void DisableAnimator_AnimationEvent()
    {
        animator.enabled = false;
    }
    public void DisableObject_AnimationEvent()
    {
        gameObject.SetActive(false);
    }
}
