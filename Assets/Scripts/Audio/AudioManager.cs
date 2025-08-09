using System.Collections;
using FMOD.Studio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private GameManager GM;
    public enum MusicTrack
    {
        Nebula,
        SolarSystem,
        Interstellar
    }

    [field: SerializeField] public MusicTrack TrackToPlayOnStart { get; set; }
    [field: SerializeField] public MusicBank MusicBank { get; private set; }
    [field: SerializeField] public SFXBank SFXBank { get; private set; }
    public float MasterVolume { get; private set; } = 1f;
    public float MusicVolume { get; private set; } = 1f;
    public float SFXVolume { get; private set; } = 1f;

    private bool initialized;
    private bool musicIsChanging;
    private EventInstance mainMusicInstance;
    private IEnumerator currentCoroutine;
    private WaitForSeconds delayChangeMusicTime = new WaitForSeconds(1f);
    private VCA masterController;
    private VCA musicController;
    private VCA sfxController;

    void Awake()
    {
        MusicBank.LoadRoomTracks();
        SFXBank.LoadUISFX();
    }

    void Start()
    {
        GM = GameManager.Instance;
        initialized = true;

        if (HUDController.Instance != null) //if it's not null, it means we're in editor playing a specific setup. Do NOT wrap this in #if UNITY_EDITOR
            PlayMusicTrackImmediately(TrackToPlayOnStart);

        masterController = FMODUnity.RuntimeManager.GetVCA("vca:/Master");
        musicController = FMODUnity.RuntimeManager.GetVCA("vca:/Music");
        sfxController = FMODUnity.RuntimeManager.GetVCA("vca:/SFX");
    }

    void OnDisable()
    {
        if (!initialized) return;

        SFXBank.UnloadPlayerSFX();
        SFXBank.UnloadCelestialBodies();
        MusicBank.UnloadRoomTracks();
        SFXBank.UnloadUISFX();
    }

    private EventDescription GetMusicTrackDescription(MusicTrack track)
    {
        if (track == MusicTrack.Nebula)
            return MusicBank.NebulaTrackDescription;

        return MusicBank.NebulaTrackDescription;
    }

    public void PlayMusicTrackImmediately(MusicTrack track)
    {
        mainMusicInstance.clearHandle();
        mainMusicInstance = BHBAudio.CreateInstanceLoadedSampleData(GetMusicTrackDescription(track));
        BHBAudio.AttachInstanceToGameObject(mainMusicInstance, gameObject);
        mainMusicInstance.start();

        musicIsChanging = false;
    }

    public void ChangeMusicTrack(MusicTrack track)
    {
        if (musicIsChanging) return;

        musicIsChanging = true;

        if (mainMusicInstance.isValid())
        {
            BHBAudio.StopAndReleaseInstanceFadeOut(mainMusicInstance);
            currentCoroutine = ChangeMusicTrackCoroutine(track);
            StartCoroutine(currentCoroutine);
        }
        else
            PlayMusicTrackImmediately(track);
    }

    private IEnumerator ChangeMusicTrackCoroutine(MusicTrack track)
    {
        yield return delayChangeMusicTime;

        mainMusicInstance.clearHandle();
        PlayMusicTrackImmediately(track);
    }

    public void StopMusicFadeOut()
    {
        BHBAudio.StopAndReleaseInstanceFadeOut(mainMusicInstance);
    }

    public bool MusicPlaying()
    {
        PLAYBACK_STATE state;
        mainMusicInstance.getPlaybackState(out state);

        return state != PLAYBACK_STATE.STOPPED;
    }

    public void SetMusicProgress()
    {
        if (GM.CurrentLevel == GameManager.Level.Level3)
            mainMusicInstance.setParameterByName(MusicBank.ProgressParameter, MusicBank.L2ProgressValue);

        else if (GM.CurrentLevel == GameManager.Level.Level4)
            mainMusicInstance.setParameterByName(MusicBank.ProgressParameter, MusicBank.L3ProgressValue);

        else if (GM.CurrentLevel == GameManager.Level.Level6)
            mainMusicInstance.setParameterByName(MusicBank.ProgressParameter, MusicBank.L4ProgressValue);

        else if (GM.CurrentLevel == GameManager.Level.Level8)
            mainMusicInstance.setParameterByName(MusicBank.ProgressParameter, MusicBank.L5ProgressValue);
    }

    public void ChangeMasterVolume(float _value)
    {
        MasterVolume = _value;
        masterController.setVolume(MasterVolume);
    }
    public void ChangeMusicVolume(float _value)
    {
        MusicVolume = _value;
       musicController.setVolume(MusicVolume);
    }
    public void ChangeSFXVolume(float _value)
    {
        SFXVolume = _value;
        sfxController.setVolume(SFXVolume);
    }
}
