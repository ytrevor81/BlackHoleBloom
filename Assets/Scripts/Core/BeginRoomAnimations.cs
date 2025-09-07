using System.Collections;
using Cinemachine;
using FMODUnity;
using UnityEngine;

public class BeginRoomAnimations : MonoBehaviour
{
    private GameManager GM;
    private CinemachineFramingTransposer transposer;
    [SerializeField] private CinemachineVirtualCamera mainCamera;
    [SerializeField] private float startCameraZoom;
    [SerializeField] private float toStarZoom;
    [SerializeField] private float finalCameraZoom;
    [SerializeField] private float cameraZoomSpeedToStar;
    [SerializeField] private float cameraZoomSpeed;

    [Space]

    [SerializeField] private GameObject[] nebulaBG;
    [SerializeField] private MainMenu mainMenu;
    [SerializeField] private CanvasGroup canvasGroupUI;
    [SerializeField] private CelestialBodyManager celestialBodyManager;
    [SerializeField] private float canvasGroupUIFadeSpeed;
    [SerializeField] private LevelUpAndGammaRay shockwaveLogic;
    [SerializeField] private Transform blackHole;
    [SerializeField] private Transform star;
    [SerializeField] private float rotationSpeed;

    [Space]

    [SerializeField] private float musicEQFadeSpeed;
    private float musicEQValue;
    private string musicEQParameter;

    private float rotZ;
    private Animator animator;
    private bool inSupernova;
    private bool fadeInUI;
    private WaitForSeconds cutsceneDelay = new WaitForSeconds(0.25f);
    private bool fastZoom;
    void Awake()
    {
        animator = GetComponent<Animator>();
        transposer = mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    IEnumerator Start()
    {
        GM = GameManager.Instance;
        GM.AudioManager.SFXBank.LoadPlayerSFX();
        musicEQParameter = GM.AudioManager.MusicBank.MusicEQParameter;

        yield return cutsceneDelay;

        if (GM.CutscenePlaying)
        {
            transposer.m_CameraDistance = startCameraZoom;
        }

        yield return cutsceneDelay;

        if (GM.CutscenePlaying)
        {
            for (int i = 0; i < nebulaBG.Length; i++)
            {
                nebulaBG[i].SetActive(true);
            }

            mainMenu.CanFadeOutMainMenu = true;
        }
    }

    private void ManageMusicEQFade()
    {
        if (musicEQValue < 1)
        {
            musicEQValue += Time.deltaTime * musicEQFadeSpeed;

            if (musicEQValue >= 1)
                musicEQValue = 1;

            RuntimeManager.StudioSystem.setParameterByName(musicEQParameter, musicEQValue);
        }
    }

    void Update()
    {
        if (!GM.CutscenePlaying)
        {
            this.enabled = false;
            return;
        }

        rotZ -= rotationSpeed * Time.deltaTime;
        star.rotation = Quaternion.Euler(0, 0, rotZ);

        if (!mainMenu.CanFadeOutMainMenu)
            return;

        ManageMusicEQFade();

        // if (!inSupernova)
        // {
        //     transposer.m_CameraDistance -= cameraZoomSpeed * Time.deltaTime;

        //     if (transposer.m_CameraDistance <= finalCameraZoom)
        //     {
        //         inSupernova = true;
        //         animator.enabled = true;
        //     }
        // }

        if (!inSupernova)
        {
            transposer.m_CameraDistance -= cameraZoomSpeedToStar * Time.deltaTime;

            if (transposer.m_CameraDistance <= toStarZoom)
            {
                inSupernova = true;
                animator.enabled = true;
            }
        }
        else if (fastZoom)
        {
            transposer.m_CameraDistance -= cameraZoomSpeed * Time.deltaTime;

            if (transposer.m_CameraDistance <= finalCameraZoom)
            {
                fastZoom = false;
                transposer.m_CameraDistance = finalCameraZoom;
            }
        }

        if (fadeInUI)
        {
            canvasGroupUI.alpha += canvasGroupUIFadeSpeed * Time.deltaTime;

            if (canvasGroupUI.alpha >= 1)
            {
                canvasGroupUI.alpha = 1;
                fadeInUI = false;
            }
        }
    }

    public void FastZoom_AnimationEvent()
    {
        fastZoom = true;
    }

    public void BlackHoleBirth_AniamtionEvent()
    {
        if (blackHole.gameObject.activeInHierarchy)
            return;

        blackHole.gameObject.SetActive(true);
        star.gameObject.SetActive(false);
        shockwaveLogic.ActivateLevelUpShockwave(5f);
        GM.HapticsManager.HeavyVibration();
        BHBAudio.PlayOneShotAttachedFromLoadedSampleData(GM.AudioManager.SFXBank.BeginGamePulseDescription, gameObject);
    }

    public void FadeInUI_AnimationEvent()
    {
        canvasGroupUI.gameObject.SetActive(true);
        fadeInUI = true;
    }
    public void SpawnGas_AnimationEvent()
    {
        celestialBodyManager.SpawnInitialCelestialBodies();
    }

    public void BeginGame_AnimationEvent()
    {
        animator.enabled = false;
        GM.InvokeCutsceneEnded();

        this.enabled = false;
    }
}
