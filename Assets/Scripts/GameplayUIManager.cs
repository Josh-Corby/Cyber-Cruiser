using UnityEngine;
using System;
using TMPro;

public class GameplayUIManager : GameBehaviour<GameplayUIManager>
{
    public UISlider playerHealthBar, playerShieldBar, weaponUpgradeSlider, weaponHeatBar;
    [SerializeField] private GameObject _playerHealthBarUI, _playerShieldBarUI, _weaponUpgradeBarUI;
    [SerializeField] private TMP_Text _plasmaCountText;


    public string PlasmaCount
    {
        set
        {
            _plasmaCountText.text = value;
        }
    }

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
        PlayerManager.OnPlasmaChange += UpdatePlasmaText;
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
        PlayerManager.OnPlasmaChange -= UpdatePlasmaText;
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

    private void UpdatePlasmaText(int plasmaCount)
    {
        PlasmaCount = plasmaCount.ToString();
    }
}
