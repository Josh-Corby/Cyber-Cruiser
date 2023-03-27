using UnityEngine;
using System.Collections;
using System;


public class PlayerWeaponController : MonoBehaviour
{
    [SerializeField] private Weapon _playerWeapon;
    [SerializeField] private bool _fireInput;
    private bool _controlsEnabled;
    private readonly float _weaponUpgradeDuration = 20;

    private Coroutine _weaponUpgradeCoroutine;

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
    }

    private void OnDisable()
    {
        InputManager.OnFire -= SetFireInput;
        InputManager.OnControlsEnabled -= EnableControls;
        InputManager.OnControlsDisabled -= DisableControls;

        GameManager.OnGamePaused -= DisableControls;
        GameManager.OnGameResumed -= EnableControls;

        Pickup.OnWeaponUpgradePickup -= WeaponUpgrade;
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

    private void WeaponUpgrade(PickupType upgradeType)
    {
        if (_weaponUpgradeCoroutine != null)
        {
            StopCoroutine(_weaponUpgradeCoroutine);
        }
        _weaponUpgradeCoroutine = StartCoroutine(WeaponUpgradeTimer(upgradeType));
    }

    private IEnumerator WeaponUpgradeTimer(PickupType upgradeType)
    {
        //reset in case a different type of pickup is picked up while an upgrade is currently active
        ResetPlayerWeapon();

        switch(upgradeType)
        {
            case PickupType.Scatter:
                _playerWeapon.MultiShotUpgrade();
                break;
            case PickupType.Pulverizer:
                _playerWeapon.PulverizerUpgrade();
                break;
        }
        yield return new WaitForSeconds(_weaponUpgradeDuration);

        //reset player weapon to its original values after upgrade duration is over
        ResetPlayerWeapon();
    }

    public void ResetPlayerWeapon()
    {
        _playerWeapon.AssignWeaponInfo();
    }
}
