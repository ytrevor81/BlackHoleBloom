#if !UNITY_EDITOR
using CandyCoded.HapticFeedback;
#endif
using UnityEngine;

public class HapticsManager : MonoBehaviour
{
    public bool HapticsDisabled { get; set; }

    public void SmallVibration()
    {
        if (HapticsDisabled)
            return;

#if !UNITY_EDITOR
        HapticFeedback.LightFeedback();
#endif
    }

    public void MediumVibration()
    {
        if (HapticsDisabled)
            return;

#if !UNITY_EDITOR
        HapticFeedback.MediumFeedback();
#endif
    }

    public void HeavyVibration()
    {
        if (HapticsDisabled)
            return;
            
#if !UNITY_EDITOR
        //HapticFeedback.HeavyFeedback();
        Handheld.Vibrate();
#endif
    }
}
