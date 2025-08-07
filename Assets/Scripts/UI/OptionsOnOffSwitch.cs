using TMPro;
using UnityEngine;

public class OptionsOnOffSwitch : MonoBehaviour
{
    [SerializeField] private OptionsPage controller;
    [SerializeField] private OptionsPage.SettingType settingType;
    [SerializeField] private TMP_Text onText;
    [SerializeField] private TMP_Text offText;

    [SerializeField] private Color offColor;

    public void OnOffSwitchPressed_Event(bool isOn)
    {
        controller.OnOffSwitchPressed(settingType, isOn);
        InitializeSwitch(isOn);
    }

    public void InitializeSwitch(bool _isOn) //called from a controller to minimize OnEnable calls
    {
        if (_isOn)
        {
            onText.color = Color.white;
            offText.color = offColor;
        }
        else
        {
            onText.color = offColor;
            offText.color = Color.white;
        }
    }
}
