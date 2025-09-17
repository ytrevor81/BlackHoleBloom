using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerController : MonoBehaviour
{
    private GameManager GM;
    private HUDController HUD;
    private SFXBank SFXBank;
    public static PlayerController Instance { get; private set; }
    private CinemachineFramingTransposer transposer;

    [field: Header("Main Refs")]
    [field: Space]

    [SerializeField] private float initialSpeed;
    [SerializeField] private VariableJoystick joystick;
    [SerializeField] private CinemachineVirtualCamera mainCamera;
    [SerializeField] private GameObject gravityArea;

    [Header("Level Increases")]
    [Space]

    //clone zoom level 1: 490

    [SerializeField] private LevelUpAndGammaRay levelUpAndGammaRay;
    [SerializeField] private LevelStats level2Stats;
    [SerializeField] private LevelStats level3Stats;
    [SerializeField] private LevelStats level4Stats;
    [SerializeField] private LevelStats level5Stats;
    [SerializeField] private LevelStats level6Stats;
    [SerializeField] private LevelStats level7Stats;
    [SerializeField] private LevelStats level8Stats;
    [SerializeField] private LevelStats level9Stats;

    [Space]
    [SerializeField] private float levelStatsLerpTime;
    [SerializeField] private AnimationCurve levelUpStatsCurve;
    private float elaspedTime;
    private bool updateSizeAndView;
    private float targetCameraView;
    private float targetCameraView_CloneActive;
    private float targetSparklesScale;
    private float targetScale;
    private float previousCameraDistance;
    private float speed;
    private Vector2 previousScale;
    private Vector2 previousSparklesScale;

    private float currentCameraDistance;
    private Vector2 currentScale;
    private Vector2 currentSparklesScale;

    [Header("Extra VFX")]
    [Space]

    [SerializeField] private GalaxyVFXController galaxyVFXController;
    [SerializeField] private Transform sparkles;

    [Header("Boost")]
    [Space]
    [SerializeField] private float boostSpeedTime;
    public float BoostMultiplier { get; private set; } = 1f;
    private float boostTimer;
    public bool inBoostMode { get; private set; }

    [Header("Abilities")]
    [Space]

    [SerializeField] private LightningAbility lightning;
    [field: SerializeField] public SplitController SplitController;

    private Rigidbody2D rb;
    private Vector2 currentVelocity;
    private List<CelestialBody> objectsInOrbit = new List<CelestialBody>();
    private HashSet<GameObject> objectsInOrbitGameObjects = new HashSet<GameObject>();

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        transposer = mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        speed = initialSpeed;
    }

    IEnumerator Start()
    {
        GM = GameManager.Instance;
        HUD = HUDController.Instance;

        SFXBank = GM.AudioManager.SFXBank;
        GM.OnLevelChanged += LevelUp;
        GM.OnCutsceneEnded += IntroCutsceneEnded;

        yield return null;

        gravityArea.SetActive(true);
    }

    private void CacheHUDIfNull()
    {
        if (HUD != null)
            return;

        HUD = HUDController.Instance;
    }

    void OnDisable()
    {
        if (GM != null)
        {
            GM.OnLevelChanged -= LevelUp;
            GM.OnCutsceneEnded -= IntroCutsceneEnded;
        }
    }

    void Update()
    {
        if (GM.CutscenePlaying)
            return;

        if (updateSizeAndView)
            UpdateScaleAndCameraSize();
    }

    private void FixedUpdate()
    {
        rb.velocity = Vector2.zero;

        if (GM.CutscenePlaying)
            return;

        Vector2 playerInputForce = CalculatePlayerInputForce();
        Vector2 predictedPosition = rb.position + playerInputForce * Time.fixedDeltaTime;
        rb.MovePosition(predictedPosition);
    }

    public float GetLevelCloneCameraDistanceValue()
    {
        if (GM.CurrentLevel == GameManager.Level.Level2)
            return level2Stats.CameraDistance_Clone;

        else if (GM.CurrentLevel == GameManager.Level.Level3)
            return level3Stats.CameraDistance_Clone;

        else if (GM.CurrentLevel == GameManager.Level.Level4)
            return level4Stats.CameraDistance_Clone;

        else if (GM.CurrentLevel == GameManager.Level.Level5)
            return level5Stats.CameraDistance_Clone;

        else if (GM.CurrentLevel == GameManager.Level.Level6)
            return level6Stats.CameraDistance_Clone;

        else if (GM.CurrentLevel == GameManager.Level.Level7)
            return level7Stats.CameraDistance_Clone;

        else if (GM.CurrentLevel == GameManager.Level.Level8)
            return level8Stats.CameraDistance_Clone;
            
        else if (GM.CurrentLevel == GameManager.Level.Level9 || GM.CurrentLevel == GameManager.Level.Level10)
            return level9Stats.CameraDistance_Clone;

        else
            return 490f;
    }

    private Vector2 CalculatePlayerInputForce()
    {
        if (inBoostMode)
        {
            boostTimer -= Time.deltaTime;

            if (boostTimer <= 1)
            {
                BoostMultiplier -= Time.fixedDeltaTime;

                if (BoostMultiplier <= 1)
                {
                    BoostMultiplier = 1;
                    galaxyVFXController.ChangeToPreviousColor();
                    HUD.EndUIBoostVFX();
                    inBoostMode = false;
                }
            }
        }

        return joystick.Direction * (speed * BoostMultiplier); ;
    }

    private float TargetCameraLerp()
    {
        if (SplitController.Active)
            return targetCameraView_CloneActive;
            
        else
            return targetCameraView;
    }

    private void UpdateScaleAndCameraSize()
    {
        elaspedTime += Time.deltaTime;

        float curveValue = levelUpStatsCurve.Evaluate(elaspedTime / levelStatsLerpTime);

        currentCameraDistance = Mathf.Lerp(previousCameraDistance, TargetCameraLerp(), curveValue);
        currentScale = Vector2.Lerp(previousScale, new Vector2(targetScale, targetScale), curveValue);
        currentSparklesScale = Vector2.Lerp(previousSparklesScale, new Vector2(targetSparklesScale, targetSparklesScale), curveValue);

        transposer.m_CameraDistance = currentCameraDistance;
        transform.localScale = currentScale;
        sparkles.localScale = currentSparklesScale;

        if (elaspedTime >= levelStatsLerpTime)
        {
            transposer.m_CameraDistance = TargetCameraLerp();
            transform.localScale = new Vector2(targetScale, targetScale);
            sparkles.localScale = new Vector2(targetSparklesScale, targetSparklesScale);
            updateSizeAndView = false;
        }
    }

    public void AddWhiteHoleMass(int mass, int numOfObjectsAbsorbed)
    {
        if (GM.CutscenePlaying)
            return;

        CacheHUDIfNull();
        GM.Mass += mass;
        GM.NumOfObjectsAbsorbed += numOfObjectsAbsorbed;
        HUD.UpdateHUDNumbers(mass);
        GM.ChangeLevelIfValid();
    }

    public void AddMass(int mass)
    {
        if (GM.CutscenePlaying)
            return;

        CacheHUDIfNull();
        GM.Mass += mass;
        GM.NumOfObjectsAbsorbed += 1;
        HUD.UpdateHUDNumbers(mass);
        GM.ChangeLevelIfValid();
    }

    public void EnterBoostMode()
    {
        if (!inBoostMode)
        {
            CacheHUDIfNull();
            galaxyVFXController.ChangeToBoostColor();
            HUD.UIBoostVFX();
        }

        boostTimer += boostSpeedTime;
        BoostMultiplier += 1;
        inBoostMode = true;
    }

    private void IncreaseStats()
    {
        elaspedTime = 0;

        previousCameraDistance = transposer.m_CameraDistance;
        previousScale = transform.localScale;
        previousSparklesScale = sparkles.localScale;

        if (GM.CurrentLevel == GameManager.Level.Level2)
        {
            targetScale = level2Stats.Scale;
            targetCameraView = level2Stats.CameraDistance;
            speed = level2Stats.Speed;
            targetSparklesScale = level2Stats.SparklesScale;
            targetCameraView_CloneActive = level2Stats.CameraDistance_Clone;
        }
        else if (GM.CurrentLevel == GameManager.Level.Level3)
        {
            targetScale = level3Stats.Scale;
            targetCameraView = level3Stats.CameraDistance;
            speed = level3Stats.Speed;
            targetSparklesScale = level3Stats.SparklesScale;
            targetCameraView_CloneActive = level3Stats.CameraDistance_Clone;
        }
        else if (GM.CurrentLevel == GameManager.Level.Level4)
        {
            targetScale = level4Stats.Scale;
            targetCameraView = level4Stats.CameraDistance;
            speed = level4Stats.Speed;
            targetSparklesScale = level4Stats.SparklesScale;
            targetCameraView_CloneActive = level4Stats.CameraDistance_Clone;
        }
        else if (GM.CurrentLevel == GameManager.Level.Level5)
        {
            targetScale = level5Stats.Scale;
            targetCameraView = level5Stats.CameraDistance;
            speed = level5Stats.Speed;
            targetSparklesScale = level5Stats.SparklesScale;
            targetCameraView_CloneActive = level5Stats.CameraDistance_Clone;

            sparkles.gameObject.SetActive(true);
        }
        else if (GM.CurrentLevel == GameManager.Level.Level6)
        {
            targetScale = level6Stats.Scale;
            targetCameraView = level6Stats.CameraDistance;
            speed = level6Stats.Speed;
            targetSparklesScale = level6Stats.SparklesScale;
            targetCameraView_CloneActive = level6Stats.CameraDistance_Clone;
        }
        else if (GM.CurrentLevel == GameManager.Level.Level7)
        {
            targetScale = level7Stats.Scale;
            targetCameraView = level7Stats.CameraDistance;
            speed = level7Stats.Speed;
            targetSparklesScale = level7Stats.SparklesScale;
            targetCameraView_CloneActive = level7Stats.CameraDistance_Clone;
        }
        else if (GM.CurrentLevel == GameManager.Level.Level8)
        {
            targetScale = level8Stats.Scale;
            targetCameraView = level8Stats.CameraDistance;
            speed = level8Stats.Speed;
            targetSparklesScale = level8Stats.SparklesScale;
            targetCameraView_CloneActive = level8Stats.CameraDistance_Clone;
        }
        else if (GM.CurrentLevel == GameManager.Level.Level9 || GM.CurrentLevel == GameManager.Level.Level10)
        {
            targetScale = level9Stats.Scale;
            targetCameraView = level9Stats.CameraDistance;
            speed = level9Stats.Speed;
            targetSparklesScale = level9Stats.SparklesScale;
            targetCameraView_CloneActive = level9Stats.CameraDistance_Clone;
        }

        updateSizeAndView = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(BHBConstants.BARRIER) && currentVelocity != Vector2.zero)
        {
            Vector2 collisionNormal = collision.contacts[0].normal;

            if (Mathf.Abs(collisionNormal.x) > Mathf.Abs(collisionNormal.y))
                currentVelocity.x = 0;

            else
                currentVelocity.y = 0;
        }
    }

    public void AddObjectToOrbitingList(Collider2D _collider)
    {
        if (objectsInOrbitGameObjects.Contains(_collider.gameObject))
            return;

        objectsInOrbitGameObjects.Add(_collider.gameObject);

        CelestialBody celestialBody = _collider.GetComponent<CelestialBody>();
        
        celestialBody.EnterOrbitOfPlayer(_targetOrbit: transform);
        objectsInOrbit.Add(celestialBody);
    }
    public void RemoveObjectFromOrbitingList(CelestialBody _celestialBody)
    {
        if (!objectsInOrbitGameObjects.Contains(_celestialBody.gameObject))
            return;

        objectsInOrbitGameObjects.Remove(_celestialBody.gameObject);
        objectsInOrbit.Remove(_celestialBody);
    }
    private void LevelUp()
    {
        IncreaseStats();
        levelUpAndGammaRay.ActivateLevelUpShockwave(GetCameraShakeValue());
        galaxyVFXController.UpgradeVFX();
        BHBAudio.PlayOneShotAttachedFromLoadedSampleData(SFXBank.LevelUpDescription, gameObject);
        GM.AudioManager.SetMusicProgress();
    }
    private void IntroCutsceneEnded()
    {
        gravityArea.SetActive(true);
    }

    private float GetCameraShakeValue()
    {
        if (GM.CurrentLevel == GameManager.Level.Level2)
            return level2Stats.CameraShakeValue;

        else if (GM.CurrentLevel == GameManager.Level.Level3)
            return level3Stats.CameraShakeValue;

        else if (GM.CurrentLevel == GameManager.Level.Level4)
            return level4Stats.CameraShakeValue;

        else if (GM.CurrentLevel == GameManager.Level.Level5)
            return level5Stats.CameraShakeValue;

        else if (GM.CurrentLevel == GameManager.Level.Level6)
            return level6Stats.CameraShakeValue;

        else if (GM.CurrentLevel == GameManager.Level.Level7)
            return level7Stats.CameraShakeValue;

        else if (GM.CurrentLevel == GameManager.Level.Level8)
            return level8Stats.CameraShakeValue;

        else if (GM.CurrentLevel == GameManager.Level.Level9 || GM.CurrentLevel == GameManager.Level.Level10)
            return level9Stats.CameraShakeValue;

        return level2Stats.CameraShakeValue;
    }

    public void ActivateLightningStrike_Editor()
    {
        lightning.transform.position = transform.position;
        lightning.gameObject.SetActive(true);
    }
}
[System.Serializable]
public struct LevelStats
{
    public float Scale;
    public float CameraDistance;
    public float Speed;
    public float CameraShakeValue;
    public float SparklesScale;
    public float CameraDistance_Clone;
}

#if UNITY_EDITOR

[CustomEditor(typeof(PlayerController))]
public class PlayerControllerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlayerController player = (PlayerController)target;

        // if (GUILayout.Button("Lightning Strike"))
        // {
        //     player.ActivateLightningStrike_Editor();
        // }
    }
}
#endif
