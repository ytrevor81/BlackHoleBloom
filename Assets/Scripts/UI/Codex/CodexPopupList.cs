using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodexPopupList : MonoBehaviour
{
    [SerializeField] private CodexEntry[] entryData;
    [SerializeField] private CodexItemPopupComponents[] popupComponents;

    public void AddToCodex(CodexEntry.CodexEntryType entryType)
    {
        //
    }
}

[System.Serializable]
public struct CodexItemPopupComponents
{
    public GameObject popupObject;
    public TMP_Text titleText;
    public Image icon;
}
