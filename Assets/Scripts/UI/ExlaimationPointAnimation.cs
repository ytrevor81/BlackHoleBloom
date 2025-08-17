using UnityEngine;

public class ExlaimationPointAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private bool keepInIdle;
    private int idleLoops;

    void OnEnable()
    {
        idleLoops = 0;
    }

    public void IdleLoopComplete_AnimationEvent()
    {
        if (keepInIdle)
            return;

        idleLoops += 1;

        if (idleLoops >= 2)
            animator.SetTrigger("Exit");
    }
    public void DisableObject_animationEvent()
    {
        gameObject.SetActive(false);
    }
}
