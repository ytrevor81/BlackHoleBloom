using UnityEngine;

public class CodexItem : MonoBehaviour
{
    [SerializeField] private CodexController controller;
    [field: SerializeField] public CodexEntry CodexEntry { get; private set; }

    public void ViewCodexEntry_Event()
    {
        controller.ViewCodexEntryProfile(CodexEntry);
    }
}
