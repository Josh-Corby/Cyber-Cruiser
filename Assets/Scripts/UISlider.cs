using UnityEngine;
using UnityEngine.UI;

public class UISlider : MonoBehaviour
{
    private Slider _slider;
    [SerializeField] private Image _fillImage;
    [SerializeField] private bool _lerpColour;
    [SerializeField] private Color _minColour;
    [SerializeField] private Color _maxColour;
    [SerializeField] private float _sliderSpeed;
    [SerializeField] private bool _lerpSlider;
    [SerializeField] private bool _isSliderChangingValue;
    [SerializeField] private float _targetValue;

    public float MaxValue
    {
        get => _slider.maxValue;
        set => _slider.maxValue = value;
    }

    public float CurrentValue
    {
        get => _slider.value;
        set
        {
            if (_lerpSlider)
            {
                if (_slider.value == TargetValue)
                {
                    _isSliderChangingValue = false;
                }
            }
            _slider.value = value;
            if (_lerpColour)
            {
                LerpColour();
            }
        }
    }

    public float TargetValue
    {
        get => _targetValue;
        set
        {
            _targetValue = value;
            _isSliderChangingValue = true;
        }
    }

    public Color SliderColour
    {
        get => _fillImage.color;
        set => _fillImage.color = value;
    }

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    private void Update()
    {
        if (_isSliderChangingValue)
        {
            CurrentValue = Mathf.Lerp(CurrentValue, _targetValue, _sliderSpeed);
        }
    }

    public void SetSliderValues(float currentValue, int maxValue)
    {
        MaxValue = maxValue;
        CurrentValue = currentValue;
    }
    public void SetSliderMaxValue(float maxValue)
    {
        MaxValue = maxValue;
        CurrentValue = MaxValue;
    }

    public void SetSliderValue(float value)
    {
        if (_lerpSlider)
        {
            TargetValue = value;
        }
        else
        {
            CurrentValue = value;
        }
    }
    public void SetFillColour(Color colour)
    {
        SliderColour = colour;
    }

    private void LerpColour()
    {
        float percentage = CurrentValue / MaxValue;
        Color lerpedColour = Color.Lerp(_minColour, _maxColour, percentage);
        SetFillColour(lerpedColour);
    }
}
