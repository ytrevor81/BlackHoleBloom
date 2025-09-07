using UnityEngine;

[CreateAssetMenu(fileName = "New Celestial Body Settings", menuName = "Celestial Body Settings")] 
public class CelestialBodySettings : ScriptableObject
{
    public enum CelestialBodyType
    {
        Tier1,
        Tier2,
        Tier3,
        Tier4
    }

    [field: SerializeField] public int Mass {  get; private set; }
    [SerializeField] private Vector2 Level1OrbitRange;
    [SerializeField] private Vector2 Level2OrbitRange;
    [SerializeField] private Vector2 Level3OrbitRange;
    [SerializeField] private Vector2 Level4OrbitRange;
    [SerializeField] private Vector2 Level5OrbitRange;
    [SerializeField] private Vector2 Level6OrbitRange;
    [SerializeField] private Vector2 Level7OrbitRange;
    [SerializeField] private Vector2 Level8OrbitRange;
    [SerializeField] private Vector2 Level9OrbitRange;
    [field: SerializeField] public float OrbitRadiusForOtherCelestialBodies { get; private set; }
    
    [field: Space]
    [field: SerializeField] public Vector2 InitialSpeedRange { get; private set; }
    [field: SerializeField] public Vector2 InitialRotationRange { get; private set; }
    [field: SerializeField] public float SpeedToPlayer { get; private set; }
    [field: SerializeField] public float OrbitDeclineRate { get; private set; }
    [field: SerializeField] public float SpeedIncreaseRate { get; private set; }
    [field: SerializeField] public int TrajectorySteps { get; private set; }
    [field: SerializeField] public float TrajectoryDeltaTime { get; private set; }
    [field: SerializeField] public float RadialGain { get; private set; }
    [field: SerializeField] public float TangentGain { get; private set; }
    [field: SerializeField] public float PathSpeed { get; private set; }

    [field: Space]

    [field: SerializeField] public float TimeBetweenAbsorbSFXCalls { get; private set; }
    [field: SerializeField] public float MinSpawnDistanceFromCamera { get; private set; }

    [field: Space]
    [SerializeField] private GameObject[] celestialBodyPrefabs;

    public GameObject GetCelestialBodyPrefab()
    {
        int randomNum = Random.Range(0, celestialBodyPrefabs.Length);
        return celestialBodyPrefabs[randomNum];
    }

    public float NewOrbitMultiplier(GameManager _GM)
    {
        if (_GM.CurrentLevel == GameManager.Level.Level1)
            return Level1OrbitRange.y;
        
        else if (_GM.CurrentLevel == GameManager.Level.Level2)
            return Level2OrbitRange.y;
        
        else if (_GM.CurrentLevel == GameManager.Level.Level3)
            return Level3OrbitRange.y;
        
        else if (_GM.CurrentLevel == GameManager.Level.Level4)
            return Level4OrbitRange.y;
        
        else if (_GM.CurrentLevel == GameManager.Level.Level5)
            return Level5OrbitRange.y;
        
        else if (_GM.CurrentLevel == GameManager.Level.Level6)
            return Level6OrbitRange.y;
        
        else if (_GM.CurrentLevel == GameManager.Level.Level7)
            return Level7OrbitRange.y;
        
        else if (_GM.CurrentLevel == GameManager.Level.Level8)
            return Level8OrbitRange.y;
        
        else if (_GM.CurrentLevel == GameManager.Level.Level9)
            return Level9OrbitRange.y;
        
        else
            return Level9OrbitRange.y;        
    }
    public float GetMinOrbitMultiplier(GameManager _GM)
    {
        if (_GM.CurrentLevel == GameManager.Level.Level1)
            return Level1OrbitRange.x;

        else if (_GM.CurrentLevel == GameManager.Level.Level2)
            return Level2OrbitRange.x;

        else if (_GM.CurrentLevel == GameManager.Level.Level3)
            return Level3OrbitRange.x;

        else if (_GM.CurrentLevel == GameManager.Level.Level4)
            return Level4OrbitRange.x;

        else if (_GM.CurrentLevel == GameManager.Level.Level5)
            return Level5OrbitRange.x;

        else if (_GM.CurrentLevel == GameManager.Level.Level6)
            return Level6OrbitRange.x;

        else if (_GM.CurrentLevel == GameManager.Level.Level7)
            return Level7OrbitRange.x;

        else if (_GM.CurrentLevel == GameManager.Level.Level8)
            return Level8OrbitRange.x;

        else if (_GM.CurrentLevel == GameManager.Level.Level9)
            return Level9OrbitRange.x;

        else
            return Level9OrbitRange.x;
    }
}
