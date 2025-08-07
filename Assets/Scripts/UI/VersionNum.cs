using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VersionNum : MonoBehaviour
{
    [SerializeField] private TMP_Text versionText;

    void Awake()
    {
        versionText.text = Application.version + " (Alpha)";
    }
}
