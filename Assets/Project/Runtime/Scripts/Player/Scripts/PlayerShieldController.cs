using System;
using System.Collections;
using UnityEngine;

namespace CyberCruiser
{
    public class PlayerShieldController : ShieldControllerBase
    {
        [Header("Player Shield Controller")]
        [SerializeField] private PlayerAddOnManager _addOnManager;
        [SerializeField] PlayerUIManager _playerUIManager;
        [SerializeField] private PlayerManager _playerManager;

        #region Fields
        [Header("Player Shield Info")]
        [Tooltip("Total duration of player shield activation")]
        [SerializeField] private int _shieldActiveDuration;
        [SerializeField] private float _shieldActiveTimer;
        [SerializeField] private IntReference _ramDamage;
        private bool _controlsEnabled;
        #endregion

        #region Pickups
        [Header("Pickups")]
        [SerializeField] private PulseDetonator _pulseDetonator;
        [Tooltip("Percentage chance of player winning signal beacon roll")]
        [SerializeField] private int _signalBeaconSuccessPercentage = 33;
        [Tooltip("SO Value for if player is currently invisible")]
        [SerializeField] private BoolValue _isPlayerInvisible;
        [Tooltip("SO Bool Value for if time is stopped")]
        [SerializeField] private BoolValue _isTimeStopped;
        #endregion

        #region Properties
        private float ShieldActiveTimer
        {
            get => _shieldActiveTimer;
            set
            {
                _shieldActiveTimer = value;
                _playerUIManager.SetShieldSliderProgress(GetShieldDurationInterped());
            }
        }

        protected override bool IsShieldsActive { get => _shieldsActive; set => base.IsShieldsActive = value; }

        public bool PublicIsShieldsActive { get => _shieldsActive; }

        private bool IsPlayerInvisible { get => _isPlayerInvisible.Value; set => _isPlayerInvisible.Value = value; }

        private bool IsTimeStopped { get => _isTimeStopped.Value; set => _isTimeStopped.Value = value; }
        #endregion

        private Coroutine _shieldGeneratorRoutine;

        #region Actions
        public static event Action OnPlayerShieldsActivated = null;
        public static event Action OnSignalBeaconActivated = null;
        #endregion

        protected override void Awake()
        {
            base.Awake();
            _playerManager = GetComponentInParent<PlayerManager>();
        }

        private void OnEnable()
        {
            InputManager.OnShield += CheckShieldsState;
            GameManager.OnMissionEnd += DeactivateShields;
            PlayerWeaponController.OnBackupSystemActivated += ActivateShield;
            _playerUIManager.ToggleGreenShieldDisplay(_playerManager.CanAffordShield());
        }

        private void OnDisable()
        {
            InputManager.OnShield -= CheckShieldsState;
            GameManager.OnMissionEnd -= DeactivateShields;
            PlayerWeaponController.OnBackupSystemActivated -= ActivateShield;
        }

        private void Update()
        {
            if (_addOnManager.ShieldGenerator.DoesPlayerHave)
            {
                return;
            }

            if (IsShieldsActive || IsPlayerInvisible || IsTimeStopped)
            {
                if (ShieldActiveTimer >= 0)
                {
                    ShieldActiveTimer -= Time.deltaTime;
                }

                else
                {
                    _soundController.PlayNewClip(_shieldDisableClip);
                    DeactivateShields();
                    return;
                }

                //shield about to disable visuals
                //if(ShieldActiveTimer >= 0.95 && ShieldActiveTimer <= 1.05)
                //{
                //    PlayCollisionParticles(transform.position);
                //}
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

        public void CheckShieldsState_Public()
        {
            Debug.Log("Shield button pressed");
            CheckShieldsState();
        }

        private void CheckShieldsState()
        {
            if (!_controlsEnabled)
            {
                return;
            }

            if (_addOnManager.ShieldGenerator.DoesPlayerHave && IsShieldsActive)
            {
                DisableShieldGenerator();
                return;
            }

            if (IsShieldsActive || IsPlayerInvisible)
            {
                return;
            }

            if (PlayerManagerInstance.ComparePlasmaToCost())
            {
                ActivateShields();
            }
        }

        //a way to activate player shields from other classes, or from unity events
        public void ActivateShield()
        {
            ActivateShields();
        }

        public override void ActivateShields()
        {
            if (_addOnManager.InvisibilityShield.DoesPlayerHave)
                IsPlayerInvisible = true;

            if (_addOnManager.PulseDetonator.DoesPlayerHave)
                _pulseDetonator.Detonate();

            if (_addOnManager.SignalBeacon.DoesPlayerHave)
                SignalBeaconRoll(); 

            if (_addOnManager.TimeStop.DoesPlayerHave)
                IsTimeStopped = true;

            if (_addOnManager.ShieldGenerator.DoesPlayerHave)
                EnableShieldGenerator();

            EnableShield();
        }

        #region Pickup Shield Effects
        private void SignalBeaconRoll()
        {
            bool signalBeaconSuccess = PercentageRoll(_signalBeaconSuccessPercentage);
            if (signalBeaconSuccess)
                OnSignalBeaconActivated();
        }

        #region Shield Generator
        private void ClearShieldGeneratorCoroutine()
        {
            if (_shieldGeneratorRoutine != null)
            {
                StopCoroutine(_shieldGeneratorRoutine);
            }
        }
        private void DisableShieldGenerator()
        {
            Debug.Log("Disabling shield generator");
            ClearShieldGeneratorCoroutine();
            DeactivateShield();
        }
        private void EnableShieldGenerator()
        {
            ClearShieldGeneratorCoroutine();
            _shieldGeneratorRoutine = StartCoroutine(nameof(ShieldGeneratorCoroutine));   
        }

        private IEnumerator ShieldGeneratorCoroutine()
        {
            yield return new WaitForSeconds(1);

            while (IsShieldsActive)
            {
                if (!PlayerManagerInstance.ComparePlasmaToCost())
                {
                    DeactivateShields();
                    StopCoroutine(_shieldGeneratorRoutine);
                }

                yield return new WaitForSeconds(1f);
            }
        }
        #endregion

        #endregion

        //enable the shield object
        private void EnableShield()
        {
            IsShieldsActive = true;
            PlayerManagerInstance.IsPlayerColliderEnabled = false;
            _soundController.PlayNewClip(_shieldEnableClip);
            ResetShieldTimer();
            ToggleSliderUI(true);
            OnPlayerShieldsActivated?.Invoke();

            _playerUIManager.ToggleGreenShieldDisplay(true);
        }

        public void DeactivateShield()
        {
            Debug.Log("Shields deactivated");
            DeactivateShields();
        }

        protected override void DeactivateShields()
        {
            ToggleSliderUI(false);
            _playerUIManager.ToggleGreenShieldDisplay(_playerManager.CanAffordShield());

            if (IsPlayerInvisible)
            {
                IsPlayerInvisible = false;
            }

            if (IsTimeStopped)
            {
                IsTimeStopped = false;
            }

            else
            {
                IsShieldsActive = false;
                PlayerManagerInstance.IsPlayerColliderEnabled = true;
            }
        }

        protected void PlayCollisionParticles(Vector3 spawnVector)
        {
            if (_collisionParticles != null)
            {
                GameObject collisionParticles = Instantiate(_collisionParticles, spawnVector, Quaternion.identity);
                collisionParticles.transform.parent = null;
            }
        }

        public override void ProcessCollision(GameObject collider, Vector2 collisionPoint)
        {
            if (collider.GetComponent<Boss>())
            {
                return;
            }

            else if (collider.TryGetComponent<Pickup>(out var pickup))
            {
                _playerManager.OnPickup(pickup);
                return;
            }

            float shieldDamage = 0;
            if (collider.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.Damage(_ramDamage.Value, null);
                shieldDamage = 1;
 
                PlayCollisionParticles(collisionPoint);
            }

            else if (collider.TryGetComponent<ShieldControllerBase>(out var shield))
            {
                shield.ReduceShields(_ramDamage.Value);

                shieldDamage = shield.ShieldCollisionDamage;
            }

            else if (collider.TryGetComponent<Bullet>(out var bullet))
            {
                _soundController.PlayNewClip(_shieldDamageClip);

                PlayCollisionParticles(collisionPoint);
                shieldDamage = bullet.Damage;
                
                if (_addOnManager.ReflectorShield.DoesPlayerHave)
                {
                    ReflectProjectile(bullet);
                    return;
                }

                Destroy(bullet.gameObject);
            }

            if (!_isShieldImmuneToDamage)
            {
                ReduceShields(shieldDamage);               
            }

            _playerManager.RetaliationMatrixCheck();

        }

        public override void ReduceShields(float damage)
        {
            ShieldActiveTimer -= damage;
        }

        private void ResetShieldTimer()
        {
            ShieldActiveTimer = _shieldActiveDuration;
        }

        private void ToggleSliderUI(bool value)
        {
            _playerUIManager.ToggleHealthSliderFill(value);
        }

        private float GetShieldDurationInterped()
        {
            return _shieldActiveTimer / _shieldActiveDuration;
        }
    }
}