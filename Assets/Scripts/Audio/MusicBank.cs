using FMOD.Studio;
using UnityEngine;

public class MusicBank : ScriptableObject
{
    [field: Header("PARAMETERS")]
    [field: Space]
    [field: SerializeField] public string MusicEQParameter { get; private set; }
    [field: SerializeField] public string ProgressParameter { get; private set; }
    [field: SerializeField] public float L2ProgressValue { get; private set; }
    [field: SerializeField] public float L3ProgressValue { get; private set; }
    [field: SerializeField] public float L4ProgressValue { get; private set; }
    [field: SerializeField] public float L5ProgressValue { get; private set; }

    [field: Header("TRACKS")]
    [field: Space]
    [field: SerializeField] public string NebulaTrack { get; private set; }
    public EventDescription NebulaTrackDescription;
    private bool nebulaLoaded;

    public void LoadRoomTracks()
    {
        if (!nebulaLoaded)
        {
            nebulaLoaded = true;
            NebulaTrackDescription = BHBAudio.EventDescriptionWithLoadedSampleData(NebulaTrack);
        }
    }
    
    public void UnloadRoomTracks()
    {
        if (nebulaLoaded)
        {
            nebulaLoaded = false;
            BHBAudio.UnloadSampleData(NebulaTrackDescription);
        }
    }
}
