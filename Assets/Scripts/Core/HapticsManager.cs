#if !UNITY_EDITOR
using CandyCoded.HapticFeedback;
#endif
using UnityEngine;

public class HapticsManager : MonoBehaviour
{
    [SerializeField] private PlayerSettings settings;

    public void SmallVibration()
    {
        if (settings.HapticsDisabled)
            return;

#if !UNITY_EDITOR
        HapticFeedback.LightFeedback();
#endif
    }

    public void MediumVibration()
    {
        if (settings.HapticsDisabled)
            return;

#if !UNITY_EDITOR
        HapticFeedback.MediumFeedback();
#endif
    }

    public void HeavyVibration()
    {
        if (settings.HapticsDisabled)
            return;
            
#if !UNITY_EDITOR
        //HapticFeedback.HeavyFeedback();
        Handheld.Vibrate();
#endif
    }
}
