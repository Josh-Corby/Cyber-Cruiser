using UnityEngine;
using UnityEngine.UI;

public class UISlider : MonoBehaviour
{
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    public void SetSliderValues(float maxValue)
    {
        _slider.maxValue = maxValue;
        _slider.value = _slider.maxValue;
    }

    public void SetSliderValue(float hp)
    {
        _slider.value = hp;
    }
}
