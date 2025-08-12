using UnityEngine;

[CreateAssetMenu(fileName = "New Codex Entry", menuName = "Codex Entry")]
public class CodexEntry : ScriptableObject
{
    public enum CodexEntryType
    {
        Helium,
        Hydrogen,
        Oxygen,
        CarbonChrondites,
        Silica,
        Ammonia,
        Protoplanet,
        DwarfPlanet,
        Protostar,
        BrownDwarf,
        Comet,
        WhiteHole,
        BlackHole
    }
    [field: SerializeField] public string Title { get; private set; }
    [field: SerializeField] public int Mass { get; private set; }

    [field: TextArea]
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public bool IsDiscovered { get; set; }

#if UNITY_EDITOR
    private void OnEnable()
    {
        IsDiscovered = false;
    }
#endif
}
