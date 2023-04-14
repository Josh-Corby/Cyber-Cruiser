using UnityEngine;
using System.Collections;
using System;


public class PlayerWeaponController : GameBehaviour
{
    #region References
    [SerializeField] private Weapon _playerWeapon;
    [SerializeField] private BeamAttack _beamAttack;
    #endregion

    #region Fields
    [SerializeField] private bool _fireInput;
    [SerializeField] private bool _controlsEnabled;
    [SerializeField] private int _heatMax = 100;
    [SerializeField] private float _currentHeat;
    [SerializeField] private float _heatPerShot;
    [SerializeField] private float _heatLossOverTime;
    [SerializeField] private float _cooldownHeatLoss;
    [SerializeField] private float _timebeforeHeatLoss;
    [SerializeField] private float _timeSinceLastShot;
    [SerializeField] private bool _isOverheated;
   
    private float _weaponUpgradeDuration;
    private float _weaponUpgradeCounter;
    private bool _isWeaponUpgradeActive;
    private bool _isHeatDecreasing;
    private Coroutine _weaponUpgradeCoroutine;
    #endregion

    #region Properties
    public int HeatMax
    {
        get
        {
            return _heatMax;
        }
        set
        {
            _heatMax = value;
        }
    }

    public float CurrentHeat
    {
        get
        {
            return _currentHeat;
        }
        set
        {
            _currentHeat = value;

            if (_currentHeat >= HeatMax)
            {
                _currentHeat = HeatMax;
                IsOverheated = true;
                OnOverheatStatusChange(GUIM.weaponHeatBar, true);
            }

            else if (_currentHeat < 0)
            {
                _currentHeat = 0;
            }

            OnHeatChange(GUIM.weaponHeatBar, _currentHeat);
        }
    }

    public float HeatPerShot
    {
        get
        {
            return _heatPerShot;
        }
        set
        {
            _heatPerShot = value;
        }
    }

    public float HeatLossOverTime
    {
        get
        {
            return _heatLossOverTime;
        }
        set
        {
            _heatLossOverTime = value;
        }
    }

    public float CooldownHeatLoss
    {
        get
        {
            return _cooldownHeatLoss;
        }
        set
        {
            _cooldownHeatLoss = value;
        }
    }

    public float TimeBeforeHeatLoss
    {
        get
        {
            return _timebeforeHeatLoss;
        }
        set
        {
            _timebeforeHeatLoss = value;
        }
    }

    public float TimeSinceLastShot
    {
        get
        {
            return _timeSinceLastShot;
        }
        set
        {
            _timeSinceLastShot = value;
            IsHeatDecreasing = _timeSinceLastShot >= TimeBeforeHeatLoss;
        }
    }

    public float WeaponUpgradeDuration
    {
        get
        {
            return _weaponUpgradeDuration;
        }
        set
        {
            _weaponUpgradeDuration = value;
        }
    }

    public bool IsHeatDecreasing
    {
        get
        {
            return _isHeatDecreasing;
        }
        set
        {
            _isHeatDecreasing = value;
        }
    }

    public bool IsOverheated
    {
        get
        {
            return _isOverheated;
        }
        set
        {
            _isOverheated = value;
            OnOverheatStatusChange(GUIM.weaponHeatBar, _isOverheated);
        }
    }

    public bool IsWeaponUpgradeActive
    {
        get
        {
            return _isWeaponUpgradeActive;
        }

        set
        {
            _isWeaponUpgradeActive = value;
        }
    }
    #endregion

    #region Actions
    public static event Action<UISlider, float> OnWeaponUpgradeStart = null;
    public static event Action<UISlider, float> OnWeaponUpgradeTimerTick = null;
    public static event Action<UISlider> OnWeaponUpgradeFinished = null;
    public static event Action<UISlider, float, int> OnWeaponHeatInitialized = null;
    public static event Action<UISlider, float> OnHeatChange = null;
    public static event Action<UISlider, bool> OnOverheatStatusChange = null;
    #endregion

    private void Awake()
    {
        _playerWeapon = GetComponentInChildren<Weapon>();
        _beamAttack = GetComponentInChildren<BeamAttack>();
    }

    private void OnEnable()
    {
        InputManager.OnFire += SetFireInput;
        InputManager.OnControlsEnabled += EnableControls;
        InputManager.OnControlsDisabled += DisableControls;

        GameManager.OnGamePaused += DisableControls;
        GameManager.OnGameResumed += EnableControls;

        Pickup.OnWeaponUpgradePickup += WeaponUpgrade;
        GameManager.OnMissionStart += ResetPlayerWeapon;
    }

    private void OnDisable()
    {
        InputManager.OnFire -= SetFireInput;
        InputManager.OnControlsEnabled -= EnableControls;
        InputManager.OnControlsDisabled -= DisableControls;

        GameManager.OnGamePaused -= DisableControls;
        GameManager.OnGameResumed -= EnableControls;

        Pickup.OnWeaponUpgradePickup -= WeaponUpgrade;
        GameManager.OnMissionStart -= ResetPlayerWeapon;
    }

    private void Start()
    {
        _fireInput = false;
        CurrentHeat = 0;
        HeatMax = 100;
        OnWeaponHeatInitialized(GUIM.weaponHeatBar, CurrentHeat, HeatMax);
    }

    private void Update()
    {
        if (!GM.isPaused)
        {
            CheckHeat();
        }
    }

    private void CheckHeat()
    {
        if (!IsOverheated)
        {

            if (_controlsEnabled)
            {
                if (_fireInput)
                {
                    CheckForFireInput();
                }

                if (!_fireInput)
                {
                    if (_beamAttack.isBeamActive)
                    {
                        _beamAttack.ResetBeam();
                    }
                }
            }

            TimeSinceLastShot += Time.deltaTime;

            if (IsHeatDecreasing)
            {
                if (CurrentHeat > 0)
                {
                    CurrentHeat -= HeatLossOverTime;
                }
            }
        }

        if (IsOverheated)
        {

            if (CurrentHeat > 0)
            {
                CurrentHeat -= CooldownHeatLoss;
            }
            else
            {
                CurrentHeat = 0;
                IsOverheated = false;
            }
            return;
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
        if (_beamAttack.enabled)
        {
            _beamAttack.isBeamActive = true;
            return;
        }

        if (_playerWeapon.readyToFire)
        {
            _playerWeapon.CheckFireTypes();
            if (!IsWeaponUpgradeActive)
            {
                CurrentHeat += HeatPerShot;
                TimeSinceLastShot = 0;
            }
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
                PulverizerUpgrade();
                break;
        }
        CurrentHeat = 0;
        IsWeaponUpgradeActive = true;
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

    private void PulverizerUpgrade()
    {
        _playerWeapon.enabled = false;
        _beamAttack.enabled = true;
    }

    public void ResetPlayerWeapon()
    {
        OnWeaponUpgradeFinished(GUIM.weaponUpgradeSlider);
        _beamAttack.isBeamActive = false;
        _beamAttack.enabled = false;

        _playerWeapon.enabled = true;
        _playerWeapon.AssignWeaponInfo();
        IsWeaponUpgradeActive = false;
        CurrentHeat = 0;
    }
}
