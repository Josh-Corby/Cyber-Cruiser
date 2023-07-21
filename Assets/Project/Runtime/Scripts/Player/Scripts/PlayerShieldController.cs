using System;
using System.Collections;
using UnityEngine;

namespace CyberCruiser
{
    public class PlayerShieldController : ShieldControllerBase
    {
        [SerializeField] PlayerUIManager _playerUIManager;

        #region Fields
        [Header("Player Shield Info")]
        [Tooltip("Total duration of player shield activation")]
        [SerializeField] private int _shieldActiveDuration;
        [SerializeField] private float _shieldActiveTimer;
        private bool _controlsEnabled;
        #endregion

        #region Pickups
        [Header("Pickups")]

        #region Pulse Detonator
        [Header("Pulse Detonator")]
        [SerializeField] private PulseDetonator _pulseDetonator;

        [Tooltip("SO Bool Reference for if player has pulse detonator")]
        [SerializeField] private BoolReference _doesPlayerHavePulseDetonator;
        #endregion

        #region Reflector Shield
        [Header("Reflector Shield")]
        [Tooltip("SO Bool Reference for if player shield is reflecting")]
        [SerializeField] private BoolReference _doesPlayerShieldReflect;
        #endregion

        #region Signal Beacon
        [Header("Signal Beacon")]
        [Tooltip("SO Bool Reference for if player has signal beacon")]
        [SerializeField] private BoolReference _doesPlayerHaveSignalBeacon;

        [Tooltip("Percentage chance of player winning signal beacon roll")]
        [SerializeField] private int _signalBeaconSuccessPercentage = 33;

        [SerializeField] private GameEvent _onSignalBeamSuccess;
        #endregion

        #region Invisibility Shield
        [Header("Invisibility Shield")]
        [Tooltip("SO Bool Reference for if player has invisibility shield")]
        [SerializeField] private BoolReference _doesPlayerHaveInvisibilityShield;

        [Tooltip("SO Value for if player is currently invisible")]
        [SerializeField] private BoolValue _isPlayerInvisible;
        #endregion

        #region Shield Generator
        [Header("Shield Generator")]
        [Tooltip("SO Bool Reference for if player has shield generator")]
        [SerializeField] private BoolReference _doesPlayerHaveShieldGenerator;
        #endregion

        #region Time Stop
        [Header("Time Stop")]
        [Tooltip("SO Bool Reference for if player has time stop")]
        [SerializeField] private BoolReference _doesPlayerHaveTimeStop;

        [Tooltip("SO Bool Value for if time is stopped")]
        [SerializeField] private BoolValue _isTimeStopped;
        #endregion
        #endregion

        #region Properties
        private float ShieldActiveTimer
        {
            get => _shieldActiveTimer;
            set
            {
                _shieldActiveTimer = value;
                _playerUIManager.ChangeSliderValue(PlayerSliderTypes.Shield, _shieldActiveTimer);
            }
        }

        protected override bool IsShieldsActive { get => _shieldsActive; set => base.IsShieldsActive = value; }

        private bool IsPlayerInvisible { get => _isPlayerInvisible.Value; set => _isPlayerInvisible.Value = value; }

        private bool IsTimeStopped { get => _isTimeStopped.Value; set => _isTimeStopped.Value = value; }
        #endregion

        private Coroutine _shieldGeneratorRoutine;

        #region Actions
        public static event Action OnPlayerShieldsActivated = null;
        #endregion

        private void OnEnable()
        {
            InputManager.OnShield += CheckShieldsState;
            GameManager.OnMissionEnd += DeactivateShields;
        }

        private void OnDisable()
        {
            InputManager.OnShield -= CheckShieldsState;
            GameManager.OnMissionEnd -= DeactivateShields;
        }

        private void Update()
        {
            if (_doesPlayerHaveShieldGenerator.Value)
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
                    DeactivateShields();
                }
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

        private void CheckShieldsState()
        {
            if (!_controlsEnabled)
            {
                return;
            }

            if (IsShieldsActive || IsPlayerInvisible)
            {
                return;
            }

            if (PlayerManagerInstance.ComparePlasmaToCost())
            {
                CheckShieldPickups();
            }
        }

        //a way to activate player shields from other classes, or from unity events
        public void ActivateShield()
        {
            CheckShieldPickups();
        }

        //activate whatever shield effect is currently equipped
        protected override void CheckShieldPickups()
        {
            if (_doesPlayerHavePulseDetonator.Value)
            {
                _pulseDetonator.Detonate();
                return;
            }

            if (_doesPlayerHaveInvisibilityShield.Value)
            {
                IsPlayerInvisible = true;
                EnableShield();
                return;
            }

            if (_doesPlayerHaveSignalBeacon.Value)
            {
                SignalBeaconRoll();
            }

            if (_doesPlayerHaveShieldGenerator.Value)
            {
                EnableShieldGenerator();
                return;
            }

            if (_doesPlayerHaveTimeStop.Value)
            {
                IsTimeStopped = true;
                EnableShield();
            }

            //if function isn't returned, activate base shield
            IsShieldsActive = true;
            PlayerManagerInstance.IsPlayerColliderEnabled = false;
            EnableShield();
        }

        #region Pickup Shield Effects
        private void SignalBeaconRoll()
        {
            bool signalBeaconSuccess = PercentageRoll(_signalBeaconSuccessPercentage);
            if (signalBeaconSuccess)
            {
                _onSignalBeamSuccess.Raise();
            }
        }

        #region Shield Generator
        private void EnableShieldGenerator()
        {
            if (_shieldGeneratorRoutine != null)
            {
                StopCoroutine(_shieldGeneratorRoutine);
            }
            Debug.Log("Shield generator started");
            IsShieldsActive = true;
            PlayerManagerInstance.IsPlayerColliderEnabled = false;
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
            ResetShieldTimer();
            ToggleSliderUI(true);
            OnPlayerShieldsActivated?.Invoke();
        }

        protected override void DeactivateShields()
        {
            ToggleSliderUI(false);

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

        public override void ProcessCollision(GameObject collider, Vector2 collisionPoint)
        {
            if (collider.GetComponent<Boss>())
            {
                return;
            }

            else if (collider.TryGetComponent<Pickup>(out var pickup))
            {
                pickup.PickupEffect();
                Destroy(pickup.gameObject);
                return;
            }

            if (collider.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.Damage(ShieldCollisionDamage);
                if (!_isShieldImmuneToDamage)
                {
                    ReduceShields(1);
                }

                if (_collisionParticles != null)
                {
                    GameObject collisionParticles = Instantiate(_collisionParticles, collisionPoint, Quaternion.identity);
                    collisionParticles.transform.parent = null;
                }
            }

            else if (collider.TryGetComponent<ShieldControllerBase>(out var shield))
            {
                shield.ReduceShields(ShieldCollisionDamage);
                if (!_isShieldImmuneToDamage)
                {
                    ReduceShields(shield.ShieldCollisionDamage);
                }
            }

            else if (collider.TryGetComponent<Bullet>(out var bullet))
            {
                if (!_isShieldImmuneToDamage)
                {
                    ReduceShields(bullet.Damage);
                }

                if (_doesPlayerShieldReflect.Value)
                {
                    ReflectProjectile(bullet);
                    return;
                }

                Destroy(bullet.gameObject);
            }
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
            if (value)
            {
                _playerUIManager.EnableSliderAtMaxValue(PlayerSliderTypes.Shield, _shieldActiveDuration);
            }

            if (!value)
            {
                _playerUIManager.DisableSlider(PlayerSliderTypes.Shield);
            }
        }

    }
}