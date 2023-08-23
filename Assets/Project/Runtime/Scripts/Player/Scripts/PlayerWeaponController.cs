using System;
using System.Collections;
using UnityEngine;

namespace CyberCruiser
{
    public class PlayerWeaponController : GameBehaviour
    {
        [SerializeField] private WeaponSO _baseWeaponSO;
        [SerializeField] private PlayerSoundController _soundController;
        [SerializeField] private PlayerUIManager _playerUIManager;
        [SerializeField] private BeamAttack _beamAttack;
        [SerializeField] private Color _weaponUpgradeSliderColour;
        private Weapon _playerWeapon;
        private WeaponSO _currentWeaponSO;

        #region SO References
        [SerializeField] private IntReference _weaponUpgradeDurationInSeconds;
        [SerializeField] private BoolReference _isGamePausedReference;
        #endregion

        [Header("Heat")]
        [Tooltip("Time player needs to not fire before they start losing heat")]
        [SerializeField] private float _timeBeforeHeatLoss;

        [Tooltip("SO Float Reference of current heat per shot on weapon fire")]
        [SerializeField] private FloatReference _currentHeatPerShotReference;

        #region Pickups Setup
        [Header("Pickups")]

        #region Emergency Arsenal
        [Header("Emergency arsenal")]
        [Tooltip("SO Bool Reference for if player has emergency arsenal")]
        [SerializeField] private BoolReference _doesPlayerHaveEmergencyArsenal;

        [Tooltip("Percentage change of player winning emergency arsenal roll")]
        [SerializeField] private int _emergencyArsenalSuccessPercentage = 10;

        [Tooltip("Game event that fires if player succeeds emergency arsenal roll")]
        [SerializeField] private GameEvent _onEmergencyArsenalSuccess;
        #endregion

        #region Backup System
        [Header("Backup System")]
        [Tooltip("SO Bool Reference for if player has backup system")]
        [SerializeField] private BoolReference _doesPlayerHaveBackupSystem;

        [Tooltip("Game event that fires if player has backup system")]
        [SerializeField] private GameEvent _onBackupSystemActivated;
        #endregion

        #region Thermal Welding
        [Header("Thermal Welding")]
        [Tooltip("SO Bool Reference for in player has thermal welding")]
        [SerializeField] private BoolReference _doesPlayerHaveThermalWelding;

        [Tooltip("Game event that fires if player has thermal welding")]
        [SerializeField] private GameEvent _onThermalWeldingActivated;
        #endregion

        #region Burst Vents
        [Header("Burst Vents")]
        [SerializeField] private BurstVents _burstVents;

        [Tooltip("So Bool Reference for if player has burst vents")]
        [SerializeField] private BoolReference _doesPlayerHaveBurstVents;
        #endregion
        #endregion

        #region Private Fields
        private const int BASE_HEAT_MAX = 100;
        private const float BASE_HEAT_LOSS_PER_FRAME = 0.4f;
        private const float BASE_COOLDOWN_HEAT_LOSS_PER_FRAME = 0.6f;
        private bool _controlsEnabled;
        private float _currentHeat;
        private float _heatLossPerFrame;
        private float _cooldownHeatLossPerFrame;
        private int _heatMax;
        private float _timeSinceLastShot;
        private bool _isOverheated;
        private bool _fireInput;
        private bool _isWeaponUpgradeActive;
        private bool _isHeatDecreasing;
        #endregion

        private Coroutine _weaponUpgradeCoroutine;

        #region Properties
        private float CurrentHeat
        {
            get => _currentHeat;
            set
            {
                _currentHeat = value;

                if (_currentHeat >= _heatMax)
                {
                    _currentHeat = _heatMax;
                    OverHeat();
                }

                else if (_currentHeat < 0)
                {
                    _currentHeat = 0;
                }

                _playerUIManager.ChangeSliderValue(PlayerSliderTypes.Heat, _currentHeat);
            }
        }

        private float TimeSinceLastShot
        {
            get => _timeSinceLastShot;
            set
            {
                _timeSinceLastShot = value;
                _isHeatDecreasing = _timeSinceLastShot >= _timeBeforeHeatLoss;
            }
        }

        private bool IsOverheated
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
            _playerWeapon = GetComponentInChildren<Weapon>();
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

        private void Update()
        {
            if (!_isGamePausedReference.Value)
            {
                CheckOverHeated();
            }
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

            if (!_isWeaponUpgradeActive)
            {
                TimeSinceLastShot += Time.deltaTime;
                HeatReduction();
            }
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


        public void OverheatToMax()
        {
            CurrentHeat = _heatMax;
        }

        private void OverHeat()
        {
            IsOverheated = true;

            PickupChecks();
        }

        private void PickupChecks()
        {
            if (_doesPlayerHaveEmergencyArsenal.Value)
            {
                EmergencyArsenalRoll();
            }

            if (_doesPlayerHaveBackupSystem.Value)
            {
                _onBackupSystemActivated.Raise();
            }

            if (_doesPlayerHaveThermalWelding.Value)
            {
                _onThermalWeldingActivated.Raise();
            }

            if (_doesPlayerHaveBurstVents.Value)
            {
                _burstVents.Burst();
            }
        }

        private void EmergencyArsenalRoll()
        {
            bool emergencyArsenalSuccess = PercentageRoll(_emergencyArsenalSuccessPercentage);

            if (emergencyArsenalSuccess)
            {
                _onEmergencyArsenalSuccess.Raise();
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
                    IncreaseHeatOnWeaponFire();
                    TimeSinceLastShot = 0;
                }
            }
        }

        private void IncreaseHeatOnWeaponFire()
        {
            CurrentHeat += _currentHeatPerShotReference.Value;
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
            OnWeaponUpgradeStart?.Invoke(_weaponUpgradeDurationInSeconds.Value);
            //_playerUIManager.EnableSliderAtMaxValue(PlayerSliderTypes.WeaponUpgrade, _weaponUpgradeDurationInSeconds.Value);

            _playerUIManager.EnableSliderAtMaxValue(PlayerSliderTypes.Heat, _weaponUpgradeDurationInSeconds.Value);
            _playerUIManager.HeatSlider.SetLerpingColour(false, _weaponUpgradeSliderColour);
            _weaponUpgradeCoroutine = StartCoroutine(WeaponUpgradeTimerCoroutine());
        }

        private IEnumerator WeaponUpgradeTimerCoroutine()
        {
            float weaponUpgradeTimer = _weaponUpgradeDurationInSeconds.Value;
            while (weaponUpgradeTimer > 0)
            {
                weaponUpgradeTimer -= Time.deltaTime;
                //_playerUIManager.ChangeSliderValue(PlayerSliderTypes.WeaponUpgrade, weaponUpgradeTimer);

                _playerUIManager.ChangeSliderValue(PlayerSliderTypes.Heat, weaponUpgradeTimer);
                yield return new WaitForSeconds(0.01f);
            }

            //reset player weapon to its original values after upgrade duration is over
            Debug.Log("Upgrade finished");
            _soundController.PlaySound(1);
            OnWeaponUpgradeFinish();
        }

        private void OnWeaponUpgradeFinish()
        {
            //_playerUIManager.DisableSlider(PlayerSliderTypes.WeaponUpgrade);

            _playerUIManager.HeatSlider.SetSliderValues(0, 100);
            _playerUIManager.HeatSlider.SetIsLerpingColour(true);
            ChangeWeapon(_baseWeaponSO);
            _isWeaponUpgradeActive = false;
            DisableBeam();
        }

        private void ChangeWeapon(WeaponSO newWeapon)
        {
            CurrentHeat = 0;
            _currentWeaponSO = newWeapon;
            _playerWeapon.SetWeapon(newWeapon);
            newWeapon.IncrementEquips();
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