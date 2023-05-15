using UnityEngine;

public class PlayerUIManager : GameBehaviour<PlayerUIManager>
{
    [SerializeField] private UISlider _playerHealthSlider;
    [SerializeField] private UISlider _playerShieldSlider;
    [SerializeField] private UISlider _weaponUpgradeSlider;
    [SerializeField] private UISlider _weaponHeatSlider;

    [SerializeField] private GameObject _playerHealthBarUI;
    [SerializeField] private GameObject _playerShieldBarUI;
    [SerializeField] private GameObject _weaponUpgradeBarUI;

    protected override void Awake()
    {
        base.Awake();
        _weaponUpgradeBarUI.SetActive(false);
    }

    #region Delegates
    private void OnEnable()
    {
        PlayerManager.OnPlayerMaxHealthSet += (maxHealth) => EnableSliderAtMaxValue(_playerHealthSlider, maxHealth);
        PlayerManager.OnPlayerCurrentHealthChange += (currentHealth) => ChangeSliderValue(_playerHealthSlider, currentHealth);

        PlayerWeaponController.OnWeaponUpgradeStart += (weaponUpgradeDuration) => EnableSliderAtMaxValue(_weaponUpgradeSlider, weaponUpgradeDuration);
        PlayerWeaponController.OnWeaponUpgradeTimerTick += (weaponUpgradeCurrentTime) => ChangeSliderValue(_weaponUpgradeSlider, weaponUpgradeCurrentTime);
        PlayerWeaponController.OnWeaponUpgradeFinished += () => DisableSlider(_weaponUpgradeSlider);

        PlayerWeaponController.OnWeaponHeatInitialized += (maxHeat) => EnableSliderAtMaxValue(_weaponHeatSlider, maxHeat);
        PlayerWeaponController.OnHeatChange += (currentHeat) => ChangeSliderValue(_weaponHeatSlider, currentHeat);
        PlayerWeaponController.OnOverheatStatusChange += OverheatUI;

        PlayerShieldController.OnPlayerShieldsActivated += (shieldDuration) => EnableSliderAtMaxValue(_playerShieldSlider,shieldDuration);
        PlayerShieldController.OnPlayerShieldsDeactivated += () => DisableSlider(_playerShieldSlider);
        PlayerShieldController.OnPlayerShieldsValueChange += (currentShieldHealth) => ChangeSliderValue(_playerShieldSlider,currentShieldHealth);
    }

    private void OnDisable()
    {
        PlayerManager.OnPlayerMaxHealthSet -= (maxHealth) => EnableSliderAtMaxValue(_playerHealthSlider, maxHealth);
        PlayerManager.OnPlayerCurrentHealthChange -= (currentHealth) => ChangeSliderValue(_playerHealthSlider, currentHealth);

        PlayerWeaponController.OnWeaponUpgradeStart -= (weaponUpgradeDuration) => EnableSliderAtMaxValue(_weaponUpgradeSlider, weaponUpgradeDuration);
        PlayerWeaponController.OnWeaponUpgradeTimerTick -= (weaponUpgradeCurrentTime) => ChangeSliderValue(_weaponUpgradeSlider, weaponUpgradeCurrentTime);
        PlayerWeaponController.OnWeaponUpgradeFinished -= () => DisableSlider(_weaponUpgradeSlider);

        PlayerWeaponController.OnWeaponHeatInitialized -= (maxHeat) => EnableSliderAtMaxValue(_weaponHeatSlider, maxHeat);
        PlayerWeaponController.OnHeatChange -= (currentHeat) => ChangeSliderValue(_weaponHeatSlider, currentHeat);
        PlayerWeaponController.OnOverheatStatusChange -= OverheatUI;

        PlayerShieldController.OnPlayerShieldsActivated -= (shieldDuration) => EnableSliderAtMaxValue(_playerShieldSlider, shieldDuration);
        PlayerShieldController.OnPlayerShieldsDeactivated -= () => DisableSlider(_playerShieldSlider);
        PlayerShieldController.OnPlayerShieldsValueChange -= (currentShieldHealth) => ChangeSliderValue(_playerShieldSlider, currentShieldHealth);
    }
    #endregion

    private void EnableSliderAtMaxValue(UISlider slider, int maxValue)
    {
        slider.EnableSliderAtMaxValue(maxValue);
    }

    private void DisableSlider(UISlider slider)
    {
        slider.DisableSlider();
    }

    private void ChangeSliderValue(UISlider slider, float value)
    {
        slider.ChangeSliderValue(value);
    }

    private void OverheatUI(bool status)
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
}
