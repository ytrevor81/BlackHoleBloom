using System.Collections;
using Cinemachine;
using FMODUnity;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private GameManager GM;
    private CinemachineFramingTransposer transposer;

    [SerializeField] private GameObject optionsPage;
    [SerializeField] private float menuFadeTime;
    private IEnumerator currentCoroutine;

    [Header("Real Game Startup")]
    [Space]

    [SerializeField] private CanvasGroup blackScreen;
    [SerializeField] private float fadeInSpeed;
    [SerializeField] private float timeBeforeFadeIn;

    private float currentBlackScreenAlpha;
    private bool fromRealGameStartup;
    private float timer;

    [Header("Cutscene")]
    [Space]

    [SerializeField] private CinemachineVirtualCamera mainVC;
    [SerializeField] private float targetZoom;
    [SerializeField] private float zoomInTime;
    [SerializeField] private float menuFadeOutSpeed;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject playerContainer;
    [SerializeField] private float fadeToPlayerStarSpeed;

    [Space]

    [SerializeField] private float zoomValueForFastEQFade;
    [SerializeField] private float musicEQFadeSpeedFar;
    [SerializeField] private float musicEQFadeSpeedClose;
    private float musicEQValue;
    private string musicEQParameter;
    private float currentZoom;
    private float currentMenuAlpha;


    [Header("Black Hole VFX")]
    [Space]
    [SerializeField] private GameObject mainMenuBlackHoleContainer;
    [SerializeField] private MainMenuBlackHole mainMenuBlackHole;

    public bool CanFadeOutMainMenu { get; set; }
    private float cachedZoom;
    private float elapsedTime;
    private bool canInteract = true;

    void Awake()
    {
        fromRealGameStartup = blackScreen.gameObject.activeInHierarchy;
        currentBlackScreenAlpha = blackScreen.alpha;
        timer = timeBeforeFadeIn;

        transposer = mainVC.GetCinemachineComponent<CinemachineFramingTransposer>();

        if (!fromRealGameStartup)
            mainMenuBlackHole.MainMenuChangeColor();
    }

    void Start()
    {
        GM = GameManager.Instance;
        musicEQParameter = GM.AudioManager.MusicBank.MusicEQParameter;

        musicEQValue = 1;
        RuntimeManager.StudioSystem.setParameterByName(musicEQParameter, musicEQValue);
        GM.AudioManager.PlayMusicTrackImmediately(AudioManager.MusicTrack.Nebula);

        currentZoom = transposer.m_CameraDistance;
    }

    private void StopCurrentCoroutine()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
    }

    private IEnumerator FadeInMenu(CanvasGroup _canvasGroup)
    {
        if (_canvasGroup == canvasGroup)
            canvasGroup.gameObject.SetActive(true);

        float currentAlpha = _canvasGroup.alpha;
        float elapsedTime = 0;

        while (elapsedTime < menuFadeTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            _canvasGroup.alpha = Mathf.Lerp(currentAlpha, 1, elapsedTime / menuFadeTime);
            yield return null;
        }

        _canvasGroup.alpha = 1;
        canInteract = true;
    }

    private IEnumerator FadeOutMenu(CanvasGroup _canvasGroup)
    {
        float currentAlpha = _canvasGroup.alpha;
        float elapsedTime = 0;

        while (elapsedTime < menuFadeTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            _canvasGroup.alpha = Mathf.Lerp(currentAlpha, 0, elapsedTime / menuFadeTime);
            yield return null;
        }
        _canvasGroup.alpha = 0;

        if (_canvasGroup == canvasGroup)
        {
            canvasGroup.gameObject.SetActive(false);
            optionsPage.SetActive(true);
        }
    }

    public void Play_Event()
    {
        if (GM.CutscenePlaying || !canInteract)
            return;

        StopCurrentCoroutine();
        canvasGroup.interactable = false;

        canInteract = false;
        currentMenuAlpha = 1f;
        cachedZoom = transposer.m_CameraDistance;
        currentZoom = transposer.m_CameraDistance;
        GM.InvokeCutsceneStarted();
    }

    public void Options_Event()
    {
        if (GM.CutscenePlaying || !canInteract)
            return;

        StopCurrentCoroutine();
        canInteract = false;

        currentCoroutine = FadeOutMenu(canvasGroup);
        StartCoroutine(currentCoroutine);
    }

    private float GetMusicEQFadeSpeed()
    {
        if (currentZoom < zoomValueForFastEQFade)
            return musicEQFadeSpeedClose;

        else
            return musicEQFadeSpeedFar;
    }

    private void ManageMusicEQFade()
    {
        if (musicEQValue == 0)
            return;

        musicEQValue -= Time.deltaTime * GetMusicEQFadeSpeed();

        if (musicEQValue <= 0)
        {
            musicEQValue = 0;
            RuntimeManager.StudioSystem.setParameterByName(musicEQParameter, musicEQValue);
        }
        else
            RuntimeManager.StudioSystem.setParameterByName(musicEQParameter, musicEQValue);
    }

    private void ManageCutscene()
    {
        if (CanFadeOutMainMenu)
        {
            if (currentBlackScreenAlpha <= 0)
            {
                gameObject.SetActive(false);
                return;
            }

            currentBlackScreenAlpha -= Time.deltaTime * fadeToPlayerStarSpeed;
            blackScreen.alpha = currentBlackScreenAlpha;
        }
        else if (currentZoom > targetZoom)
        {
            elapsedTime += Time.deltaTime;
            currentZoom = Mathf.Lerp(cachedZoom, targetZoom, elapsedTime / zoomInTime);
            transposer.m_CameraDistance = currentZoom;
            ManageMusicEQFade();

            if (currentZoom <= targetZoom)
            {
                //transposer.m_CameraDistance = targetZoom;
                blackScreen.gameObject.SetActive(true);
                blackScreen.alpha = 1;
                currentBlackScreenAlpha = 1f;
                playerContainer.SetActive(true);
                mainMenuBlackHoleContainer.SetActive(false);
            }
        }

        if (currentMenuAlpha > 0f)
        {
            currentMenuAlpha -= Time.deltaTime * menuFadeOutSpeed;
            canvasGroup.alpha = currentMenuAlpha;
        }
    }
    private void ManageRealGameStartup()
    {
        if (timer < 0)
        {
            if (currentBlackScreenAlpha <= 0)
            {
                fromRealGameStartup = false;
                blackScreen.gameObject.SetActive(false);
                mainMenuBlackHole.MainMenuChangeColor();
                return;
            }

            currentBlackScreenAlpha -= Time.deltaTime * fadeInSpeed;
            blackScreen.alpha = currentBlackScreenAlpha;
        }
        else
            timer -= Time.deltaTime;
    }

    void Update()
    {
        if (fromRealGameStartup)
            ManageRealGameStartup();

        else if (GM.CutscenePlaying)
            ManageCutscene();
    }
    
    public void BackFromOptionsPage()
    {
        StopCurrentCoroutine();

        currentCoroutine = FadeInMenu(canvasGroup);
        StartCoroutine(currentCoroutine);
    }
}
