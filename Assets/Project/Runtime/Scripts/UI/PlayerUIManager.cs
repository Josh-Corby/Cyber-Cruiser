using UnityEngine;
using UnityEngine.UI;

namespace CyberCruiser
{
    public class PlayerUIManager : GameBehaviour
    {
        [SerializeField] private UISlider _playerHealthSlider;
        [SerializeField] private UISlider _weaponHeatSlider;

        [SerializeField] private Image _shieldActiveImage;
        [SerializeField] private Image _shieldActiveUnderlay;
        [SerializeField] private Image _shieldUnderlay;

        [SerializeField] private Sprite _healthFillSprite;
        [SerializeField] private Sprite _shieldFillSprite;

        [SerializeField] private Sprite _heatFillSprite;
        [SerializeField] private Sprite _overheatFillSprite;
        [SerializeField] private Sprite _weaponPackFillSprite;

        [SerializeField] private Image _pickupImage;

        private void OnEnable()
        {
            GameManager.OnMissionStart += ResetUI;
            Pickup.OnBossPickup += SetPickupImage;
        }

        private void OnDisable()
        {
            GameManager.OnMissionEnd -= ResetUI;
            Pickup.OnBossPickup -= SetPickupImage;
        }

        private void ResetUI()
        {
            _pickupImage.enabled = false;
            _pickupImage.sprite = null;
        }

        private void SetPickupImage(PickupInfo info)
        {
            Sprite newSprite = info.Sprite;
            if(newSprite == null)
            {
                _pickupImage.enabled = false;
                _pickupImage.sprite = null;
            }

            _pickupImage.sprite = newSprite;
            _pickupImage.enabled = true;
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