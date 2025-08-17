using UnityEngine;

public class CodexItem : MonoBehaviour
{
    [SerializeField] private CodexController controller;
    [field: SerializeField] public CodexEntry CodexEntry { get; private set; }
    [SerializeField] private GameObject exlaimationPoint;

    void OnEnable()
    {
        if (!CodexEntry.ReadInCodex)
            exlaimationPoint.SetActive(true);
    }

    void OnDisable()
    {
        exlaimationPoint.SetActive(false);
    }

    public void ViewCodexEntry_Event()
    {
        CodexEntry.ReadInCodex = true;
        controller.ViewCodexEntryProfile(CodexEntry);
    }
}
