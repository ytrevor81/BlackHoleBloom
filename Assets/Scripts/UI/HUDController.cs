using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public static HUDController Instance;
    private GameManager GM;
    private PlayerController player;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject conclusionScreen;

    [Header("TIMER")]
    [Space]

    [SerializeField] private TMP_Text timerText;

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

    private const string LEVEL_UP_TRIGGER = "LevelUp";
    private const string GAIN_TRIGGER = "Gain";
    private const string END_BOOL = "end";
    private const string BOOST_TRIGGER = "Boost";

    private bool isFadingIn;
    private bool isFadingOut;
    private float fadeSpeed = 1f;
    private IEnumerator currentCoroutine;

    void Awake()
    {
        Instance = this;
        galaxyIconTransform = galaxyIcon.transform;
        targetIconAndBarColor = galaxyIcon.color;
    }

    void Start()
    {
        GM = GameManager.Instance;
        player = PlayerController.Instance;
        levelBar.fillAmount = 0;
        GM.Timer = GM.RoomTime;
        isTimerActive = true;

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

        HandleGalaxyIconAndProgressBarVFX();
        HandFadeInOrOut();
    }

    private void HandFadeInOrOut()
    {
        if (isFadingIn)
        {
            canvasGroup.alpha += fadeSpeed * Time.deltaTime;

            if (canvasGroup.alpha >= 1)
            {
                canvasGroup.alpha = 1;
                isFadingIn = false;
            }
        }
        else if (isFadingOut)
        {
            canvasGroup.alpha -= fadeSpeed * Time.deltaTime;

            if (canvasGroup.alpha <= 0)
            {
                canvasGroup.alpha = 0;
                isFadingOut = false;
            }
        }
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

        if (GM.Timer > 0)
        {
            GM.Timer -= Time.deltaTime;

            timeSpan = TimeSpan.FromSeconds(GM.Timer);

            int minutes = timeSpan.Minutes;
            int seconds = timeSpan.Seconds;

            timerText.text = $"{minutes:D2}:{seconds:D2}";
        }
        else
        {
            isTimerActive = false;

            if (GM.CurrentLevel != GameManager.Level.Level10)
                conclusionScreen.SetActive(true);
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
        conclusionScreen.SetActive(true);
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
