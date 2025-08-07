using FMOD.Studio;
using UnityEngine;

public class CommonButtonBehavior : MonoBehaviour
{
    private enum SFXType
    {
        ButtonPress,
        EnterGame
    }

    [SerializeField] private SFXType sfxType;
    [SerializeField] private Animator animator;
    private SFXBank sfx;
    private const string CLICKED_TRIGGER = "Clicked";

    public void PressedVFX_Event()
    {
        if (sfx == null)
            sfx = GameManager.Instance.AudioManager.SFXBank;
        
        BHBAudio.PlayOneShotFromLoadedSampleData(GetEventDescription(), transform.position);

        if (animator.enabled)
            animator.SetTrigger(CLICKED_TRIGGER);

        else
            animator.enabled = true;
    }

    private EventDescription GetEventDescription()
    {
        if (sfxType == SFXType.EnterGame)
            return sfx.EnterGameDescription;

        else
            return sfx.ButtonPressDescription;
    }

    public void DisableAnimator_AnimationEvent()
    {
        animator.enabled = false;
    }
}
