using System;
using System.Collections;
using UnityEngine;

namespace CyberCruiser
{
    public class PlayerWeaponController : GameBehaviour
    {
        [SerializeField] private Weapon _playerWeapon;
        [SerializeField] private WeaponSO _baseWeaponSO;
        [SerializeField] private WeaponSO _currentWeaponSO;
        [SerializeField] private PlayerSoundController _soundController;
        [SerializeField] private PlayerUIManager _playerUIManager;
        [SerializeField] private BeamAttack _beamAttack;

        #region Fields
        private bool _controlsEnabled;

        [SerializeField] private IntReference _weaponUpgradeDurationInSeconds;
        [SerializeField] private FloatReference _currentHeatPerShotReference;
        [SerializeField] private BoolReference _isGamePausedReference;

        private const int BASE_HEAT_MAX = 100;
        private const float BASE_HEAT_PER_SHOT = 1.75f;
        private const float BASE_HEAT_LOSS_PER_FRAME = 0.4f;
        private const float BASE_COOLDOWN_HEAT_LOSS_PER_FRAME = 0.6f;

        [Header("Heat")]
        [SerializeField] private float _currentHeat;
        private float _heatLossPerFrame;
        private float _cooldownHeatLossPerFrame;
        [SerializeField] private float _timeBeforeHeatLoss;

        private int _heatMax;
        private float _timeSinceLastShot;
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
                    _playerUIManager.OverheatUI(IsOverheated);
                }

                else if (_currentHeat < 0)
                {
                    _currentHeat = 0;
                }
                _playerUIManager.ChangeSliderValue(PlayerSliderTypes.Heat, _currentHeat);
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
                _playerUIManager.OverheatUI(_isOverheated);
            }
        }
        #endregion

        #region Actions
        public static event Action<int> OnWeaponUpgradeStart = null;
        public static event Action OnShoot = null;
        #endregion

        private void Awake()
        {
            _playerWeapon.SetWeapon(_baseWeaponSO);
            _beamAttack = GetComponentInChildren<BeamAttack>();
        }

        private void OnEnable()
        {
            InputManager.OnFire += SetFireInput;
            GameManager.OnMissionEnd += DisableBeam;
            Pickup.OnWeaponUpgradePickup += WeaponUpgrade;
            InitializeWeapon();
        }

        private void OnDisable()
        {
            InputManager.OnFire -= SetFireInput;
            GameManager.OnMissionEnd -= DisableBeam;
            Pickup.OnWeaponUpgradePickup -= WeaponUpgrade;
        }

        private void Start()
        {
            InitializeWeapon();
        }

        private void InitializeWeapon()
        {
            _currentWeaponSO = _baseWeaponSO;
            _fireInput = false;
            CurrentHeat = 0;
            DisableBeam();
            _heatMax = BASE_HEAT_MAX;
            _heatLossPerFrame = BASE_HEAT_LOSS_PER_FRAME;
            _cooldownHeatLossPerFrame = BASE_COOLDOWN_HEAT_LOSS_PER_FRAME;
            _playerUIManager.EnableSliderAtMaxValue(PlayerSliderTypes.Heat, _heatMax);
        }

        private void Update()
        {
            if (!_isGamePausedReference.Value)
            {
                CheckOverHeated();
            }
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

        private void SetFireInput(bool input)
        {
            if (_controlsEnabled)
            {
                _fireInput = input;
            }
        }

        private void CheckForInput()
        {
            if (_fireInput)
            {
                CheckHoldToFire();

                if (_beamAttack.IsBeamActive)
                {
                    if(_beamAttack.IsBeamFiring == false)
                    {
                        _beamAttack.StartFiring();
                    }
                }
            }

            if (!_fireInput)
            {
                if (_beamAttack.IsBeamActive)
                {
                    _beamAttack.IsBeamFiring = false;
                    _beamAttack.ResetBeam();
                }
            }

            OnShoot?.Invoke();
        } 

        private void CheckHoldToFire()
        {
            if (!_currentWeaponSO.IsWeaponAutomatic)
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

            if (_playerWeapon.ReadyToFire)
            {
                _playerWeapon.CheckFireTypes();
                if (!_isWeaponUpgradeActive)
                {
                    CurrentHeat += _currentHeatPerShotReference.Value;
                    TimeSinceLastShot = 0;
                }
            }
        }

        private void CancelFireInput()
        {
            _fireInput = false;
        }

        public void EnableControls()
        {
            _controlsEnabled = true;
        }

        public void DisableControls()
        {
            SetFireInput(false);
            _controlsEnabled = false;
        }

        private void WeaponUpgrade(WeaponSO upgradeWeapon)
        {
            Debug.Log("weapon upgrade pickup");
            if (_weaponUpgradeCoroutine != null)
            {
                StopCoroutine(_weaponUpgradeCoroutine);
            }

            _soundController.PlaySound(0);
            ChangeWeapon(upgradeWeapon);
            //reset in case a different type of pickup is picked up while an upgrade is currently active
            // need a case for pulverizer

            CurrentHeat = 0;
            _isWeaponUpgradeActive = true;
            _weaponUpgradeCounter = _weaponUpgradeDurationInSeconds.Value;
            OnWeaponUpgradeStart?.Invoke(_weaponUpgradeDurationInSeconds.Value);
            _playerUIManager.EnableSliderAtMaxValue(PlayerSliderTypes.WeaponUpgrade, _weaponUpgradeDurationInSeconds.Value);
            _weaponUpgradeCoroutine = StartCoroutine(WeaponUpgradeTimerCoroutine());
        }

        private IEnumerator WeaponUpgradeTimerCoroutine()
        {
            while (_weaponUpgradeCounter > 0)
            {
                _weaponUpgradeCounter -= Time.deltaTime;
                _playerUIManager.ChangeSliderValue(PlayerSliderTypes.WeaponUpgrade, _weaponUpgradeCounter);
                yield return new WaitForSeconds(0.01f);
            }

            //reset player weapon to its original values after upgrade duration is over
            Debug.Log("Upgrade finished");
            _soundController.PlaySound(1);
            OnWeaponUpgradeFinish();
        }

        private void OnWeaponUpgradeFinish()
        {
            _playerUIManager.DisableSlider(PlayerSliderTypes.WeaponUpgrade);
            ChangeWeapon(_baseWeaponSO);
            _isWeaponUpgradeActive = false;
            DisableBeam();
        }

        private void ChangeWeapon(WeaponSO newWeapon)
        {
            CurrentHeat = 0;
            _currentWeaponSO = newWeapon;
            _playerWeapon.SetWeapon(newWeapon);
        }

        private void PulverizerUpgrade()
        {
            //_baseWeapon.gameObject.SetActive(false);
            _beamAttack.enabled = true;
        }

        public void DisableBeam()
        {
            _beamAttack.StopFiring();
            _beamAttack.enabled = false;
        }   
    }
}