using UnityEngine;

public class GameplayUIManager : GameBehaviour<GameplayUIManager>
{
    public UISlider playerHealthBar;
    public UISlider playerShieldBar;
    public UISlider weaponUpgradeSlider;
    public UISlider weaponHeatBar;

    [SerializeField] private GameObject _playerHealthBarUI;
    [SerializeField] private GameObject _playerShieldBarUI;
    [SerializeField] private GameObject _weaponUpgradeBarUI;

    private void OnEnable()
    {
        PlayerManager.OnPlayerMaxHealthSet += EnableSlider;
        PlayerManager.OnPlayerCurrentHealthChange += ChangeSliderValue;

        PlayerWeaponController.OnWeaponUpgradeStart += EnableSlider;
        PlayerWeaponController.OnWeaponUpgradeTimerTick += ChangeSliderValue;
        PlayerWeaponController.OnWeaponUpgradeFinished += DisableSlider;
        PlayerWeaponController.OnWeaponHeatInitialized += EnableAndSetSlider;
        PlayerWeaponController.OnHeatChange += ChangeSliderValue;
        PlayerWeaponController.OnOverheatStatusChange += OverheatUI;

        PlayerShieldController.OnPlayerShieldsActivated += EnableSlider;
        PlayerShieldController.OnPlayerShieldsDeactivated += DisableSlider;
        PlayerShieldController.OnPlayerShieldsValueChange += ChangeSliderValue;

        _weaponUpgradeBarUI.SetActive(false);
    }

    private void OnDisable()
    {
        PlayerManager.OnPlayerMaxHealthSet -= EnableSlider;
        PlayerManager.OnPlayerCurrentHealthChange -= ChangeSliderValue;

        PlayerWeaponController.OnWeaponUpgradeStart -= EnableSlider;
        PlayerWeaponController.OnWeaponUpgradeTimerTick -= ChangeSliderValue;
        PlayerWeaponController.OnWeaponUpgradeFinished -= DisableSlider;
        PlayerWeaponController.OnWeaponHeatInitialized -= EnableAndSetSlider;
        PlayerWeaponController.OnHeatChange -= ChangeSliderValue;
        PlayerWeaponController.OnOverheatStatusChange -= OverheatUI;

        PlayerShieldController.OnPlayerShieldsActivated -= EnableSlider;
        PlayerShieldController.OnPlayerShieldsDeactivated -= DisableSlider;
        PlayerShieldController.OnPlayerShieldsValueChange -= ChangeSliderValue;
    }

    private void OverheatUI(UISlider slider, bool status)
    {
        if (status)
        {
            slider.SetFillColour(Color.red);
        }
        else if (!status)
        {
            slider.SetFillColour(Color.cyan);
        }
    }
}
