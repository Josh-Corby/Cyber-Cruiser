using System;
using System.Collections;
using UnityEngine;

namespace CyberCruiser
{
    public class PlayerWeaponController : GameBehaviour
    {
        [SerializeField] private PlayerSoundController _soundController;  

        #region Weapon References
        [SerializeField] private Weapon _currentWeapon;

        [SerializeField] private Weapon _baseWeapon;
        [SerializeField] private Weapon _chainLightning;
        [SerializeField] private Weapon _bFG;
        [SerializeField] private BeamAttack _beamAttack;
        #endregion

        #region Fields
        private bool _controlsEnabled;

        private const int BASE_HEAT_MAX = 100;
        private const float BASE_HEAT_PER_SHOT = 1.75f;
        private const int BASE_UPGRADE_DURATION = 10;
        private const float BASE_HEAT_LOSS_PER_FRAME = 0.4f;
        private const float BASE_COOLDOWN_HEAT_LOSS_PER_FRAME = 0.6f;

        [SerializeField] private float _currentHeat;
        private float _heatPerShot;
        private float _heatLossPerFrame;
        private float _cooldownHeatLossPerFrame;
        [SerializeField] private float _timeBeforeHeatLoss;

        private int _heatMax;
        private float _timeSinceLastShot;
        private int _weaponUpgradeDuration;
        private float _weaponUpgradeCounter;

        private bool _isOverheated;
        private bool _isHoming;
        private bool _fireInput;
        private bool _isWeaponUpgradeActive;
        private bool _isHeatDecreasing;

        private bool _isWeaponFiringHomingProjectiles;
        private Coroutine _weaponUpgradeCoroutine;
        #endregion

        #region Properties
        public float CurrentHeat
        {
            get => _currentHeat;
            private set
            {
                _currentHeat = value;

                if (_currentHeat >= _heatMax)
                {
                    _currentHeat = _heatMax;
                    IsOverheated = true;
                    OnOverheatStatusChange(true);
                }

                else if (_currentHeat < 0)
                {
                    _currentHeat = 0;
                }
                OnHeatChange?.Invoke(_currentHeat);
            }
        }

        public float TimeSinceLastShot
        {
            get => _timeSinceLastShot;
            set
            {
                _timeSinceLastShot = value;
                _isHeatDecreasing = _timeSinceLastShot >= _timeBeforeHeatLoss;
            }
        }

        public bool IsOverheated
        {
            get => _isOverheated;
            set
            {
                _isOverheated = value;
                OnOverheatStatusChange?.Invoke(_isOverheated);
            }
        }
        #endregion

        #region Actions
        public static event Action<int> OnWeaponUpgradeStart = null;
        public static event Action<float> OnWeaponUpgradeTimerTick = null;
        public static event Action OnWeaponUpgradeFinished = null;
        public static event Action<int> OnWeaponHeatInitialized = null;
        public static event Action<float> OnHeatChange = null;
        public static event Action<bool> OnOverheatStatusChange = null;
        public static event Action OnShoot = null;
        #endregion

        private void Awake()
        {
            _currentWeapon = _baseWeapon;
            _beamAttack = GetComponentInChildren<BeamAttack>();
        }

        private void OnEnable()
        {
            InputManager.OnFire += SetFireInput;
            GameManager.OnMissionEnd += ResetPlayerWeapon;
            Pickup.OnWeaponUpgradePickup += WeaponUpgrade;
            ResetPlayerWeapon();
        }

        private void OnDisable()
        {
            InputManager.OnFire -= SetFireInput;
            GameManager.OnMissionEnd -= ResetPlayerWeapon;
            Pickup.OnWeaponUpgradePickup -= WeaponUpgrade;
        }

        private void Start()
        {
            InitializeWeapon();
        }

        private void InitializeWeapon()
        {
            _fireInput = false;
            CurrentHeat = 0;
            _heatMax = BASE_HEAT_MAX;
            _heatPerShot = BASE_HEAT_PER_SHOT;
            _heatLossPerFrame = BASE_HEAT_LOSS_PER_FRAME;
            _cooldownHeatLossPerFrame = BASE_COOLDOWN_HEAT_LOSS_PER_FRAME;

            if (PlayerAddOnManagerInstance.IsHydrocoolantActive)
            {
                _heatPerShot -= 0.25f;
            }

            _weaponUpgradeDuration = BASE_UPGRADE_DURATION;

            if (PlayerAddOnManagerInstance.IsBatteryPackActive)
            {
                _weaponUpgradeDuration += 5;
            }

            OnWeaponHeatInitialized?.Invoke(_heatMax);
        }

        private void Update()
        {
            CheckOverHeated();
        }

        private void CheckOverHeated()
        {
            if (!IsOverheated)
            {
                CheckControlsEnabled();
                return;
            }

            if (IsOverheated)
            {
                Overheating();
            }
        }

        private void Overheating()
        {
            if (CurrentHeat > 0)
            {
                CurrentHeat -= _cooldownHeatLossPerFrame;
            }
            else
            {
                CurrentHeat = 0;
                IsOverheated = false;
            }
        }

        private void CheckControlsEnabled()
        {
            if (_controlsEnabled)
            {
                CheckForInput();
            }

            TimeSinceLastShot += Time.deltaTime;
            HeatReduction();
        }

        private void HeatReduction()
        {
            if (_isHeatDecreasing)
            {
                if (CurrentHeat > 0)
                {
                    CurrentHeat -= _heatLossPerFrame;
                }
            }
        }

        private void CheckForInput()
        {
            if (_fireInput)
            {
                CheckHoldToFire();
            }

            if (!_fireInput)
            {
                if (_beamAttack.IsBeamActive)
                {
                    _beamAttack.ResetBeam();
                }
            }
            OnShoot?.Invoke();
        }

        private void SetFireInput(bool input)
        {
            if(_controlsEnabled)
            {
                _fireInput = input;
            }
        }

        private void CheckHoldToFire()
        {
            if (!_currentWeapon.CurrentStats.IsWeaponAutomatic)
            {
                CancelFireInput();
            }
            FireWeapon();
        }

        private void FireWeapon()
        {
            if (_beamAttack.enabled)
            {
                _beamAttack.EnableBeam();
                return;
            }

            if (_currentWeapon.ReadyToFire)
            {
                _currentWeapon.CheckFireTypes();
                if (!_isWeaponUpgradeActive)
                {
                    CurrentHeat += _heatPerShot;
                    TimeSinceLastShot = 0;
                }
            }
        }

        private void CancelFireInput()
        {
            _fireInput = false;
        }

        private void ToggleControls(bool value)
        {
            if (value)
            {
                DisableControls();
            }
            else
            {
                EnableControls();
            }
        }

        public void EnableControls()
        {
            _controlsEnabled = true;
        }

        public void DisableControls()
        {
            _controlsEnabled = false;
        }

        private void WeaponUpgrade(WeaponUpgradeType upgradeType)
        {
            if (_weaponUpgradeCoroutine != null)
            {
                StopCoroutine(_weaponUpgradeCoroutine);
            }
            _weaponUpgradeCoroutine = StartCoroutine(WeaponUpgradeCoroutine(upgradeType));
        }

        private IEnumerator WeaponUpgradeCoroutine(WeaponUpgradeType upgradeType)
        {
            _soundController.PlaySound(0);

            //reset in case a different type of pickup is picked up while an upgrade is currently active
            ResetPlayerWeapon();

            switch (upgradeType)
            {
                case WeaponUpgradeType.Scatter_Fixed:
                case WeaponUpgradeType.Scatter_Random:
                    ScatterUpgrade(upgradeType);
                    break;
                case WeaponUpgradeType.Pulverizer:
                    PulverizerUpgrade();
                    break;
                case WeaponUpgradeType.Homing:
                    _isWeaponFiringHomingProjectiles = true;
                    break;
                case WeaponUpgradeType.ChainLightning:
                    _currentWeapon = _chainLightning;
                    break;
                case WeaponUpgradeType.BFG:
                    _currentWeapon = _bFG;
                    break;
            }

            CurrentHeat = 0;
            _isWeaponUpgradeActive = true;
            _weaponUpgradeCounter = _weaponUpgradeDuration;

            OnWeaponUpgradeStart?.Invoke(_weaponUpgradeDuration);
            while (_weaponUpgradeCounter > 0)
            {
                _weaponUpgradeCounter -= Time.deltaTime;
                OnWeaponUpgradeTimerTick?.Invoke(_weaponUpgradeCounter);
                yield return new WaitForSeconds(0.01f);
            }

            //reset player weapon to its original values after upgrade duration is over
            _soundController.PlaySound(1);
            ResetPlayerWeapon();
        }

        private void PulverizerUpgrade()
        {
            _currentWeapon.enabled = false;
            _beamAttack.enabled = true;
        }

        public void ScatterUpgrade(WeaponUpgradeType scatterType)
        {
            switch (scatterType)
            {
                case WeaponUpgradeType.Scatter_Fixed:
                    _currentWeapon.CurrentStats.IsMultiFireSpreadRandom = false;
                    break;
                case WeaponUpgradeType.Scatter_Random:
                    _currentWeapon.CurrentStats.IsMultiFireSpreadRandom = true;
                    break;
            }

            _currentWeapon.CurrentStats.IsWeaponMultiFire = true;
            _currentWeapon.CurrentStats.MultiFireShots = 3;
            _currentWeapon.CurrentStats.DoesWeaponUseSpread = true;
            _currentWeapon.CurrentStats.SpreadHalfAngle = 30;
        }

        public void ResetPlayerWeapon()
        {
            WeaponUpgradeFinished();
            _beamAttack.DisableBeam();
            _beamAttack.enabled = false;
        }

        private void WeaponUpgradeFinished()
        {    
            OnWeaponUpgradeFinished?.Invoke();
            CurrentHeat = 0;
            _currentWeapon.enabled = true;
            _currentWeapon.ResetWeapon();
            _currentWeapon = _baseWeapon;
            _isWeaponUpgradeActive = false;
        }
    }
}