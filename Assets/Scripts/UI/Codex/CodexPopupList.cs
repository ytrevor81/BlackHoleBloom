using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodexPopupList : MonoBehaviour
{
    [SerializeField] private CodexEntry[] entryData;
    [SerializeField] private CodexItemPopupComponents[] popupComponents;
    [SerializeField] private CodexController codexController;

    public void GoToCodexEntryPageFromPopup(CodexEntry _entryData)
    {
        codexController.CurrentCodexEntry = _entryData;
        codexController.gameObject.SetActive(true);
    }
    public void AddToCodex(CodexEntry.CodexEntryType entryType)
    {
        // set correct CodexEntry object as property for next available popup
        //activate said popup
    }
}

[System.Serializable]
public struct CodexItemPopupComponents
{
    public CodexPopup popup;
    public TMP_Text titleText;
    public Image icon;
}
