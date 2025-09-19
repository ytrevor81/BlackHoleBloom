using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum Level
    {
        Level1,
        Level2,
        Level3,
        Level4,
        Level5,
        Level6,
        Level7,
        Level8,
        Level9,
        Level10
    }

    public Level CurrentLevel { get; private set; }
    [field: SerializeField] public PlayerSettings Settings { get; private set; }
    [field: SerializeField] public HapticsManager HapticsManager { get; private set; }
    [field: SerializeField] public AudioManager AudioManager { get; private set; }
    [field: SerializeField] public float RoomTime { get; private set; }
    [field: SerializeField] public float TimeAddition { get; private set; }
    
    [field: Space]

    [field: SerializeField] public int Level2ObjectCount { get; private set; }
    [field: SerializeField] public int Level3ObjectCount { get; private set; }
    [field: SerializeField] public int Level4ObjectCount { get; private set; }
    [field: SerializeField] public int Level5ObjectCount { get; private set; }
    [field: SerializeField] public int Level6ObjectCount { get; private set; }
    [field: SerializeField] public int Level7ObjectCount { get; private set; }
    [field: SerializeField] public int Level8ObjectCount { get; private set; }
    [field: SerializeField] public int Level9ObjectCount { get; private set; }
    [field: SerializeField] public int Level10ObjectCount { get; private set; }

    public float Timer { get; set; }
    public long Mass { get; set; }
    public int NumOfObjectsAbsorbed { get; set; }
    public bool CutscenePlaying { get; set; }

    public delegate void GameManagerHandler();

    public event GameManagerHandler OnLevelChanged;
    public event GameManagerHandler OnRoomCompleted;
    public event GameManagerHandler OnCutsceneStarted;
    public event GameManagerHandler OnCutsceneEnded;

    private bool isRoomCompleted;
    public bool CometAnimationPlayedFirstTime { get; set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        Application.runInBackground = false;
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }   

    public void InvokeCutsceneStarted()
    {
        CutscenePlaying = true;
        OnCutsceneStarted?.Invoke();
    }
    public void InvokeCutsceneEnded()
    {
        CutscenePlaying = false;
        OnCutsceneEnded?.Invoke();
    }

    public void ChangeLevelIfValid()
    {
        if (CanLevelUp())
        {
            if (CurrentLevel == Level.Level1)
                CurrentLevel = Level.Level2;
        
            else if (CurrentLevel == Level.Level2)
                CurrentLevel = Level.Level3;

            else if (CurrentLevel == Level.Level3)
                CurrentLevel = Level.Level4;

            else if (CurrentLevel == Level.Level4)
                CurrentLevel = Level.Level5;

            else if (CurrentLevel == Level.Level5)
                CurrentLevel = Level.Level6;

            else if (CurrentLevel == Level.Level6)
                CurrentLevel = Level.Level7;

            else if (CurrentLevel == Level.Level7)
                CurrentLevel = Level.Level8;

            else if (CurrentLevel == Level.Level8)
                CurrentLevel = Level.Level9;

            else if (CurrentLevel == Level.Level9 || CurrentLevel == Level.Level10)
                CurrentLevel = Level.Level10;

            NumOfObjectsAbsorbed = 0;
            OnLevelChanged?.Invoke();
            HapticsManager.HeavyVibration();
        }
        else
        {
            HapticsManager.MediumVibration();
        }

        if (CurrentLevel == Level.Level10 && !isRoomCompleted)
        {
            isRoomCompleted = true;
            OnRoomCompleted?.Invoke();
        }
    }

    private bool CanLevelUp()
    {
        return (CurrentLevel == Level.Level1 && NumOfObjectsAbsorbed >= Level2ObjectCount) ||
               (CurrentLevel == Level.Level2 && NumOfObjectsAbsorbed >= Level3ObjectCount) ||
               (CurrentLevel == Level.Level3 && NumOfObjectsAbsorbed >= Level4ObjectCount) ||
               (CurrentLevel == Level.Level4 && NumOfObjectsAbsorbed >= Level5ObjectCount) ||
               (CurrentLevel == Level.Level5 && NumOfObjectsAbsorbed >= Level6ObjectCount) ||
               (CurrentLevel == Level.Level6 && NumOfObjectsAbsorbed >= Level7ObjectCount) ||
               (CurrentLevel == Level.Level7 && NumOfObjectsAbsorbed >= Level8ObjectCount) ||
               (CurrentLevel == Level.Level8 && NumOfObjectsAbsorbed >= Level9ObjectCount) ||
               (CurrentLevel == Level.Level9 && NumOfObjectsAbsorbed >= Level10ObjectCount) ||
               (CurrentLevel == Level.Level10 && NumOfObjectsAbsorbed >= Level10ObjectCount);
    }

    public void BackToMainMenu()
    {
        isRoomCompleted = false;
        Time.timeScale = 1;
        CurrentLevel = Level.Level1;
        Mass = 0;
        Timer = RoomTime;
        NumOfObjectsAbsorbed = 0;
        CutscenePlaying = false;
        SceneManager.LoadScene("Nebula Room");
    }
}
