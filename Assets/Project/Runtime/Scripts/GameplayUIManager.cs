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
        PlayerManager.OnPlayerMaxHealthSet += UISliderHelper.EnableSlider;
        PlayerManager.OnPlayerCurrentHealthChange += UISliderHelper.ChangeSliderValue;

        PlayerWeaponController.OnWeaponUpgradeStart += UISliderHelper.EnableSlider;
        PlayerWeaponController.OnWeaponUpgradeTimerTick += UISliderHelper.ChangeSliderValue;
        PlayerWeaponController.OnWeaponUpgradeFinished += UISliderHelper.DisableSlider;
        PlayerWeaponController.OnWeaponHeatInitialized += UISliderHelper.EnableAndSetSlider;
        PlayerWeaponController.OnHeatChange += UISliderHelper.ChangeSliderValue;
        PlayerWeaponController.OnOverheatStatusChange += OverheatUI;

        PlayerShieldController.OnPlayerShieldsActivated += UISliderHelper.EnableSlider;
        PlayerShieldController.OnPlayerShieldsDeactivated += UISliderHelper.DisableSlider;
        PlayerShieldController.OnPlayerShieldsValueChange += UISliderHelper.ChangeSliderValue;

        _weaponUpgradeBarUI.SetActive(false);
    }

    private void OnDisable()
    {
        PlayerManager.OnPlayerMaxHealthSet -= UISliderHelper.EnableSlider;
        PlayerManager.OnPlayerCurrentHealthChange -= UISliderHelper.ChangeSliderValue;

        PlayerWeaponController.OnWeaponUpgradeStart -= UISliderHelper.EnableSlider;
        PlayerWeaponController.OnWeaponUpgradeTimerTick -= UISliderHelper.ChangeSliderValue;
        PlayerWeaponController.OnWeaponUpgradeFinished -= UISliderHelper.DisableSlider;
        PlayerWeaponController.OnWeaponHeatInitialized -= UISliderHelper.EnableAndSetSlider;
        PlayerWeaponController.OnHeatChange -= UISliderHelper.ChangeSliderValue;
        PlayerWeaponController.OnOverheatStatusChange -= OverheatUI;

        PlayerShieldController.OnPlayerShieldsActivated -= UISliderHelper.EnableSlider;
        PlayerShieldController.OnPlayerShieldsDeactivated -= UISliderHelper.DisableSlider;
        PlayerShieldController.OnPlayerShieldsValueChange -= UISliderHelper.ChangeSliderValue;
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
