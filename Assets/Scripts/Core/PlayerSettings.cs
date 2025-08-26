using UnityEngine;

public class PlayerSettings : ScriptableObject
{
    [field: SerializeField] public bool HapticsDisabled { get; set; } = false;
    [field: SerializeField] public float MasterVolume { get; set; } = 1f;
    [field: SerializeField] public float MusicVolume { get; set; } = 1f;
    [field: SerializeField] public float SFXVolume { get; set; } = 1f;

    private void ResetSettingsToDefault()
    {
        HapticsDisabled = false;
        MasterVolume = 1f;
        MusicVolume = 1f;
        SFXVolume = 1f;
    }
#if UNITY_EDITOR
    void OnEnable()
    {
        ResetSettingsToDefault();
    }
#endif
}