using UnityEngine;
using UnityEngine.UI;

public class OptionsSlider : MonoBehaviour
{
    [SerializeField] private OptionsPage controller;
    [SerializeField] private OptionsPage.SettingType settingType;
    [SerializeField] private Slider slider;

    public void SliderValueChanged_Event(float _value)
    {
        controller.SliderValueChanged(settingType, _value);
    }

    public void InitializeSlider(float _value) //called from a controller to minimize OnEnable calls
    {
        slider.value = _value;
    }
}
