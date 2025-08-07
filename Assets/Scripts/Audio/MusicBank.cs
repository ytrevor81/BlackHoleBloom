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
    [field: SerializeField] public string Room1Track { get; private set; }
    public EventDescription Room1TrackDescription;
    private bool room1Loaded;

    public void LoadRoomTracks()
    {
        if (!room1Loaded)
        {
            room1Loaded = true;
            Room1TrackDescription = BHBAudio.EventDescriptionWithLoadedSampleData(Room1Track);
        }
    }
    
    public void UnloadRoomTracks()
    {
        if (room1Loaded)
        {
            room1Loaded = false;
            BHBAudio.UnloadSampleData(Room1TrackDescription);
        }
    }
}
