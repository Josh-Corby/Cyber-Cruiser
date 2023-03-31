using UnityEngine;
using System.Collections;
using System;


public class PlayerWeaponController : GameBehaviour
{
    [SerializeField] private Weapon _playerWeapon;
    [SerializeField] private bool _fireInput;
    private bool _controlsEnabled;
    private float _weaponUpgradeCounter;

    private Coroutine _weaponUpgradeCoroutine;

    public static event Action<UISlider, float> OnWeaponUpgradeStart = null;
    public static event Action<UISlider, float> OnWeaponUpgradeTimerTick = null;
    public static event Action<UISlider> OnWeaponUpgradeFinished = null;

    private void Awake()
    {
        _playerWeapon = GetComponentInChildren<Weapon>();
    }
    private void OnEnable()
    {
        InputManager.OnFire += SetFireInput;
        InputManager.OnControlsEnabled += EnableControls;
        InputManager.OnControlsDisabled += DisableControls;

        GameManager.OnGamePaused += DisableControls;
        GameManager.OnGameResumed += EnableControls;

        Pickup.OnWeaponUpgradePickup += WeaponUpgrade;
        GameManager.OnLevelCountDownStart += ResetPlayerWeapon;
    }

    private void OnDisable()
    {
        InputManager.OnFire -= SetFireInput;
        InputManager.OnControlsEnabled -= EnableControls;
        InputManager.OnControlsDisabled -= DisableControls;

        GameManager.OnGamePaused -= DisableControls;
        GameManager.OnGameResumed -= EnableControls;

        Pickup.OnWeaponUpgradePickup -= WeaponUpgrade;
        GameManager.OnLevelCountDownStart -= ResetPlayerWeapon;
    }

    private void Start()
    {
        _fireInput = false;
    }

    private void Update()
    {
        if (_controlsEnabled)
        {
            if (_fireInput)
            {
                CheckForFireInput();
            }
        }
    }

    private void SetFireInput(bool input)
    {
        _fireInput = input;
    }

    private void CheckForFireInput()
    {
        if (!_playerWeapon._holdToFire)
        {
            CancelFireInput();
        }
        FireWeapon();
    }

    private void FireWeapon()
    {
        if (_playerWeapon.readyToFire)
        {
            _playerWeapon.CheckFireTypes();
        }
    }

    private void CancelFireInput()
    {
        _fireInput = false;
    }

    private void EnableControls()
    {
        _controlsEnabled = true;
    }

    private void DisableControls()
    {
        _controlsEnabled = false;
    }

    private void WeaponUpgrade(WeaponUpgradeType upgradeType, float duration)
    {
        if (_weaponUpgradeCoroutine != null)
        {
            StopCoroutine(_weaponUpgradeCoroutine);
        }
        _weaponUpgradeCoroutine = StartCoroutine(WeaponUpgradeTimer(upgradeType, duration));
    }

    private IEnumerator WeaponUpgradeTimer(WeaponUpgradeType upgradeType, float duration)
    {
        //reset in case a different type of pickup is picked up while an upgrade is currently active
        ResetPlayerWeapon();

        switch (upgradeType)
        {
            case WeaponUpgradeType.Scatter_Fixed:
            case WeaponUpgradeType.Scatter_Random:
                _playerWeapon.ScatterUpgrade(upgradeType);
                break;
            case WeaponUpgradeType.Pulverizer:
                _playerWeapon.PulverizerUpgrade();
                break;
        }

        _weaponUpgradeCounter = duration;
        OnWeaponUpgradeStart(GUIM.weaponUpgradeSlider, duration);
        while (_weaponUpgradeCounter > 0)
        {
            _weaponUpgradeCounter -= Time.deltaTime;
            OnWeaponUpgradeTimerTick(GUIM.weaponUpgradeSlider, _weaponUpgradeCounter);
            yield return new WaitForSeconds(0.01f);
        }
        //reset player weapon to its original values after upgrade duration is over
        ResetPlayerWeapon();
    }

    public void ResetPlayerWeapon()
    {
        OnWeaponUpgradeFinished(GUIM.weaponUpgradeSlider);
        _playerWeapon.AssignWeaponInfo();
    }
}
