using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using static CelestialBodySettings; 

public class CelestialBodyManager : MonoBehaviour
{    
    private GameManager GM;
    private SFXBank SFXBank;
    private HUDController HUD;

    [Header("Main References")]
    [Space]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private GameObject player;

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

    [SerializeField] private CelestialBodySettings gasSettings;
    [SerializeField] private CelestialBodySettings asteroidSettings;
    [SerializeField] private CelestialBodySettings planetSettings;
    [SerializeField] private CelestialBodySettings starSettings;

    [Header("Spawn Settings")]
    [Space]
    
    [SerializeField] private float spawnInterval;
    [SerializeField] private float higherLevelSpawnInterval;
    [SerializeField] private float minDistanceFromCamera;

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

    private List<CelestialBody> moveableCelestialBodiesSet = new List<CelestialBody>();
    private List<CelestialBody> activeGasSet = new List<CelestialBody>();
    private List<CelestialBody> activeAsteroidsSet = new List<CelestialBody>();
    private List<CelestialBody> activeStarsSet = new List<CelestialBody>();
    private List<CelestialBody> activePlanetsSet = new List<CelestialBody>();
    private List<CelestialBody> gasSet = new List<CelestialBody>();
    private List<CelestialBody> asteroidSet = new List<CelestialBody>();
    private List<CelestialBody> starSet = new List<CelestialBody>();
    private List<CelestialBody> planetSet = new List<CelestialBody>();

    private float gasAbsorbSFXTimer;
    private float asteroidAbsorbSFXTimer;
    private float planetAbsorbSFXTimer;
    private float starAbsorbSFXTimer;
    
    void Awake()
    {
        vcHelper = virtualCamera.GetComponent<VCHelper>();
        spawnTimer = spawnInterval;
        InitializeListsAndCelestialBodies();

        if (!player.activeInHierarchy)
        {
            overrideSpawning = true;

            for (int i = 0; i < initialCelestialBodies.Length; i++)
            {
                if (initialCelestialBodies[i].Type == CelestialBodyType.Tier1)
                    activeGasSet.Remove(initialCelestialBodies[i]);

                else if (initialCelestialBodies[i].Type == CelestialBodyType.Tier2)
                    activeAsteroidsSet.Remove(initialCelestialBodies[i]);

                else if (initialCelestialBodies[i].Type == CelestialBodyType.Tier3)
                    activePlanetsSet.Remove(initialCelestialBodies[i]);

                else if (initialCelestialBodies[i].Type == CelestialBodyType.Tier4)
                    activeStarsSet.Remove(initialCelestialBodies[i]);

                initialCelestialBodies[i].gameObject.SetActive(false);
            }
        }
    }   

    void Start()
    {
        HUD = HUDController.Instance;
        GM = GameManager.Instance;
        GM.OnCutsceneStarted += OnCutsceneStarted;
        GM.OnCutsceneEnded += OnCutsceneEnded;
        GM.OnLevelChanged += UpdateSpawnInterval;
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
            GM.OnLevelChanged -= UpdateSpawnInterval;
        }
    }

    private void UpdateSpawnInterval()
    {
        if (GM.CurrentLevel == GameManager.Level.Level4)
        {
            spawnInterval = higherLevelSpawnInterval;
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
                gasSet.Add(initialCelestialBodies[i]);
                activeGasSet.Add(initialCelestialBodies[i]);
                initialCelestialBodies[i].InitialBoost(gasSettings);
            }
            else if (initialCelestialBodies[i].Type == CelestialBodyType.Tier2)
            {
                asteroidSet.Add(initialCelestialBodies[i]);
                activeAsteroidsSet.Add(initialCelestialBodies[i]);
                initialCelestialBodies[i].InitialBoost(asteroidSettings);
            }
            else if (initialCelestialBodies[i].Type == CelestialBodyType.Tier3)
            {
                planetSet.Add(initialCelestialBodies[i]);
                activePlanetsSet.Add(initialCelestialBodies[i]);
                initialCelestialBodies[i].InitialBoost(planetSettings);
            }
            else if (initialCelestialBodies[i].Type == CelestialBodyType.Tier4)
            {
                starSet.Add(initialCelestialBodies[i]);
                activeStarsSet.Add(initialCelestialBodies[i]);
                initialCelestialBodies[i].InitialBoost(starSettings);
            }
        }

        for (int i = 0; i < amountOfBodiesLevel9.MaxNumOfBodiesToSpawn; i++)
        {
            GameObject gas = Instantiate(gasSettings.GetCelestialBodyPrefab(), spawnOffscreenPos, Quaternion.identity, transform);
            GameObject asteroid = Instantiate(asteroidSettings.GetCelestialBodyPrefab(), spawnOffscreenPos, Quaternion.identity, transform);
            GameObject star = Instantiate(starSettings.GetCelestialBodyPrefab(), spawnOffscreenPos, Quaternion.identity, transform);
            GameObject planet = Instantiate(planetSettings.GetCelestialBodyPrefab(), spawnOffscreenPos, Quaternion.identity, transform);

            CelestialBody gasLogic = gas.GetComponent<CelestialBody>();
            CelestialBody asteroidLogic = asteroid.GetComponent<CelestialBody>();
            CelestialBody starLogic = star.GetComponent<CelestialBody>();
            CelestialBody planetLogic = planet.GetComponent<CelestialBody>();

            gasSet.Add(gasLogic);
            asteroidSet.Add(asteroidLogic);
            starSet.Add(starLogic);
            planetSet.Add(planetLogic);

            gas.SetActive(false);
            asteroid.SetActive(false);
            star.SetActive(false);
            planet.SetActive(false);
           
            gasLogic.Manager = this;
            asteroidLogic.Manager = this;
            starLogic.Manager = this;
            planetLogic.Manager = this;
        }
    }

    private bool CannotMove()
    {
        return GM.SpecialAnimationPlaying || GM.CutscenePlaying;
    }

    void FixedUpdate()
    {
        if (CannotMove())
            return;

        if (moveableCelestialBodiesSet.Count > 0)
        {
            for (int i = 0; i < moveableCelestialBodiesSet.Count; i++)
            {
                if (moveableCelestialBodiesSet[i].Type == CelestialBodyType.Tier1)
                    ManageCelestialBody(moveableCelestialBodiesSet[i], gasSettings);

                else if (moveableCelestialBodiesSet[i].Type == CelestialBodyType.Tier2)
                    ManageCelestialBody(moveableCelestialBodiesSet[i], starSettings);

                else if (moveableCelestialBodiesSet[i].Type == CelestialBodyType.Tier3)
                    ManageCelestialBody(moveableCelestialBodiesSet[i], planetSettings);

                else
                    ManageCelestialBody(moveableCelestialBodiesSet[i], asteroidSettings);
            }
        }
    }

    private void ManageCelestialBody(CelestialBody celestialBody, CelestialBodySettings settings)
    {
        celestialBody.MoveToPlayer(settings);
        celestialBody.PlotEventHorizonPath(settings);
        celestialBody.AnimateLine(settings);
    }

    private bool CanSpawn()
    {
        return (activeAsteroidsSet.Count + activeStarsSet.Count + activePlanetsSet.Count + activeGasSet.Count) < MaxOfBodiesToSpawn()
            && !overrideSpawning
            && !GM.SpecialAnimationPlaying;
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
    }

    private void HandleAbsorbSFXTimers()
    {
        gasAbsorbSFXTimer -= Time.deltaTime;
        asteroidAbsorbSFXTimer -= Time.deltaTime;
        planetAbsorbSFXTimer -= Time.deltaTime;
        starAbsorbSFXTimer -= Time.deltaTime;
    }
    
    private void SpawnCelestialBody()
    {
        CelestialBody celestialBody = CelestialBody();

        celestialBody.transform.position = GetRandomValidPosition();
        celestialBody.gameObject.SetActive(true);

        if (celestialBody.Type == CelestialBodyType.Tier1)
        {
            activeGasSet.Add(celestialBody);
            celestialBody.InitialBoost(gasSettings);
        }
        else if (celestialBody.Type == CelestialBodyType.Tier2)
        {
            activeAsteroidsSet.Add(celestialBody);
            celestialBody.InitialBoost(asteroidSettings);
        }
        else if (celestialBody.Type == CelestialBodyType.Tier4)
        {
            activeStarsSet.Add(celestialBody);
            celestialBody.InitialBoost(starSettings);
        }
        else if (celestialBody.Type == CelestialBodyType.Tier3)
        {
            activePlanetsSet.Add(celestialBody);
            celestialBody.InitialBoost(planetSettings);
        }
    }

    public void PerformAbsorbBehavior(CelestialBodyType _type, CodexEntry _entryData)
    {
        CacheHUDIfValid();

        PlayAbsorbSFX(_type);
        HUD.CheckCodexEntry(_entryData);
    }

    private void PlayAbsorbSFX(CelestialBodyType _type)
    {
        AmountOfBodiesPerLevel levelSpecificCelestialBodySettings = LevelSpecificCelestialBodySettings();

        if (_type == CelestialBodyType.Tier1 && gasAbsorbSFXTimer <= 0)
        {
            gasAbsorbSFXTimer = gasSettings.TimeBetweenAbsorbSFXCalls;
            BHBAudio.PlayOneShotAttachedFromLoadedSampleData(SFXBank.GasDescription, player, levelSpecificCelestialBodySettings.VolumeOfAbsorbSoundsPerLevel.x);
        }
        else if (_type == CelestialBodyType.Tier2 && asteroidAbsorbSFXTimer <= 0)
        {
            asteroidAbsorbSFXTimer = asteroidSettings.TimeBetweenAbsorbSFXCalls;
            BHBAudio.PlayOneShotAttachedFromLoadedSampleData(SFXBank.AsteroidDescription, player, levelSpecificCelestialBodySettings.VolumeOfAbsorbSoundsPerLevel.y);
        }
        else if (_type == CelestialBodyType.Tier3 && planetAbsorbSFXTimer <= 0)
        {
            planetAbsorbSFXTimer = planetSettings.TimeBetweenAbsorbSFXCalls;
            BHBAudio.PlayOneShotAttachedFromLoadedSampleData(SFXBank.PlanetDescription, player, levelSpecificCelestialBodySettings.VolumeOfAbsorbSoundsPerLevel.z);
        }
        else if (_type == CelestialBodyType.Tier4 && starAbsorbSFXTimer <= 0)
        {
            starAbsorbSFXTimer = starSettings.TimeBetweenAbsorbSFXCalls;
            BHBAudio.PlayOneShotAttachedFromLoadedSampleData(SFXBank.StarDescription, player, levelSpecificCelestialBodySettings.VolumeOfAbsorbSoundsPerLevel.w);
        }
    }

    public float GetMass(CelestialBodyType _type)
    {
        if (_type == CelestialBodyType.Tier2)
            return asteroidSettings.Mass;

        else if (_type == CelestialBodyType.Tier4)
            return starSettings.Mass;

        else if (_type == CelestialBodyType.Tier1)
            return gasSettings.Mass;

        else
            return planetSettings.Mass;
    }

    public void MakeCelestialBodyMoveable(CelestialBody celestialBody)
    {
        if (!moveableCelestialBodiesSet.Contains(celestialBody))
            moveableCelestialBodiesSet.Add(celestialBody);
    }

    public void RemoveCelestialBodyFromActiveSet(CelestialBody celestialBody)
    {
        if (celestialBody.Type == CelestialBodyType.Tier2)
            activeAsteroidsSet.Remove(celestialBody);

        else if (celestialBody.Type == CelestialBodyType.Tier4)
            activeStarsSet.Remove(celestialBody);
        
        else if (celestialBody.Type == CelestialBodyType.Tier1)
            activeGasSet.Remove(celestialBody);
        
        else if (celestialBody.Type == CelestialBodyType.Tier3)
            activePlanetsSet.Remove(celestialBody);

        moveableCelestialBodiesSet.Remove(celestialBody);
    }

    public float NewOrbitMultiplier(CelestialBodyType _type)
    {
        if (_type == CelestialBodyType.Tier2)
            return asteroidSettings.NewOrbitMultiplier(GM);
        
        else if (_type == CelestialBodyType.Tier4)
            return starSettings.NewOrbitMultiplier(GM);
        
        else if (_type == CelestialBodyType.Tier1)
            return gasSettings.NewOrbitMultiplier(GM);
        
        else
            return planetSettings.NewOrbitMultiplier(GM);
    }
    public float MaxOrbitMultiplier(CelestialBodyType _type)
    {
        if (_type == CelestialBodyType.Tier2)
            return asteroidSettings.GetMaxOribtRange();
        
        else if (_type == CelestialBodyType.Tier4)
            return starSettings.GetMaxOribtRange();
        
        else if (_type == CelestialBodyType.Tier1)
            return gasSettings.GetMaxOribtRange();
        
        else
            return planetSettings.GetMaxOribtRange();
    }
    public float TravelSpeed(CelestialBodyType _type)
    {
        if (_type == CelestialBodyType.Tier2)
            return asteroidSettings.SpeedToPlayer;

        else if (_type == CelestialBodyType.Tier4)
            return starSettings.SpeedToPlayer;

        else if (_type == CelestialBodyType.Tier1)
            return gasSettings.SpeedToPlayer;

        else
            return planetSettings.SpeedToPlayer;
    }
    public float GetMinOrbitMultiplier(CelestialBodyType _type)
    {
        if (_type == CelestialBodyType.Tier2)
            return asteroidSettings.GetMinOrbitMultiplier(GM);
        
        else if (_type == CelestialBodyType.Tier4)
            return starSettings.GetMinOrbitMultiplier(GM);
        
        else if (_type == CelestialBodyType.Tier1)
            return gasSettings.GetMinOrbitMultiplier(GM);
        
        else
            return planetSettings.GetMinOrbitMultiplier(GM);
    }
    public float OrbitDeclineRate(CelestialBodyType _type)
    {
        if (_type == CelestialBodyType.Tier2)
            return asteroidSettings.OrbitDeclineRate;
        
        else if (_type == CelestialBodyType.Tier4)
            return starSettings.OrbitDeclineRate;
        
        else if (_type == CelestialBodyType.Tier1)
            return gasSettings.OrbitDeclineRate;
        
        else
            return planetSettings.OrbitDeclineRate;
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

        if (celestialBodyType == CelestialBodyType.Tier2)
        {
            for (int i=0; i < asteroidSet.Count; i++)
            {
                if (!asteroidSet[i].gameObject.activeInHierarchy)
                {
                    celestialBody = asteroidSet[i];
                    break;
                }
            }
        }
        else if (celestialBodyType == CelestialBodyType.Tier1)
        {
            for (int i=0; i < gasSet.Count; i++)
            {
                if (!gasSet[i].gameObject.activeInHierarchy)
                {
                    celestialBody = gasSet[i];
                    break;
                }
            }
        }
        else if (celestialBodyType == CelestialBodyType.Tier3)
        {
            for (int i=0; i < planetSet.Count; i++)
            {
                if (!planetSet[i].gameObject.activeInHierarchy)
                {
                    celestialBody = planetSet[i];
                    break;
                }
            }
        }
        else if (celestialBodyType == CelestialBodyType.Tier4)
        {
            for (int i=0; i < starSet.Count; i++)
            {
                if (!starSet[i].gameObject.activeInHierarchy)
                {
                    celestialBody = starSet[i];
                    break;
                }
            }
        }

        return celestialBody;
    }

    private Vector3 GetRandomValidPosition()
    {
        Vector2 newPosition = new Vector2(Random.Range(spawnBoxWestBound.position.x, spawnBoxEastBound.position.x), Random.Range(spawnBoxSouthBound.position.y, spawnBoxNorthBound.position.y));
        bool isInCameraView = IsPositionInCameraView(newPosition);
        bool isInRoom = IsPositionInRoom(newPosition);

        if (!isInCameraView && isInRoom)
            return newPosition;
        
        else
        {
            while (!(!isInCameraView && isInRoom))
            {
                newPosition = new Vector2(Random.Range(spawnBoxWestBound.position.x, spawnBoxEastBound.position.x), Random.Range(spawnBoxSouthBound.position.y, spawnBoxNorthBound.position.y));
                isInCameraView = IsPositionInCameraView(newPosition);
                isInRoom = IsPositionInRoom(newPosition);
            }

            return newPosition;
        }
    }

    private bool IsPositionInRoom(Vector3 position)
    {
        return position.x > roomWestBound.position.x && position.x < roomEastBound.position.x && position.y > roomSouthBound.position.y && position.y < roomNorthBound.position.y;
    }
    
    private bool IsPositionInCameraView(Vector3 position)
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
                activeGasSet.Add(initialCelestialBodies[i]);
                initialCelestialBodies[i].InitialBoost(gasSettings);
            }
            else if (initialCelestialBodies[i].Type == CelestialBodyType.Tier2)
            {
                activeAsteroidsSet.Add(initialCelestialBodies[i]);
                initialCelestialBodies[i].InitialBoost(asteroidSettings);
            }
            else if (initialCelestialBodies[i].Type == CelestialBodyType.Tier3)
            {
                activePlanetsSet.Add(initialCelestialBodies[i]);
                initialCelestialBodies[i].InitialBoost(planetSettings);
            }
            else if (initialCelestialBodies[i].Type == CelestialBodyType.Tier4)
            {
                activeStarsSet.Add(initialCelestialBodies[i]);
                initialCelestialBodies[i].InitialBoost(starSettings);
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
    public Vector4 PercentageOfBodiesToSpawn; //x = Gas, y = Asteroid, z = Planet, w = Star
    public int MaxNumOfBodiesToSpawn;
    public Vector4 VolumeOfAbsorbSoundsPerLevel; //x = Gas, y = Asteroid, z = Planet, w = Star
}
