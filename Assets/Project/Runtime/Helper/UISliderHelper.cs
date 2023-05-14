
public static class UISliderHelper
{
    public static void EnableSlider(UISlider slider, float maxValue)
    {
        slider.gameObject.SetActive(true);
        slider.SetSliderMaxValue(maxValue);
    }

    public static void EnableAndSetSlider(UISlider slider, float currentValue, int maxValue)
    {
        slider.gameObject.SetActive(true);
        slider.SetSliderValues(currentValue, maxValue);
    }

    public static void DisableSlider(UISlider slider)
    {
        slider.gameObject.SetActive(false);
    }

    public static void ChangeSliderValue(UISlider slider, float value)
    {
        if (slider.gameObject.activeSelf)
        {
            slider.SetSliderValue(value);
        }
    }
}
