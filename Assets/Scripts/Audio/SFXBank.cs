using FMOD.Studio;
using UnityEngine;

public class SFXBank : ScriptableObject
{
    [field: Header("UI")]
    [field: Space]
    [field: SerializeField] public string ButtonPress { get; private set; }
    [field: SerializeField] public string EnterGame { get; private set; }

    public EventDescription ButtonPressDescription;
    public EventDescription EnterGameDescription;

    private bool isUISFXLoaded;

    [field: Header("PLAYER")]
    [field: Space]

    [field: SerializeField] public string LevelUp { get; private set; }
    [field: SerializeField] public string BeginGamePulse { get; private set; }
    public EventDescription LevelUpDescription;
    public EventDescription BeginGamePulseDescription;
    private bool isPlayerSFXLoaded;

    [field: Header("CELESTIAL BODIES")]
    [field: Space]

    [field: SerializeField] public string Gas { get; private set; }
    [field: SerializeField] public string Asteroid { get; private set; }
    [field: SerializeField] public string Planet { get; private set; }
    [field: SerializeField] public string Star { get; private set; }
    public EventDescription GasDescription;
    public EventDescription AsteroidDescription;
    public EventDescription PlanetDescription;
    public EventDescription StarDescription;
    private bool isGasLoaded;
    private bool isAsteroidLoaded;
    private bool isPlanetLoaded;
    private bool isStarLoaded;

    public void LoadCelestialBodies()
    {
        if (!isGasLoaded)
        {
            isGasLoaded = true;
            GasDescription = BHBAudio.EventDescriptionWithLoadedSampleData(Gas);
        }

        if (!isAsteroidLoaded)
        {
            isAsteroidLoaded = true;
            AsteroidDescription = BHBAudio.EventDescriptionWithLoadedSampleData(Asteroid);
        }

        if (!isPlanetLoaded)
        {
            isPlanetLoaded = true;
            PlanetDescription = BHBAudio.EventDescriptionWithLoadedSampleData(Planet);
        }

        if (!isStarLoaded)
        {
            isStarLoaded = true;
            StarDescription = BHBAudio.EventDescriptionWithLoadedSampleData(Star);
        }
    }

    public void UnloadCelestialBodies()
    {
        if (isGasLoaded)
        {
            isGasLoaded = false;
            BHBAudio.UnloadSampleData(GasDescription);
        }

        if (isAsteroidLoaded)
        {
            isAsteroidLoaded = false;
            BHBAudio.UnloadSampleData(AsteroidDescription);
        }

        if (isPlanetLoaded)
        {
            isPlanetLoaded = false;
            BHBAudio.UnloadSampleData(PlanetDescription);
        }

        if (isStarLoaded)
        {
            isStarLoaded = false;
            BHBAudio.UnloadSampleData(StarDescription);
        }
    }

    public void LoadPlayerSFX() //called in awake
    {
        if (isPlayerSFXLoaded) return;

        isPlayerSFXLoaded = true;
        LevelUpDescription = BHBAudio.EventDescriptionWithLoadedSampleData(LevelUp);
        BeginGamePulseDescription = BHBAudio.EventDescriptionWithLoadedSampleData(BeginGamePulse);
    }

    public void UnloadPlayerSFX()
    {
        if (!isPlayerSFXLoaded) return;

        isPlayerSFXLoaded = false;
        EventDescription[] descriptions = { LevelUpDescription, BeginGamePulseDescription };

        BHBAudio.UnloadSampleData(descriptions);
    }
    
    public void LoadUISFX() //called in awake
    {
        if (isUISFXLoaded) return;

        isUISFXLoaded = true;
        ButtonPressDescription = BHBAudio.EventDescriptionWithLoadedSampleData(ButtonPress);
        EnterGameDescription = BHBAudio.EventDescriptionWithLoadedSampleData(EnterGame);
    }
    
    public void UnloadUISFX()
    {
        if (!isUISFXLoaded) return;

        isUISFXLoaded = false;
        BHBAudio.UnloadSampleData(ButtonPressDescription);
        BHBAudio.UnloadSampleData(EnterGameDescription);
    }
}
