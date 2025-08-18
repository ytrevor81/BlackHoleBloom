using System;
using System.Collections;
using System.Text;
using Cinemachine;
using FMOD.Studio;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public static HUDController Instance;
    private GameManager GM;
    private PlayerController player;

    [Header("MAIN REFS")]
    [Space]

    [SerializeField] private CanvasGroup overallHUDcanvasGroup;
    [SerializeField] private CanvasGroup upperHUDcanvasGroup;
    [SerializeField] private CanvasGroup lowerHUDcanvasGroup;
    [SerializeField] private CanvasGroup joystickCanvasGroup;
    // [SerializeField] private CodexPopupList codexPopupList;
    [SerializeField] private GameObject codexExclaimationPoint;

    [Header("TIMER")]
    [Space]

    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Animator timerAnimator;

    private bool isTimerActive;
    private TimeSpan timeSpan;

    [Header("PROGRESS BAR")]
    [Space]

    [SerializeField] private Image levelBar;
    [SerializeField] private Animator levelBarAnimator;
    [SerializeField] private Animator levelTextAnimator;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text objectsToAbsorbText;
    [SerializeField] private Image galaxyIcon;
    private Transform galaxyIconTransform;

    [Space]

    [SerializeField] private Color purpleColor;
    [SerializeField] private Color boostColor;
    [SerializeField] private Color blueColor;
    [SerializeField] private float iconAndBarcolorLerpTime;
    [SerializeField] private float galaxyIconRotationSpeed;
    private float galaxyIconRotation;
    private Color previousIconAndBarColor;
    private Color currentIconAndBarColor;
    private Color targetIconAndBarColor;
    private float elapsedTimeColorLerp;
    private bool isColorLerping;
    private const string SLASH = "/";

    private StringBuilder stringBuilder = new StringBuilder();


    [Header("MASS SCORE")]
    [Space]

    [SerializeField] private SpawnedNumberManager spawnedNumbers;
    [SerializeField] private TMP_Text scoreText;

    [Header("BOOST")]
    [Space]

    [SerializeField] private Animator boostAnimator;
    [SerializeField] private TMP_Text boostMultiplierText;

    [Header("CHANGE ROOM CUTSCENES")]
    [Space]

    [SerializeField] private CinemachineVirtualCamera mainCamera;
    [SerializeField] private float zoomTarget;
    [SerializeField] private float zoomTime;
    [SerializeField] private float fadeTimeUI;
    [SerializeField] private float musicEQFadeSpeed;
    [SerializeField] private float pitchChangeTime;

    private CinemachineFramingTransposer transposer;

    private const string LEVEL_UP_TRIGGER = "LevelUp";
    private const string GAIN_TRIGGER = "Gain";
    private const string END_BOOL = "end";
    private const string BOOST_TRIGGER = "Boost";

    private bool isFadingIn;
    private bool isFadingOut;
    private float fadeSpeed = 1f;
    private IEnumerator currentCoroutine;
    private WaitForSecondsRealtime delayOneSecond = new WaitForSecondsRealtime(1f);

    void Awake()
    {
        Instance = this;
        galaxyIconTransform = galaxyIcon.transform;
        targetIconAndBarColor = galaxyIcon.color;
        transposer = mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    IEnumerator Start()
    {
        player = PlayerController.Instance;
        levelBar.fillAmount = 0;
        isTimerActive = true;

        yield return null;

        GM = GameManager.Instance;
        GM.Timer = GM.RoomTime;

        timeSpan = TimeSpan.FromSeconds(GM.Timer);

        int minutes = timeSpan.Minutes;
        int seconds = timeSpan.Seconds;
        timerText.text = $"{minutes:D2}:{seconds:D2}";

        GM.OnLevelChanged += LevelUp;
        GM.OnRoomCompleted += RoomCompleted;
    }

    void OnDisable()
    {
        if (GM != null)
        {
            GM.OnLevelChanged -= LevelUp;
            GM.OnRoomCompleted -= RoomCompleted;
        }

        StopCurrentCoroutine();
    }

    private void StopCurrentCoroutine()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
    }

    void Update()
    {
        if (isTimerActive)
            UpdateTimer();
        
        if (player.inBoostMode)
            boostMultiplierText.text = HandleBoostMultiplierNum();

        HandleGalaxyIconAndProgressBarVFX();
        HandleHUDFadeInOrOut();
    }

    private string HandleBoostMultiplierNum()
    {
        if (player.BoostMultiplier <= 1)
            return "x2";

        else
        {
            int displayMultiplier = Mathf.Max(2, Mathf.CeilToInt(player.BoostMultiplier));
            return $"x{displayMultiplier}";
        }
    }

    public void BackToMainMenu()
    {
        StopCurrentCoroutine();

        GM.InvokeCutsceneStarted();
        isFadingOut = true;
        currentCoroutine = BackToMainMenuCoroutine();
        StartCoroutine(currentCoroutine);
    }

    public void PlayerLost()
    {
        StopCurrentCoroutine();

        GM.InvokeCutsceneStarted();
        isFadingOut = true;
        timerAnimator.enabled = true;

        currentCoroutine = PlayerLostCoroutine();
        StartCoroutine(currentCoroutine);
    }
    

    private IEnumerator PlayerLostCoroutine()
    {
        float elapsedTime = 0;
        float startZoom = transposer.m_CameraDistance;
        float currentZoom;
        float musicEQValue = 1f;
        string musicEQParameter = GM.AudioManager.MusicBank.MusicEQParameter;

        EventInstance mainMusicInstance = GM.AudioManager.GetMusicInstance();
        float currentPitch = 1;
        float targetPitch = 0.1f;
        float canvasAlpha = 1f;

        while (elapsedTime < zoomTime)
        {
            canvasAlpha -= Time.deltaTime * fadeSpeed;

            if (canvasAlpha <= 0)
                canvasAlpha = 0;

            upperHUDcanvasGroup.alpha = canvasAlpha;
            lowerHUDcanvasGroup.alpha = canvasAlpha;
            joystickCanvasGroup.alpha = canvasAlpha;

            musicEQValue -= Time.deltaTime * musicEQFadeSpeed;

            if (musicEQValue <= 0)
            {
                musicEQValue = 0;
                RuntimeManager.StudioSystem.setParameterByName(musicEQParameter, musicEQValue);
            }
            else
                RuntimeManager.StudioSystem.setParameterByName(musicEQParameter, musicEQValue);

            elapsedTime += Time.deltaTime;
            currentZoom = Mathf.Lerp(startZoom, zoomTarget, elapsedTime / zoomTime);
            currentPitch = Mathf.Lerp(1f, targetPitch, elapsedTime / zoomTime);
            mainMusicInstance.setPitch(currentPitch); //this was originally done in AudioManager but for some reason a null reference was being thrown that even confused AI why it was happening
            transposer.m_CameraDistance = currentZoom;

            yield return null;
        }

        GM.AudioManager.StopMusicFadeOut();

        yield return delayOneSecond;

        GM.BackToMainMenu();
    }
    private IEnumerator BackToMainMenuCoroutine()
    {
        float elapsedTime = 0;
        float startZoom = transposer.m_CameraDistance;
        float currentZoom;
        float musicEQValue = 1f;
        string musicEQParameter = GM.AudioManager.MusicBank.MusicEQParameter;

        while (elapsedTime < zoomTime)
        {
            musicEQValue -= Time.deltaTime * musicEQFadeSpeed;

            if (musicEQValue <= 0)
            {
                musicEQValue = 0;
                RuntimeManager.StudioSystem.setParameterByName(musicEQParameter, musicEQValue);
            }
            else
                RuntimeManager.StudioSystem.setParameterByName(musicEQParameter, musicEQValue);

            elapsedTime += Time.deltaTime;
            currentZoom = Mathf.Lerp(startZoom, zoomTarget, elapsedTime / zoomTime);
            transposer.m_CameraDistance = currentZoom;

            yield return null;
        }

        GM.AudioManager.StopMusicFadeOut();

        yield return delayOneSecond;

        GM.BackToMainMenu();
    }

    private void HandleHUDFadeInOrOut()
    {
        if (isFadingIn)
        {
            overallHUDcanvasGroup.alpha += fadeSpeed * Time.deltaTime;

            if (overallHUDcanvasGroup.alpha >= 1)
            {
                overallHUDcanvasGroup.alpha = 1;
                isFadingIn = false;
            }
        }
        else if (isFadingOut)
        {
            overallHUDcanvasGroup.alpha -= fadeSpeed * Time.deltaTime;

            if (overallHUDcanvasGroup.alpha <= 0)
            {
                overallHUDcanvasGroup.alpha = 0;
                isFadingOut = false;
            }
        }
    }

    public void CheckCodexEntry(CodexEntry _entryData)
    {
        //codexPopupList.AddToCodex(_entryData);
        if (_entryData.IsDiscovered)
            return;

        _entryData.IsDiscovered = true;
        codexExclaimationPoint.SetActive(true);
    }

    public void FadeOutHUD()
    {
        isFadingOut = true;
        isFadingIn = false;
    }

    public void FadeInHUD()
    {
        isFadingIn = true;
        isFadingOut = false;
    }

    private void HandleGalaxyIconAndProgressBarVFX()
    {
        galaxyIconRotation -= Time.deltaTime * (galaxyIconRotationSpeed * player.BoostMultiplier);
        galaxyIconTransform.rotation = Quaternion.Euler(0, 0, galaxyIconRotation);

        if (isColorLerping)
        {
            elapsedTimeColorLerp += Time.deltaTime;
            currentIconAndBarColor = Color.Lerp(previousIconAndBarColor, targetIconAndBarColor, elapsedTimeColorLerp / iconAndBarcolorLerpTime);
            galaxyIcon.color = currentIconAndBarColor;
            levelBar.color = currentIconAndBarColor;

            if (elapsedTimeColorLerp >= iconAndBarcolorLerpTime)
            {
                isColorLerping = false;
                galaxyIcon.color = targetIconAndBarColor;
                levelBar.color = targetIconAndBarColor;
            }
        }
    }

    private void UpdateTimer()
    {
        if (GM.SpecialAnimationPlaying || GM.CutscenePlaying)
            return;

        if (GM.Timer > 1)
        {
            GM.Timer -= Time.deltaTime;

            if (GM.Timer <= 1)
            {
                timerText.text = "0:00";
                return;
            }

            timeSpan = TimeSpan.FromSeconds(GM.Timer);

            int minutes = timeSpan.Minutes;
            int seconds = timeSpan.Seconds;

            timerText.text = $"{minutes:D2}:{seconds:D2}";
        }
        else
        {
            isTimerActive = false;

            if (GM.CurrentLevel != GameManager.Level.Level10)
                PlayerLost();
        }
    }

    private string LevelText()
    {
        if (GM.CurrentLevel == GameManager.Level.Level1)
            return "LVL 1";

        else if (GM.CurrentLevel == GameManager.Level.Level2)
            return "LVL 2";

        else if (GM.CurrentLevel == GameManager.Level.Level3)
            return "LVL 3";

        else if (GM.CurrentLevel == GameManager.Level.Level4)
            return "LVL 4";

        else if (GM.CurrentLevel == GameManager.Level.Level5)
            return "LVL 5";

        else if (GM.CurrentLevel == GameManager.Level.Level6)
            return "LVL 6";

        else if (GM.CurrentLevel == GameManager.Level.Level7)
            return "LVL 7";

        else if (GM.CurrentLevel == GameManager.Level.Level8)
            return "LVL 8";

        else if (GM.CurrentLevel == GameManager.Level.Level9)
            return "LVL 9";

        else
            return "LVL 10";
    }


    public void UpdateHUDNumbers(float mass)
    {
        scoreText.text = GM.Score.ToString("N0");
        UpdateLevelBar();
        spawnedNumbers.AddNumToQueue((int)mass);
    }
    private void UpdateLevelBar()
    {
        UpdateObjectsToAbsorbText();

        levelBar.fillAmount = (float)GM.NumOfObjectsAbsorbed / (float)RequiredObjectAmountToLevelUp();
        TriggerMassMeterAnimation(levelBarAnimator, GAIN_TRIGGER);
    }

    private int RequiredObjectAmountToLevelUp()
    {
        if (GM.CurrentLevel == GameManager.Level.Level1)
            return GM.Level2ObjectCount;

        else if (GM.CurrentLevel == GameManager.Level.Level2)
            return GM.Level3ObjectCount;

        else if (GM.CurrentLevel == GameManager.Level.Level3)
            return GM.Level4ObjectCount;

        else if (GM.CurrentLevel == GameManager.Level.Level4)
            return GM.Level5ObjectCount;

        else if (GM.CurrentLevel == GameManager.Level.Level5)
            return GM.Level6ObjectCount;

        else if (GM.CurrentLevel == GameManager.Level.Level6)
            return GM.Level7ObjectCount;

        else if (GM.CurrentLevel == GameManager.Level.Level7)
            return GM.Level8ObjectCount;

        else if (GM.CurrentLevel == GameManager.Level.Level8)
            return GM.Level9ObjectCount;

        else if (GM.CurrentLevel == GameManager.Level.Level9)
            return GM.Level10ObjectCount;

        else
            return GM.Level10ObjectCount;
    }

    private void LevelUp()
    {
        levelBar.fillAmount = 0;
        levelText.text = LevelText();
        TriggerMassMeterAnimation(levelTextAnimator, LEVEL_UP_TRIGGER);
        UpdateObjectsToAbsorbText();

        if (GM.CurrentLevel == GameManager.Level.Level5 || GM.CurrentLevel == GameManager.Level.Level7)
        {
            elapsedTimeColorLerp = 0;
            previousIconAndBarColor = galaxyIcon.color;
            currentIconAndBarColor = galaxyIcon.color;
            targetIconAndBarColor = TargetColor();

            if (!player.inBoostMode)
                isColorLerping = true;
        }
    }

    private Color TargetColor()
    {
        if (GM.CurrentLevel == GameManager.Level.Level5)
            return purpleColor;

        else if (GM.CurrentLevel == GameManager.Level.Level7)
            return blueColor;

        else
            return purpleColor;
    }

    private void RoomCompleted()
    {
        isTimerActive = false;
        StopCurrentCoroutine();

        GM.InvokeCutsceneStarted();
        isFadingOut = true;
        timerAnimator.enabled = true;
        timerAnimator.SetBool("victory", true);

        currentCoroutine = BackToMainMenuCoroutine();
        StartCoroutine(currentCoroutine);
    }

    private void UpdateObjectsToAbsorbText()
    {
        stringBuilder.Clear();
        stringBuilder.Append(GM.NumOfObjectsAbsorbed.ToString("N0"));
        stringBuilder.Append(SLASH);
        stringBuilder.Append(RequiredObjectAmountToLevelUp().ToString("N0"));
        objectsToAbsorbText.text = stringBuilder.ToString();
    }

    private void TriggerMassMeterAnimation(Animator _animator, string _trigger)
    {
        if (_animator.enabled)
            _animator.SetTrigger(_trigger);

        else
            _animator.enabled = true;
    }

    public void UIBoostVFX()
    {
        isColorLerping = false;
        boostAnimator.SetBool(END_BOOL, false);

        if (boostAnimator.gameObject.activeInHierarchy)
            boostAnimator.SetTrigger(BOOST_TRIGGER);

        else
            boostAnimator.gameObject.SetActive(true);

        StopCurrentCoroutine();
        currentCoroutine = ChangeToColorOverrideCoroutine(boostColor);
        StartCoroutine(currentCoroutine);
    }
    public void EndUIBoostVFX()
    {
        boostAnimator.SetBool(END_BOOL, true);
        StopCurrentCoroutine();
        currentCoroutine = ChangeToColorOverrideCoroutine(targetIconAndBarColor);
        StartCoroutine(currentCoroutine);
    }
    
    private IEnumerator ChangeToColorOverrideCoroutine(Color _targetColor)
    {
        Color currentColor;
        Color cachedColor = galaxyIcon.color;
        float _elaspedTime = 0;

        while (_elaspedTime < iconAndBarcolorLerpTime)
        {
            _elaspedTime += Time.deltaTime;
            currentColor = Color.Lerp(cachedColor, _targetColor, _elaspedTime / iconAndBarcolorLerpTime);
            galaxyIcon.color = currentColor;
            levelBar.color = currentColor;

            yield return null;
        }

        galaxyIcon.color = _targetColor;
        levelBar.color = _targetColor;
    }
}
