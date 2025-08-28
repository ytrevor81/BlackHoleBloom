using UnityEngine;

public class PlayerSettings : ScriptableObject
{
    [field: SerializeField] public bool HapticsDisabled { get; set; } = false;
    [field: SerializeField] public float MasterVolume { get; set; } = 1f;
    [field: SerializeField] public float MusicVolume { get; set; } = 1f;
    [field: SerializeField] public float SFXVolume { get; set; } = 1f;
    [field: SerializeField] public bool HideJoystick { get; set; } = false;

    public delegate void SettingsChangedHandler();
    public event SettingsChangedHandler OnJoystickVisibilityChanged;

    public void ToggleJoystickVisibility(bool _hide)
    {
        HideJoystick = _hide;
        OnJoystickVisibilityChanged?.Invoke();
    }

    private void ResetSettingsToDefault()
    {
        HapticsDisabled = false;
        MasterVolume = 1f;
        MusicVolume = 1f;
        SFXVolume = 1f;
        HideJoystick = false;
    }
#if UNITY_EDITOR
    void OnEnable()
    {
        ResetSettingsToDefault();
    }
#endif

    void OnDisable()
    {
        OnJoystickVisibilityChanged = null;
    }
}