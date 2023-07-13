using System;
using UnityEngine;

namespace CyberCruiser
{
    public class PlayerShieldController : ShieldControllerBase
    {
        [SerializeField] PlayerUIManager _playerUIManager;
        private PulseDetonator _pulseDetonator;

        #region Fields
        [Tooltip("SO Bool Reference for if player has pulse detonator")]
        [SerializeField] private BoolReference _doesPlayerHavePulseDetonator;

        [Tooltip("SO Bool Reference for if player shield is reflecting")]
        [SerializeField] private BoolReference _doesPlayerShieldReflect;

        [Tooltip("SO Bool Reference for if player has signal beacon")]
        [SerializeField] private BoolReference _doesPlayerHaveSignalBeacon;

        [Tooltip("Percentage chance of player winning signal beacon roll")]
        [SerializeField] private int _signalBeaconSuccessPercentage =33;

        [Tooltip("Total duration of player shield activation")]
        [SerializeField] private int _shieldActiveDuration;
        private float _shieldActiveTimer;
        private bool _controlsEnabled;
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

        protected override bool IsShieldsActive
        {
            get => _shieldsActive;
            set
            {
                base.IsShieldsActive = value;
                if (!_shieldsActive)
                {
                    _playerUIManager.DisableSlider(PlayerSliderTypes.Shield);
                }

                if (_shieldsActive)
                {
                    _playerUIManager.EnableSliderAtMaxValue(PlayerSliderTypes.Shield, _shieldActiveDuration);
                    OnPlayerShieldsActivated?.Invoke();
                }
            }
        }
        #endregion

        #region Actions
        public static event Action OnPlayerShieldsActivated = null;
        #endregion

        [SerializeField] private GameEvent _onSignalBeamSuccess;

        protected override void Awake()
        {
            base.Awake();
            _pulseDetonator = GetComponentInChildren<PulseDetonator>();
        }

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
            if (IsShieldsActive)
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
            if(!_controlsEnabled)
            {
                return;
            }

            if (IsShieldsActive)
            {
                return;
            }

            if (PlayerManagerInstance.CheckPlasma())
            {
                ActivateShields();
            }
        }

        //a way to activate player shields from other classes, or from unity events
        public void ActivateShield()
        {
            ActivateShields();
        }

        //activate whatever shield effect is currently equipped
        protected override void ActivateShields()
        {
            if (_doesPlayerHavePulseDetonator.Value)
            {
                _pulseDetonator.Detonate();
                return;
            }

            if (_doesPlayerHaveSignalBeacon.Value)
            {
               SignalBeaconRoll();
            }

            EnableShield();
        }

        //enable the shield object
        private void EnableShield()
        {
            IsShieldsActive = true;
            PlayerManagerInstance.IsPlayerColliderEnabled = false;
        }

        private void SignalBeaconRoll()
        {
            bool signalBeaconSuccess = PercentageRoll(_signalBeaconSuccessPercentage);
            if (signalBeaconSuccess)
            {
                _onSignalBeamSuccess.Raise();
            }
        }

        protected override void DeactivateShields()
        {
            ShieldActiveTimer = _shieldActiveDuration;

            IsShieldsActive = false;
            PlayerManagerInstance.IsPlayerColliderEnabled = true;
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
    }
}