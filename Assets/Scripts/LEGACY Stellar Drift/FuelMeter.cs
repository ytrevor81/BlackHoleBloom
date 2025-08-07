using UnityEngine;
using UnityEngine.UI;

public class FuelMeter : MonoBehaviour
{
    [SerializeField] private Image meterImage;

    public void UpdateMeter(float fuel)
    {
        float fuelConverted = fuel / 100;
        meterImage.fillAmount = fuelConverted;
    }
}
