using UnityEngine;
using UnityEngine.UI;

namespace CyberCruiser
{
    public class UISlider : GameBehaviour
    {
        private Slider _slider;
        [SerializeField] private Image _fillImage;

        [Header("Lerp Settins")]
        [SerializeField] private bool _doesSliderLerp;
        [SerializeField] private float _sliderLerpSpeed;
        [SerializeField] private bool _isSliderLerping;

        private float _targetLerpValue;

        private float CurrentValue
        {
            get => _slider.value;
            set
            {
                if (_doesSliderLerp)
                {
                    if (_slider.value == TargetValue)
                    {
                        _isSliderLerping = false;
                    }
                }

                _slider.value = value;
            }
        }

        private int MinValue
        {
            get => (int)_slider.minValue;
            set => _slider.minValue = value;
        }

        private int MaxValue
        {
            get => (int)_slider.maxValue;
            set => _slider.maxValue = value;
        }

        private float TargetValue
        {
            get => _targetLerpValue;
            set
            {
                _targetLerpValue = value;
                _isSliderLerping = true;
            }
        }

        private void Awake()
        {
            _slider = GetComponent<Slider>();
        }

        private void Update()
        {
            if (_isSliderLerping)
            {
                CurrentValue = Mathf.Lerp(CurrentValue, _targetLerpValue, _sliderLerpSpeed);
            }
        }

        public void EnableSliderAtMaxValue(int maxValue)
        {
            gameObject.SetActive(true);
            SetSliderToMax(maxValue);
        }

        public void EnableAndSetSlider(float currentValue, int minValue, int maxValue)
        {
            gameObject.SetActive(true);

            MaxValue = maxValue;
            MinValue = minValue;
            CurrentValue = currentValue;
        }

        public void DisableSlider()
        {
            gameObject.SetActive(false);
        }

        public void SetSliderValues(float currentValue, int maxValue)
        {
            MaxValue = maxValue;
            CurrentValue = currentValue;
        }

        public void SetSliderToMax(int maxValue)
        {
            MaxValue = maxValue;
            CurrentValue = MaxValue;
        }

        public void SetSliderValue(float value)
        {
            if (_doesSliderLerp)
            {
                TargetValue = value;
            }
            else
            {
                CurrentValue = value;
            }
        }

        public void ChangeSliderValue(float value)
        {
            if (gameObject.activeSelf)
            {
                SetSliderValue(value);
            }
        }

        public void SetFillImage(Sprite sprite)
        {
            _fillImage.sprite = sprite;
        }

    }
}