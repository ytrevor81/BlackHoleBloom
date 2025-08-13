using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodexPopupList : MonoBehaviour
{
    [SerializeField] private CodexEntry cometEntryData;
    [SerializeField] private CodexEntry whiteHoleEntryData;
    [SerializeField] private CodexItemPopupComponents[] popupComponents;
    [SerializeField] private CodexController codexController;

    public void GoToCodexEntryPageFromPopup(CodexEntry _entryData)
    {
        codexController.CurrentCodexEntry = _entryData;
        codexController.gameObject.SetActive(true);
    }
    public void AddToCodex(CodexEntry entryData)
    {
        if (entryData.IsDiscovered)
            return;

        entryData.IsDiscovered = true;

        CodexItemPopupComponents popup = GetPopup();
        popup.titleText.text = entryData.Title;
        popup.icon.sprite = entryData.Icon;
        popup.popup.EntryData = entryData;
        popup.popup.gameObject.SetActive(true);
    }

    public void AddCometToCodex()
    {
        if (cometEntryData.IsDiscovered)
            return;
        
        cometEntryData.IsDiscovered = true;

        CodexItemPopupComponents popup = GetPopup();
        popup.titleText.text = cometEntryData.Title;
        popup.icon.sprite = cometEntryData.Icon;
        popup.popup.EntryData = cometEntryData;
        popup.popup.gameObject.SetActive(true);
    }
    public void AddWhiteHoleToCodex()
    {
        if (whiteHoleEntryData.IsDiscovered)
            return;

        whiteHoleEntryData.IsDiscovered = true;

        CodexItemPopupComponents popup = GetPopup();
        popup.titleText.text = whiteHoleEntryData.Title;
        popup.icon.sprite = whiteHoleEntryData.Icon;
        popup.popup.EntryData = whiteHoleEntryData;
        popup.popup.gameObject.SetActive(true);
    }

    private CodexItemPopupComponents GetPopup()
    {
        CodexItemPopupComponents popup = popupComponents[0];

        for (int i = 0; i < popupComponents.Length; i++)
        {
            if (!popupComponents[i].popup.gameObject.activeInHierarchy)
            {
                popup = popupComponents[i];
                break;
            }
        }

        return popup;
    }
}

[System.Serializable]
public struct CodexItemPopupComponents
{
    public CodexPopup popup;
    public TMP_Text titleText;
    public Image icon;
}
