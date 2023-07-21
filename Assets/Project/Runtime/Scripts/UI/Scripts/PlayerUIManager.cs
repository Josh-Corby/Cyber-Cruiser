using UnityEngine;
using UnityEngine.UI;

namespace CyberCruiser
{
    public class PlayerUIManager : GameBehaviour
    {
        [SerializeField] private UISlider _playerHealthSlider;
        [SerializeField] private UISlider _playerShieldSlider;
        [SerializeField] private UISlider _weaponHeatSlider;
        [SerializeField] private UISlider _weaponUpgradeSlider;

        [SerializeField] private Image _pickupImage;
        [SerializeField] private GameObject _weaponUpgradeBarUI;

        private void Awake()
        {
            _weaponUpgradeBarUI.SetActive(false);
        }

        private void OnEnable()
        {
            GameManager.OnMissionStart += ClearPickupImage;
            Pickup.OnBossPickup += (pickupName, pickupSprite) => { SetPickupImage(pickupSprite); };
        }

        private void OnDisable()
        {
            GameManager.OnMissionEnd -= ClearPickupImage;
            Pickup.OnBossPickup -= (pickupName, pickupSprite) => { SetPickupImage(pickupSprite); };
        }

        private void ClearPickupImage()
        {
            _pickupImage.enabled = false;
            _pickupImage.sprite = null;
        }

        private void SetPickupImage(Sprite pickupSprite)
        {
            _pickupImage.sprite = pickupSprite;
            _pickupImage.enabled = true;
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

        public void OverheatUI(bool status)
        {
            if (status)
            {
                _weaponHeatSlider.SetSliderFillColour(Color.red);
            }
            else if (!status)
            {
                _weaponHeatSlider.SetSliderFillColour(Color.cyan);
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
                case PlayerSliderTypes.Shield:
                    currentSlider = _playerShieldSlider;
                    break;
                case PlayerSliderTypes.Heat:
                    currentSlider = _weaponHeatSlider;
                    break;
                case PlayerSliderTypes.WeaponUpgrade:
                    currentSlider = _weaponUpgradeSlider;
                    break;
            }

            return currentSlider;
        }
    }
}

public enum PlayerSliderTypes
{
    Health,
    Shield,
    Heat,
    WeaponUpgrade
}