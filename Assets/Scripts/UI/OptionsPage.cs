using System.Collections;
using UnityEngine;

public class OptionsPage : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private GameManager GM;
    private AudioManager AM;

    public enum SettingType
    {
        MasterVolume,
        MusicVolume,
        SFXVolume,
        Vibration,
        HideJoystick
    }

    [SerializeField] private OptionsSlider masterVolumeSlider;
    [SerializeField] private OptionsSlider musicVolumeSlider;
    [SerializeField] private OptionsSlider sfxVolumeSlider;
    [SerializeField] private OptionsOnOffSwitch vibrationSwitch;
    [SerializeField] private OptionsOnOffSwitch hideJoystickSwitch;

    [Space]

    [SerializeField] private float fadeInAndOutTime;

    [Space]

    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private MainMenu mainMenu;
    private bool initialized;
    private IEnumerator currentCoroutine;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    void OnEnable()
    {
        GM = GameManager.Instance;
        AM = GM.AudioManager;

        hideJoystickSwitch.InitializeSwitch(GM.Settings.HideJoystick);
        vibrationSwitch.InitializeSwitch(!GM.Settings.HapticsDisabled);
        masterVolumeSlider.InitializeSlider(GM.Settings.MasterVolume);
        musicVolumeSlider.InitializeSlider(GM.Settings.MusicVolume);
        sfxVolumeSlider.InitializeSlider(GM.Settings.SFXVolume);

        initialized = true;

        currentCoroutine = FadeInCoroutine();
        StartCoroutine(currentCoroutine);
    }

    void OnDisable()
    {
        initialized = false;
        StopCurrentCoroutine();
    }

    private void StopCurrentCoroutine()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
    }

    private IEnumerator FadeInCoroutine()
    {
        float elapsedTime = 0;
        float currentAlpha = canvasGroup.alpha;

        while (elapsedTime < fadeInAndOutTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(currentAlpha, 1, elapsedTime / fadeInAndOutTime);
            yield return null;
        }

        canvasGroup.alpha = 1;
    }
    private IEnumerator FadeOutCoroutine()
    {
        float elapsedTime = 0;
        float currentAlpha = canvasGroup.alpha;

        while (elapsedTime < fadeInAndOutTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(currentAlpha, 0, elapsedTime / fadeInAndOutTime);
            yield return null;
        }

        canvasGroup.alpha = 0;

        if (pauseMenu != null)
            pauseMenu.BackFromOptionsPage();

        else if (mainMenu != null)
            mainMenu.BackFromOptionsPage();

        gameObject.SetActive(false);
    }

    public void OnOffSwitchPressed(SettingType settingType, bool isOn)
    {
        if (!initialized) return;

        if (settingType == SettingType.Vibration)
            GM.Settings.HapticsDisabled = !isOn;
        
        else if (settingType == SettingType.HideJoystick)
            GM.Settings.ToggleJoystickVisibility(isOn);
    }
    public void SliderValueChanged(SettingType settingType, float _value)
    {
        if (!initialized) return;

        if (settingType == SettingType.MasterVolume)
            AM.ChangeMasterVolume(_value);

        else if (settingType == SettingType.MusicVolume)
            AM.ChangeMusicVolume(_value);

        else if (settingType == SettingType.SFXVolume)
            AM.ChangeSFXVolume(_value);
    }

    public void BackButtonPressed_Event()
    {
        if (!initialized) return;

        initialized = false;

        StopCurrentCoroutine();
        currentCoroutine = FadeOutCoroutine();
        StartCoroutine(currentCoroutine);
    }
}
