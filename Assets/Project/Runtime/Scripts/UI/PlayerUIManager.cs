using UnityEngine;
using UnityEngine.UI;

namespace CyberCruiser
{
    public class PlayerUIManager : GameBehaviour
    {
        [SerializeField] private BoolReference _isPlatformPC;

        [SerializeField] private UISlider _playerHealthSlider;
        [SerializeField] private UISlider _weaponHeatSlider;

        [SerializeField] private Image _shieldActiveImage;
        [SerializeField] private Image _shieldActiveUnderlay;
        [SerializeField] private Image _shieldUnderlay;

        // PC Elements
        [SerializeField] private UISlider _pcPlayerHealthSlider;
        [SerializeField] private UISlider _pcWeaponHeatSlider;

        [SerializeField] private Image _pcShieldActiveImage;
        [SerializeField] private Image _pcShieldActiveUnderlay;
        [SerializeField] private Image _pcShieldUnderlay;

        // MobileElements
        [SerializeField] private UISlider _mobilePlayerHealthSlider;
        [SerializeField] private UISlider _mobileWeaponHeatSlider;

        [SerializeField] private Image _mobileShieldActiveImage;
        [SerializeField] private Image _mobileShieldActiveUnderlay;
        [SerializeField] private Image _mobileShieldUnderlay;

        [SerializeField] private Sprite _healthFillSprite;
        [SerializeField] private Sprite _shieldFillSprite;

        [SerializeField] private Sprite _heatFillSprite;
        [SerializeField] private Sprite _overheatFillSprite;
        [SerializeField] private Sprite _weaponPackFillSprite;


        private void Awake()
        {
            if(_isPlatformPC.Value)
            {
                _playerHealthSlider = _pcPlayerHealthSlider;
                _weaponHeatSlider = _pcWeaponHeatSlider;
                _shieldActiveImage = _pcShieldActiveImage;
                _shieldActiveUnderlay = _pcShieldActiveUnderlay;
                _shieldUnderlay = _pcShieldUnderlay;
            }
            else
            {
                _playerHealthSlider = _mobilePlayerHealthSlider;
                _weaponHeatSlider = _mobileWeaponHeatSlider;
                _shieldActiveImage = _mobileShieldActiveImage;
                _shieldActiveUnderlay = _mobileShieldActiveUnderlay;
                _shieldUnderlay = _mobileShieldUnderlay;
            }
        }

        public void EnableSliderAtValue(PlayerSliderTypes slider, int maxValue, float currentValue)
        {
            GetSliderFromEnum(slider).EnableAndSetSlider(currentValue, 0, maxValue);
        }

        public void EnableSliderAtMaxValue(PlayerSliderTypes slider, int maxValue)
        {
            GetSliderFromEnum(slider).EnableSliderAtMaxValue(maxValue);
        }

        public void DisableSlider(PlayerSliderTypes slider)
        {
            GetSliderFromEnum(slider).DisableSlider();
        }

        public void ChangeSliderValue(PlayerSliderTypes slider, float value)
        {
            GetSliderFromEnum(slider).ChangeSliderValue(value);
        }

        public void ToggleHeatSliderFill(bool status)
        {
            if (status)
            {
                _weaponHeatSlider.SetFillImage(_overheatFillSprite);
            }
            else if (!status)
            {
                _weaponHeatSlider.SetFillImage(_heatFillSprite);
            }
        }

        public void ToggleWeaponPackSliderFill(bool status)
        {
            if (status)
            {
                _weaponHeatSlider.SetFillImage(_weaponPackFillSprite);
            }

            else if (!status)
            {
                _weaponHeatSlider.SetFillImage(_heatFillSprite);
            }
        }

        private UISlider GetSliderFromEnum(PlayerSliderTypes slider)
        {
            UISlider currentSlider = null;
            switch (slider)
            {
                case PlayerSliderTypes.Health:
                    currentSlider = _playerHealthSlider;
                    break;
                case PlayerSliderTypes.Heat:
                    currentSlider = _weaponHeatSlider;
                    break;
            }

            return currentSlider;
        }

        public void SetShieldSliderProgress(float progress)
        {
            _shieldActiveImage.fillAmount = progress;
            _shieldActiveUnderlay.fillAmount = progress;
        }

        public void ToggleHealthSliderFill(bool status)
        {
            if(status)
            {
                _playerHealthSlider.SetFillImage(_shieldFillSprite);
            }

            else if (!status)
            {
                _playerHealthSlider.SetFillImage(_healthFillSprite);
            }
        }

        public void ToggleGreenShieldDisplay(bool status)
        {
            SetShieldSliderProgress(1);
            _shieldActiveImage.enabled = status;
            _shieldActiveUnderlay.enabled = status;
            _shieldUnderlay.enabled = status;
        }
    }
}

public enum PlayerSliderTypes
{
    Health,
    Heat,
}