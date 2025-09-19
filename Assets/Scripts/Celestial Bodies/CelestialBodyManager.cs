using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using static CelestialBodySettings; 

public class CelestialBodyManager : MonoBehaviour
{    
    private GameManager GM;
    private SFXBank SFXBank;
    private HUDController HUD;
    private PlayerController playerLogic;
    private Transform playerTrans;
    private Transform cloneTrans;

    [Header("Main References")]
    [Space]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private GameObject player;
    [SerializeField] private SplitClone clone;

    private VCHelper vcHelper;
    private bool overrideSpawning;

    [Space]

    [SerializeField] private Transform roomWestBound;
    [SerializeField] private Transform roomEastBound;
    [SerializeField] private Transform roomNorthBound;
    [SerializeField] private Transform roomSouthBound;
    [SerializeField] private Transform spawnBoxWestBound;
    [SerializeField] private Transform spawnBoxEastBound;
    [SerializeField] private Transform spawnBoxNorthBound;
    [SerializeField] private Transform spawnBoxSouthBound;
    
    [SerializeField] private CelestialBody[] initialCelestialBodies;
    
    [Space]

    [SerializeField] private CelestialBodySettings tier1Settings;
    [SerializeField] private CelestialBodySettings tier2Settings;
    [SerializeField] private CelestialBodySettings tier3Settings;
    [SerializeField] private CelestialBodySettings tier4Settings;
    [field: SerializeField] public AnimationCurve GoToCloneCurve { get; private set; }

    [field: Header("Spawn Settings")]
    [field: Space]
    
    [SerializeField] private float spawnInterval;

    [Space]

    [SerializeField] private AmountOfBodiesPerLevel amountOfBodiesLevel1;
    [SerializeField] private AmountOfBodiesPerLevel amountOfBodiesLevel2;
    [SerializeField] private AmountOfBodiesPerLevel amountOfBodiesLevel3;
    [SerializeField] private AmountOfBodiesPerLevel amountOfBodiesLevel4;
    [SerializeField] private AmountOfBodiesPerLevel amountOfBodiesLevel5;
    [SerializeField] private AmountOfBodiesPerLevel amountOfBodiesLevel6;
    [SerializeField] private AmountOfBodiesPerLevel amountOfBodiesLevel7;
    [SerializeField] private AmountOfBodiesPerLevel amountOfBodiesLevel8;
    [SerializeField] private AmountOfBodiesPerLevel amountOfBodiesLevel9;

    private float spawnTimer;

    private List<CelestialBody> orbitPlayerBodies = new List<CelestialBody>();
    private List<CelestialBody> orbitCloneBodies = new List<CelestialBody>();
    private List<CelestialBody> orbitOtherBodiesBodies = new List<CelestialBody>();
    private List<CelestialBody> activeTier1Set = new List<CelestialBody>();
    private List<CelestialBody> activeTier2Set = new List<CelestialBody>();
    private List<CelestialBody> activeTier3Set = new List<CelestialBody>();
    private List<CelestialBody> activeTier4Set = new List<CelestialBody>();
    private List<CelestialBody> tier1Set = new List<CelestialBody>();
    private List<CelestialBody> tier2Set = new List<CelestialBody>();
    private List<CelestialBody> tier3Set = new List<CelestialBody>();
    private List<CelestialBody> tier4Set = new List<CelestialBody>();

    private float tier1AbsorbSFXTimer;
    private float tier2AbsorbSFXTimer;
    private float tier3AbsorbSFXTimer;
    private float tier4AbsorbSFXTimer;
    
    void Awake()
    {
        vcHelper = virtualCamera.GetComponent<VCHelper>();
        spawnTimer = spawnInterval;
        InitializeListsAndCelestialBodies();
        playerTrans = player.transform;
        cloneTrans = clone.transform;

        if (!player.activeInHierarchy)
        {
            overrideSpawning = true;

            for (int i = 0; i < initialCelestialBodies.Length; i++)
            {
                if (initialCelestialBodies[i].Type == CelestialBodyType.Tier1)
                    activeTier1Set.Remove(initialCelestialBodies[i]);

                else if (initialCelestialBodies[i].Type == CelestialBodyType.Tier2)
                    activeTier2Set.Remove(initialCelestialBodies[i]);

                else if (initialCelestialBodies[i].Type == CelestialBodyType.Tier3)
                    activeTier3Set.Remove(initialCelestialBodies[i]);

                else if (initialCelestialBodies[i].Type == CelestialBodyType.Tier4)
                    activeTier4Set.Remove(initialCelestialBodies[i]);

                initialCelestialBodies[i].gameObject.SetActive(false);
            }
        }
    }   

    void Start()
    {
        HUD = HUDController.Instance;
        GM = GameManager.Instance;
        playerLogic = PlayerController.Instance;
        GM.OnCutsceneStarted += OnCutsceneStarted;
        GM.OnCutsceneEnded += OnCutsceneEnded;
        SFXBank = GM.AudioManager.SFXBank;
        SFXBank.LoadCelestialBodies();
    }

    private void CacheHUDIfValid()
    {
        if (HUD == null)
            HUD = HUDController.Instance;
    }

    void OnDisable()
    {
        if (GM != null)
        {
            GM.OnCutsceneStarted -= OnCutsceneStarted;
            GM.OnCutsceneEnded -= OnCutsceneEnded;
        }
    }

    private void InitializeListsAndCelestialBodies()
    {
        Vector2 spawnOffscreenPos = new Vector2(600, 600);

        for (int i = 0; i < initialCelestialBodies.Length; i++)
        {
            initialCelestialBodies[i].Manager = this;
            
            if (initialCelestialBodies[i].Type == CelestialBodyType.Tier1)
            {
                tier1Set.Add(initialCelestialBodies[i]);
                activeTier1Set.Add(initialCelestialBodies[i]);
                initialCelestialBodies[i].InitialBoost(tier1Settings);
            }
            else if (initialCelestialBodies[i].Type == CelestialBodyType.Tier2)
            {
                tier2Set.Add(initialCelestialBodies[i]);
                activeTier2Set.Add(initialCelestialBodies[i]);
                initialCelestialBodies[i].InitialBoost(tier2Settings);
            }
            else if (initialCelestialBodies[i].Type == CelestialBodyType.Tier3)
            {
                tier3Set.Add(initialCelestialBodies[i]);
                activeTier3Set.Add(initialCelestialBodies[i]);
                initialCelestialBodies[i].InitialBoost(tier3Settings);
            }
            else if (initialCelestialBodies[i].Type == CelestialBodyType.Tier4)
            {
                tier4Set.Add(initialCelestialBodies[i]);
                activeTier4Set.Add(initialCelestialBodies[i]);
                initialCelestialBodies[i].InitialBoost(tier4Settings);
            }
        }

        for (int i = 0; i < amountOfBodiesLevel9.MaxNumOfBodiesToSpawn; i++)
        {
            GameObject tier1Object = Instantiate(tier1Settings.GetCelestialBodyPrefab(), spawnOffscreenPos, Quaternion.identity, transform);
            GameObject tier2Object = Instantiate(tier2Settings.GetCelestialBodyPrefab(), spawnOffscreenPos, Quaternion.identity, transform);
            GameObject tier3Object = Instantiate(tier3Settings.GetCelestialBodyPrefab(), spawnOffscreenPos, Quaternion.identity, transform);
            GameObject tier4Object = Instantiate(tier4Settings.GetCelestialBodyPrefab(), spawnOffscreenPos, Quaternion.identity, transform);

            CelestialBody tier1Logic = tier1Object.GetComponent<CelestialBody>();
            CelestialBody tier2Logic = tier2Object.GetComponent<CelestialBody>();
            CelestialBody tier3Logic = tier3Object.GetComponent<CelestialBody>();
            CelestialBody tier4Logic = tier4Object.GetComponent<CelestialBody>();

            tier1Set.Add(tier1Logic);
            tier2Set.Add(tier2Logic);
            tier3Set.Add(tier3Logic);
            tier4Set.Add(tier4Logic);

            tier1Object.SetActive(false);
            tier2Object.SetActive(false);
            tier3Object.SetActive(false);
            tier4Object.SetActive(false);
           
            tier1Logic.Manager = this;
            tier2Logic.Manager = this;
            tier3Logic.Manager = this;
            tier4Logic.Manager = this;
        }
    }

    void FixedUpdate()
    {
        if (GM.CutscenePlaying)
            return;

        if (orbitPlayerBodies.Count > 0)
        {
            for (int i = 0; i < orbitPlayerBodies.Count; i++)
                ManageMovementToPlayer(orbitPlayerBodies[i], GetSettings(orbitPlayerBodies[i].Type));
        }

        if (orbitOtherBodiesBodies.Count > 0)
        {
            for (int i = 0; i < orbitOtherBodiesBodies.Count; i++)
                orbitOtherBodiesBodies[i].MoveToOtherCelestialBody(GetSettings(orbitOtherBodiesBodies[i].Type));
        }
    }
    void Update()
    {
        if (CanSpawn())
        {
            spawnTimer -= Time.deltaTime;

            if (spawnTimer < 0)
            {
                SpawnCelestialBody();
                spawnTimer = spawnInterval;
            }
        }

        HandleAbsorbSFXTimers();

        if (orbitCloneBodies.Count > 0)
        {
            for (int i = 0; i < orbitCloneBodies.Count; i++)
                orbitCloneBodies[i].MoveToClone();
        }
    }

    private CelestialBodySettings GetSettings(CelestialBodyType _type)
    {
        if (_type == CelestialBodyType.Tier1)
            return tier1Settings;

        else if (_type == CelestialBodyType.Tier2)
            return tier2Settings;

        else if (_type == CelestialBodyType.Tier3)
            return tier3Settings;

        else
            return tier4Settings;
    }

    private void ManageMovementToPlayer(CelestialBody celestialBody, CelestialBodySettings settings)
    {
        celestialBody.MoveToPlayer(settings);
        celestialBody.PlotEventHorizonPath(settings);
        celestialBody.AnimateLine(settings);
    }

    private bool CanSpawn()
    {
        return (activeTier1Set.Count + activeTier2Set.Count + activeTier3Set.Count + activeTier4Set.Count) < MaxOfBodiesToSpawn()
            && !overrideSpawning
            && !GM.CutscenePlaying;
    }

    private int MaxOfBodiesToSpawn()
    {
        if (GM.CurrentLevel == GameManager.Level.Level1)
            return amountOfBodiesLevel1.MaxNumOfBodiesToSpawn;
        
        else if (GM.CurrentLevel == GameManager.Level.Level2)
            return amountOfBodiesLevel2.MaxNumOfBodiesToSpawn;
        
        else if (GM.CurrentLevel == GameManager.Level.Level3)
            return amountOfBodiesLevel3.MaxNumOfBodiesToSpawn;
        
        else if (GM.CurrentLevel == GameManager.Level.Level4)
            return amountOfBodiesLevel4.MaxNumOfBodiesToSpawn;
        
        else if (GM.CurrentLevel == GameManager.Level.Level5)
            return amountOfBodiesLevel5.MaxNumOfBodiesToSpawn;
        
        else if (GM.CurrentLevel == GameManager.Level.Level6)
            return amountOfBodiesLevel6.MaxNumOfBodiesToSpawn;
        
        else if (GM.CurrentLevel == GameManager.Level.Level7)
            return amountOfBodiesLevel7.MaxNumOfBodiesToSpawn;
        
        else if (GM.CurrentLevel == GameManager.Level.Level8)
            return amountOfBodiesLevel8.MaxNumOfBodiesToSpawn;
        
        else if (GM.CurrentLevel == GameManager.Level.Level9)
            return amountOfBodiesLevel9.MaxNumOfBodiesToSpawn;
        
        else
            return amountOfBodiesLevel9.MaxNumOfBodiesToSpawn;;
    }

    private AmountOfBodiesPerLevel LevelSpecificCelestialBodySettings()
    {
        if (GM.CurrentLevel == GameManager.Level.Level1)
            return amountOfBodiesLevel1;
        
        else if (GM.CurrentLevel == GameManager.Level.Level2)
            return amountOfBodiesLevel2;
        
        else if (GM.CurrentLevel == GameManager.Level.Level3)
            return amountOfBodiesLevel3;
        
        else if (GM.CurrentLevel == GameManager.Level.Level4)
            return amountOfBodiesLevel4;
        
        else if (GM.CurrentLevel == GameManager.Level.Level5)
            return amountOfBodiesLevel5;
        
        else if (GM.CurrentLevel == GameManager.Level.Level6)
            return amountOfBodiesLevel6;
        
        else if (GM.CurrentLevel == GameManager.Level.Level7)
            return amountOfBodiesLevel7;
        
        else if (GM.CurrentLevel == GameManager.Level.Level8)
            return amountOfBodiesLevel8;
        
        else if (GM.CurrentLevel == GameManager.Level.Level9)
            return amountOfBodiesLevel9;
        
        else
            return amountOfBodiesLevel9;
    }

    private void HandleAbsorbSFXTimers()
    {
        tier1AbsorbSFXTimer -= Time.deltaTime;
        tier2AbsorbSFXTimer -= Time.deltaTime;
        tier3AbsorbSFXTimer -= Time.deltaTime;
        tier4AbsorbSFXTimer -= Time.deltaTime;
    }

    private void ManageEnablingOrbitingColliderOfOtherBodies(CelestialBody _celestialBody)
    {
        if (_celestialBody.Type != CelestialBodyType.Tier3)
        {
            if (_celestialBody.CelestialBodyFinderCollider != null)
            {
                _celestialBody.CelestialBodyFinderCollider.SetActive(true);
                return;
            }
        }
        else
        {
            if (GM.CurrentLevel == GameManager.Level.Level7
            || GM.CurrentLevel == GameManager.Level.Level8
            || GM.CurrentLevel == GameManager.Level.Level9
            || GM.CurrentLevel == GameManager.Level.Level10)
            {
                if (_celestialBody.CelestialBodyFinderCollider != null)
                    _celestialBody.CelestialBodyFinderCollider.SetActive(false);
            }
            else
            {

                if (_celestialBody.CelestialBodyFinderCollider != null)
                    _celestialBody.CelestialBodyFinderCollider.SetActive(true);
            }
        }
    }

    private void SpawnCelestialBody()
    {
        CelestialBody celestialBody = CelestialBody();

        ManageEnablingOrbitingColliderOfOtherBodies(celestialBody);

        celestialBody.transform.position = GetRandomValidPosition(celestialBody);
        celestialBody.gameObject.SetActive(true);
        celestialBody.BlockAndResetMovementVars();

        if (celestialBody.Type == CelestialBodyType.Tier1)
        {
            activeTier1Set.Add(celestialBody);
            celestialBody.InitialBoost(tier1Settings);
        }
        else if (celestialBody.Type == CelestialBodyType.Tier2)
        {
            activeTier2Set.Add(celestialBody);
            celestialBody.InitialBoost(tier2Settings);
        }
        else if (celestialBody.Type == CelestialBodyType.Tier3)
        {
            activeTier3Set.Add(celestialBody);
            celestialBody.InitialBoost(tier3Settings);
        }
        else if (celestialBody.Type == CelestialBodyType.Tier4)
        {
            activeTier4Set.Add(celestialBody);
            celestialBody.InitialBoost(tier4Settings);
        }


        StartCoroutine(DelayReactivatingOrbitingBehavior(celestialBody));
    }

    private IEnumerator DelayReactivatingOrbitingBehavior(CelestialBody _celestialBody)
    {
        yield return null;
        
        RemoveCelestialBodyOrbitingClone(_celestialBody);
        RemoveCelestialBodyOrbitingOtherBody(_celestialBody);
        RemoveCelestialBodyOrbitingPlayer(_celestialBody);

        if (_celestialBody.gameObject.activeInHierarchy)
            _celestialBody.UnblockMovementVars();
    }


    public void PerformAbsorbBehavior(CelestialBodyType _type, CodexEntry _entryData, bool playSFX)
    {
        playerLogic.AddMass(GetMass(_type));

        CacheHUDIfValid();

        if (playSFX)
            PlayAbsorbSFX(_type);

        if (_entryData != null)
            HUD.CheckCodexEntry(_entryData);
    }

    private void PlayAbsorbSFX(CelestialBodyType _type)
    {
        AmountOfBodiesPerLevel levelSpecificCelestialBodySettings = LevelSpecificCelestialBodySettings();

        if (_type == CelestialBodyType.Tier1 && tier1AbsorbSFXTimer <= 0)
        {
            tier1AbsorbSFXTimer = tier1Settings.TimeBetweenAbsorbSFXCalls;
            BHBAudio.PlayOneShotAttachedFromLoadedSampleData(SFXBank.GasDescription, player, levelSpecificCelestialBodySettings.VolumeOfAbsorbSoundsPerLevel.x);
        }
        else if (_type == CelestialBodyType.Tier2 && tier2AbsorbSFXTimer <= 0)
        {
            tier2AbsorbSFXTimer = tier2Settings.TimeBetweenAbsorbSFXCalls;
            BHBAudio.PlayOneShotAttachedFromLoadedSampleData(SFXBank.AsteroidDescription, player, levelSpecificCelestialBodySettings.VolumeOfAbsorbSoundsPerLevel.y);
        }
        else if (_type == CelestialBodyType.Tier3 && tier3AbsorbSFXTimer <= 0)
        {
            tier3AbsorbSFXTimer = tier3Settings.TimeBetweenAbsorbSFXCalls;
            BHBAudio.PlayOneShotAttachedFromLoadedSampleData(SFXBank.PlanetDescription, player, levelSpecificCelestialBodySettings.VolumeOfAbsorbSoundsPerLevel.z);
        }
        else if (_type == CelestialBodyType.Tier4 && tier4AbsorbSFXTimer <= 0)
        {
            tier4AbsorbSFXTimer = tier4Settings.TimeBetweenAbsorbSFXCalls;
            BHBAudio.PlayOneShotAttachedFromLoadedSampleData(SFXBank.StarDescription, player, levelSpecificCelestialBodySettings.VolumeOfAbsorbSoundsPerLevel.w);
        }
    }

    public int GetMass(CelestialBodyType _type)
    {
        if (_type == CelestialBodyType.Tier1)
            return tier1Settings.Mass;

        else if (_type == CelestialBodyType.Tier2)
            return tier2Settings.Mass;

        else if (_type == CelestialBodyType.Tier3)
            return tier3Settings.Mass;

        else
            return tier4Settings.Mass;
    }

    public void MoveCelestialBodyAroundPlayer(CelestialBody celestialBody)
    {
        if (!celestialBody.OrbitingPlayer)
            orbitPlayerBodies.Add(celestialBody);
    }
    public void MoveCelestialBodyAroundClone(CelestialBody celestialBody)
    {
        if (!celestialBody.OrbitingClone)
            orbitCloneBodies.Add(celestialBody);
    }
    public void MoveCelestialBodyAroundOtherBody(CelestialBody celestialBody)
    {
        if (!celestialBody.OrbitingOtherBody)
            orbitOtherBodiesBodies.Add(celestialBody);
    }

    public void RemoveCelestialBodyOrbitingOtherBody(CelestialBody celestialBody)
    {
        if (celestialBody.OrbitingOtherBody)
            orbitOtherBodiesBodies.Remove(celestialBody);
    }
    public void RemoveCelestialBodyOrbitingPlayer(CelestialBody celestialBody)
    {
        if (celestialBody.OrbitingPlayer)
            orbitPlayerBodies.Remove(celestialBody);
    }
    public void RemoveCelestialBodyOrbitingClone(CelestialBody celestialBody)
    {
        if (celestialBody.OrbitingClone)
            orbitCloneBodies.Remove(celestialBody);
    }

    public void RemoveCelestialBodyCompletelyFromActiveLists(CelestialBody celestialBody)
    {
        if (celestialBody.Type == CelestialBodyType.Tier1 && activeTier1Set.Contains(celestialBody))
            activeTier1Set.Remove(celestialBody);

        else if (celestialBody.Type == CelestialBodyType.Tier2 && activeTier2Set.Contains(celestialBody))
            activeTier2Set.Remove(celestialBody);

        else if (celestialBody.Type == CelestialBodyType.Tier3 && activeTier3Set.Contains(celestialBody))
            activeTier3Set.Remove(celestialBody);

        else if (celestialBody.Type == CelestialBodyType.Tier4 && activeTier4Set.Contains(celestialBody))
            activeTier4Set.Remove(celestialBody);

        RemoveCelestialBodyOrbitingPlayer(celestialBody);
        RemoveCelestialBodyOrbitingClone(celestialBody);
        RemoveCelestialBodyOrbitingOtherBody(celestialBody);
    }

    public float NewOrbitMultiplier(CelestialBodyType _type)
    {
        if (_type == CelestialBodyType.Tier1)
            return tier1Settings.NewOrbitMultiplier(GM);
        
        else if (_type == CelestialBodyType.Tier2)
            return tier2Settings.NewOrbitMultiplier(GM);
        
        else if (_type == CelestialBodyType.Tier3)
            return tier3Settings.NewOrbitMultiplier(GM);
        
        else
            return tier4Settings.NewOrbitMultiplier(GM);
    }
    public float GetOrbitRadiusForOtherCelestialBodies(CelestialBodyType _type)
    {
        if (_type == CelestialBodyType.Tier1)
            return tier1Settings.OrbitRadiusForOtherCelestialBodies;
        
        else if (_type == CelestialBodyType.Tier2)
            return tier2Settings.OrbitRadiusForOtherCelestialBodies;
        
        else if (_type == CelestialBodyType.Tier3)
            return tier3Settings.OrbitRadiusForOtherCelestialBodies;
        
        else
            return tier4Settings.OrbitRadiusForOtherCelestialBodies;
    }
    public float TravelSpeed(CelestialBodyType _type)
    {
        if (_type == CelestialBodyType.Tier1)
            return tier1Settings.SpeedToPlayer;

        else if (_type == CelestialBodyType.Tier2)
            return tier2Settings.SpeedToPlayer;

        else if (_type == CelestialBodyType.Tier3)
            return tier3Settings.SpeedToPlayer;

        else
            return tier4Settings.SpeedToPlayer;
    }
    public float GetMinOrbitMultiplier(CelestialBodyType _type)
    {
        if (_type == CelestialBodyType.Tier1)
            return tier1Settings.GetMinOrbitMultiplier(GM);
        
        else if (_type == CelestialBodyType.Tier2)
            return tier2Settings.GetMinOrbitMultiplier(GM);
        
        else if (_type == CelestialBodyType.Tier3)
            return tier3Settings.GetMinOrbitMultiplier(GM);
        
        else
            return tier4Settings.GetMinOrbitMultiplier(GM);
    }
    public float OrbitDeclineRate(CelestialBodyType _type)
    {
        if (_type == CelestialBodyType.Tier1)
            return tier1Settings.OrbitDeclineRate;
        
        else if (_type == CelestialBodyType.Tier2)
            return tier2Settings.OrbitDeclineRate;
        
        else if (_type == CelestialBodyType.Tier3)
            return tier3Settings.OrbitDeclineRate;
        
        else
            return tier4Settings.OrbitDeclineRate;
    }

    private CelestialBodyType GetRandomCelestialBodyTypeByPercentage(Vector4 percentages)
    {
        // Calculate total percentage to normalize values
        float totalPercentage = percentages.x + percentages.y + percentages.z + percentages.w;
        float randomValue = Random.Range(0f, totalPercentage);
        float cumulativePercentage = 0f;
        
        cumulativePercentage += percentages.x;
        if (randomValue <= cumulativePercentage)
            return CelestialBodyType.Tier1;
        
        cumulativePercentage += percentages.y;
        if (randomValue <= cumulativePercentage)
            return CelestialBodyType.Tier2;

        cumulativePercentage += percentages.z;
        if (randomValue <= cumulativePercentage)
            return CelestialBodyType.Tier3;

        cumulativePercentage += percentages.w;
        if (randomValue <= cumulativePercentage)
            return CelestialBodyType.Tier4;
        
        return CelestialBodyType.Tier2;
    }

    private CelestialBodyType CelestialBodyTypeToSpawn()
    {
        if (GM.CurrentLevel == GameManager.Level.Level1)
            return GetRandomCelestialBodyTypeByPercentage(amountOfBodiesLevel1.PercentageOfBodiesToSpawn);
        
        else if (GM.CurrentLevel == GameManager.Level.Level2)
            return GetRandomCelestialBodyTypeByPercentage(amountOfBodiesLevel2.PercentageOfBodiesToSpawn);

        else if (GM.CurrentLevel == GameManager.Level.Level3)
            return GetRandomCelestialBodyTypeByPercentage(amountOfBodiesLevel3.PercentageOfBodiesToSpawn);

        else if (GM.CurrentLevel == GameManager.Level.Level4)
            return GetRandomCelestialBodyTypeByPercentage(amountOfBodiesLevel4.PercentageOfBodiesToSpawn);

        else if (GM.CurrentLevel == GameManager.Level.Level5)
            return GetRandomCelestialBodyTypeByPercentage(amountOfBodiesLevel5.PercentageOfBodiesToSpawn);

        else if (GM.CurrentLevel == GameManager.Level.Level6)
            return GetRandomCelestialBodyTypeByPercentage(amountOfBodiesLevel6.PercentageOfBodiesToSpawn);

        else if (GM.CurrentLevel == GameManager.Level.Level7)
            return GetRandomCelestialBodyTypeByPercentage(amountOfBodiesLevel7.PercentageOfBodiesToSpawn);

        else if (GM.CurrentLevel == GameManager.Level.Level8)
            return GetRandomCelestialBodyTypeByPercentage(amountOfBodiesLevel8.PercentageOfBodiesToSpawn);

        else if (GM.CurrentLevel == GameManager.Level.Level9)
            return GetRandomCelestialBodyTypeByPercentage(amountOfBodiesLevel9.PercentageOfBodiesToSpawn);
        
        else if (GM.CurrentLevel == GameManager.Level.Level10)
            return CelestialBodyType.Tier4;

        else
            return CelestialBodyType.Tier2;
    }

    private CelestialBody CelestialBody()
    {
        CelestialBody celestialBody = null;
        CelestialBodyType celestialBodyType = CelestialBodyTypeToSpawn();

        if (celestialBodyType == CelestialBodyType.Tier1)
        {
            for (int i=0; i < tier1Set.Count; i++)
            {
                if (!tier1Set[i].gameObject.activeInHierarchy)
                {
                    celestialBody = tier1Set[i];
                    break;
                }
            }
        }
        else if (celestialBodyType == CelestialBodyType.Tier2)
        {
            for (int i=0; i < tier2Set.Count; i++)
            {
                if (!tier2Set[i].gameObject.activeInHierarchy)
                {
                    celestialBody = tier2Set[i];
                    break;
                }
            }
        }
        else if (celestialBodyType == CelestialBodyType.Tier3)
        {
            for (int i=0; i < tier3Set.Count; i++)
            {
                if (!tier3Set[i].gameObject.activeInHierarchy)
                {
                    celestialBody = tier3Set[i];
                    break;
                }
            }
        }
        else if (celestialBodyType == CelestialBodyType.Tier4)
        {
            for (int i=0; i < tier4Set.Count; i++)
            {
                if (!tier4Set[i].gameObject.activeInHierarchy)
                {
                    celestialBody = tier4Set[i];
                    break;
                }
            }
        }

        return celestialBody;
    }

    private Vector3 GetRandomValidPosition(CelestialBody _celestialBody)
    {
        Vector2 newPosition = new Vector2(Random.Range(spawnBoxWestBound.position.x, spawnBoxEastBound.position.x), Random.Range(spawnBoxSouthBound.position.y, spawnBoxNorthBound.position.y));
        bool isInCameraView = IsPositionInCameraView(newPosition, _celestialBody);
        bool isInRoom = IsPositionInRoom(newPosition);

        if (!isInCameraView && isInRoom)
            return newPosition;
        
        else
        {
            while (!(!isInCameraView && isInRoom))
            {
                newPosition = new Vector2(Random.Range(spawnBoxWestBound.position.x, spawnBoxEastBound.position.x), Random.Range(spawnBoxSouthBound.position.y, spawnBoxNorthBound.position.y));
                isInCameraView = IsPositionInCameraView(newPosition, _celestialBody);
                isInRoom = IsPositionInRoom(newPosition);
            }

            return newPosition;
        }
    }

    private bool IsPositionInRoom(Vector3 position)
    {
        return position.x > roomWestBound.position.x && position.x < roomEastBound.position.x && position.y > roomSouthBound.position.y && position.y < roomNorthBound.position.y;
    }

    private float MinDistanceFromCamera(CelestialBody _celestialBody)
    {
        if (_celestialBody.Type == CelestialBodyType.Tier1)
            return tier1Settings.MinSpawnDistanceFromCamera;
        
        else if (_celestialBody.Type == CelestialBodyType.Tier2)
            return tier2Settings.MinSpawnDistanceFromCamera;
        
        else if (_celestialBody.Type == CelestialBodyType.Tier3)
            return tier3Settings.MinSpawnDistanceFromCamera;
        
        else
            return tier4Settings.MinSpawnDistanceFromCamera;
    }
    
    private bool IsPositionInCameraView(Vector3 position, CelestialBody _celestialBody)
    {
        if (vcHelper == null)
            return false;

        VCHelperVectors vcVectors = vcHelper.GetVCVectorsAtRuntime();

        // Calculate camera view bounds using VCHelperVectors
        float cameraLeft = vcVectors.center.x - (vcVectors.bounds.x / 2f);
        float cameraRight = vcVectors.center.x + (vcVectors.bounds.x / 2f);
        float cameraBottom = vcVectors.center.y - (vcVectors.bounds.y / 2f);
        float cameraTop = vcVectors.center.y + (vcVectors.bounds.y / 2f);

        // Add padding for minimum distance
        float minDistanceFromCamera = MinDistanceFromCamera(_celestialBody);
        cameraLeft -= minDistanceFromCamera;
        cameraRight += minDistanceFromCamera;
        cameraBottom -= minDistanceFromCamera;
        cameraTop += minDistanceFromCamera;

        // Check if position is within extended camera bounds
        bool isInView = position.x >= cameraLeft && position.x <= cameraRight &&
                       position.y >= cameraBottom && position.y <= cameraTop;

        return isInView;
    }

    private void OnCutsceneStarted()
    {
        overrideSpawning = true;
    }

    public void SpawnInitialCelestialBodies()
    {
        for (int i = 0; i < initialCelestialBodies.Length; i++)
        {
            initialCelestialBodies[i].gameObject.SetActive(true);

            if (initialCelestialBodies[i].Type == CelestialBodyType.Tier1)
            {
                activeTier1Set.Add(initialCelestialBodies[i]);
                initialCelestialBodies[i].InitialBoost(tier1Settings);
            }
            else if (initialCelestialBodies[i].Type == CelestialBodyType.Tier2)
            {
                activeTier2Set.Add(initialCelestialBodies[i]);
                initialCelestialBodies[i].InitialBoost(tier2Settings);
            }
            else if (initialCelestialBodies[i].Type == CelestialBodyType.Tier3)
            {
                activeTier3Set.Add(initialCelestialBodies[i]);
                initialCelestialBodies[i].InitialBoost(tier3Settings);
            }
            else if (initialCelestialBodies[i].Type == CelestialBodyType.Tier4)
            {
                activeTier4Set.Add(initialCelestialBodies[i]);
                initialCelestialBodies[i].InitialBoost(tier4Settings);
            }
        }
    }

    private void OnCutsceneEnded()
    {
        overrideSpawning = false;
    }
}

[System.Serializable]
public struct AmountOfBodiesPerLevel
{
    public Vector4 PercentageOfBodiesToSpawn; //x = Tier 1, y = Tier2, z = Tier 3, w = Tier 4
    public int MaxNumOfBodiesToSpawn;
    public Vector4 VolumeOfAbsorbSoundsPerLevel; //x = Gas, y = Tier2, z = Tier 3, w = Tier 4
}
