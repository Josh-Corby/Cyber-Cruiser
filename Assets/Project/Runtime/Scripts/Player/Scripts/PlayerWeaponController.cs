using CyberCruiser.Audio;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CyberCruiser
{
    public class PlayerWeaponController : GameBehaviour
    {
        [Header("Managers")]
        [SerializeField] private PlayerAddOnManager _addOnManager;
        [SerializeField] private PlayerSoundController _soundController;
        [SerializeField] private PlayerUIManager _playerUIManager;

        [Header("OverheatUI")]
        private Image _overheatFlame;
        [SerializeField] private Image _pcOverheatFlame;
        [SerializeField] private Image _mobileOverheatFlame;

        [SerializeField] private WeaponSO _baseWeaponSO;
        [SerializeField] private WeaponSO _chainLightingWeaponSO;
        [SerializeField] private BeamAttack _beamAttack;
        [SerializeField] private ClipInfo _overheatClip;

        private Weapon _playerWeapon;
        [SerializeField] private WeaponSO _currentWeaponSO;
        [SerializeField] private WeaponSO _pulverizerData;

        #region SO References
        [Header("SO References")]
        [SerializeField] private IntReference _weaponUpgradeDurationInSeconds;
        [SerializeField] private BoolReference _isGamePausedReference;
        [SerializeField] private BoolReference _isPlatformPC;
        #endregion

        [Header("Heat")]
        [Tooltip("Time player needs to not fire before they start losing heat")]
        [SerializeField] private float _timeBeforeHeatLoss;
        [Tooltip("Percentage change of player winning emergency arsenal roll")]
        [SerializeField] private int _emergencyArsenalSuccessPercentage = 10;
        [Tooltip("SO Float Reference of current heat per shot on weapon fire")]
        [SerializeField] private FloatReference _currentHeatPerShotReference;
        [SerializeField] private BurstVents _burstVents;
        [SerializeField] private ParticleSystem _OverheatParticles;
  
        #region Fields
        private const int BASE_HEAT_MAX = 100;
        private const float BASE_HEAT_LOSS_PER_FRAME = 0.4f;
        private const float BASE_COOLDOWN_HEAT_LOSS_PER_FRAME = 0.6f;

        private int _heatMax;
        private float _currentHeat;
        private float _heatLossPerFrame;
        private float _cooldownHeatLossPerFrame;
        private float _timeSinceLastShot;
        private bool _isOverheated;
        private bool _fireInput;
        private bool _isWeaponUpgradeActive;
        private bool _isHeatDecreasing;
        private bool _controlsEnabled;
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
                _playerUIManager.ToggleHeatSliderFill(_isOverheated);
            }
        }
        #endregion

        #region Actions
        public static event Action OnShoot = null;
        public static event Action OnThermalWeldingActivated = null;
        public static event Action OnBackupSystemActivated = null;
        public static event Action OnEmergencyArsenalActivated = null;
        public static event Action<int> OnWeaponUpgradeStart = null;
        #endregion

        private void Awake()
        {
            _playerWeapon = GetComponentInChildren<Weapon>();
            _playerWeapon.SetWeapon(_baseWeaponSO);
            _beamAttack = GetComponentInChildren<BeamAttack>();

            if(_isPlatformPC.Value)
            {
                _overheatFlame = _pcOverheatFlame;
            }
            else
            {
                _overheatFlame = _mobileOverheatFlame;
            }
        }

        private void OnEnable()
        {
            InputManager.OnFire += SetFireInput;
            GameManager.OnMissionEnd += ResetWeapon;
            Pickup.OnWeaponUpgradePickup += WeaponUpgrade;
            //Pickup.OnBossPickup += CheckIfAddOnIsChainLightning;
            InitializeWeapon();
        }

        private void OnDisable()
        {
            InputManager.OnFire -= SetFireInput;
            GameManager.OnMissionEnd -= ResetWeapon;
            Pickup.OnWeaponUpgradePickup -= WeaponUpgrade;
            //Pickup.OnBossPickup -= CheckIfAddOnIsChainLightning;
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
            _heatMax = BASE_HEAT_MAX;
            CurrentHeat = 0;
            DisableBeam();
            _heatLossPerFrame = BASE_HEAT_LOSS_PER_FRAME;
            _cooldownHeatLossPerFrame = BASE_COOLDOWN_HEAT_LOSS_PER_FRAME;
            _isWeaponUpgradeActive = false;
            _playerUIManager.EnableSliderAtValue(PlayerSliderTypes.Heat, _heatMax, _currentHeat);
            _overheatFlame.enabled = false;
        }

        private void ResetWeapon()
        {
            DisableBeam();
            ChangeWeapon(_baseWeaponSO);
            _isWeaponUpgradeActive = false;
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
                _overheatFlame.enabled = false;
                _playerUIManager.ToggleHeatSliderFill(false);
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
            _soundController.PlayNewClip(_overheatClip);
            PickupChecks();
            _overheatFlame.enabled = true;
            _playerUIManager.ToggleHeatSliderFill(true);
            _OverheatParticles.Play();
        }

        private void PickupChecks()
        {
            if (_addOnManager.BackupSystem.DoesPlayerHave)
                OnBackupSystemActivated?.Invoke();

            if (_addOnManager.BurstVents.DoesPlayerHave)
                _burstVents.Burst();

            if (_addOnManager.EmergencyArsenal.DoesPlayerHave)
                EmergencyArsenalRoll();

            if (_addOnManager.ThermalWelding.DoesPlayerHave)
                OnThermalWeldingActivated?.Invoke();
        }

        private void EmergencyArsenalRoll()
        {
            bool emergencyArsenalSuccess = PercentageRoll(_emergencyArsenalSuccessPercentage);
            if (emergencyArsenalSuccess)
                OnEmergencyArsenalActivated?.Invoke();
        }

        private void CheckIfAddOnIsChainLightning(PickupInfo info)
        {
            if(info.Name != _addOnManager.ChainLightning.Info.Name)
            {
                DisableChainLightning();
            }
        }

        public void EnableChainLightning()
        {
            ChangeWeapon(_chainLightingWeaponSO);
        }

        private void DisableChainLightning()
        {
            ChangeWeapon(_baseWeaponSO);
        }

        public void SetFireInput(bool input)
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

                OnShoot?.Invoke();
            }

            if (!_fireInput)
            {
                if (_beamAttack.IsBeamActive)
                {
                    _beamAttack.IsBeamFiring = false;
                    _beamAttack.ResetBeam();
                    _beamAttack.StopFiring();
                }
            }
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

            if(upgradeWeapon == null)
            {
                Debug.Log("pulverizer upgrade");
                PulverizerUpgrade();
                _pulverizerData.IncrementEquips();
            }

            else
            {
                ChangeWeapon(upgradeWeapon);
            }

            CurrentHeat = 0;
            _isWeaponUpgradeActive = true;
            OnWeaponUpgradeStart?.Invoke(_weaponUpgradeDurationInSeconds.Value);

            _playerUIManager.EnableSliderAtMaxValue(PlayerSliderTypes.Heat, _weaponUpgradeDurationInSeconds.Value);
            _playerUIManager.ToggleWeaponPackSliderFill(true);
            _weaponUpgradeCoroutine = StartCoroutine(WeaponUpgradeTimerCoroutine());
        }

        private IEnumerator WeaponUpgradeTimerCoroutine()
        {
            float weaponUpgradeTimer = _weaponUpgradeDurationInSeconds.Value;
            while (weaponUpgradeTimer > 0)
            {
                weaponUpgradeTimer -= Time.deltaTime;

                _playerUIManager.ChangeSliderValue(PlayerSliderTypes.Heat, weaponUpgradeTimer);
                yield return new WaitForSeconds(0.01f);
            }

            OnWeaponUpgradeFinish();
        }

        private void OnWeaponUpgradeFinish()
        {
            _soundController.PlaySound(1);
            ChangeWeapon(_baseWeaponSO);
            _isWeaponUpgradeActive = false;
            _playerUIManager.ToggleHeatSliderFill(false);
            _playerUIManager.EnableSliderAtValue(PlayerSliderTypes.Heat, _heatMax, _currentHeat);
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
            _beamAttack.enabled = true;
        }

        public void DisableBeam()
        {
            _beamAttack.StopFiring();
            _beamAttack._audioSource.Stop();
            _beamAttack.enabled = false;
        }   
    }
}