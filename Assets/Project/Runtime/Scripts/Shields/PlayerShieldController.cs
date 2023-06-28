using System;
using UnityEngine;

namespace CyberCruiser
{
    public class PlayerShieldController : ShieldControllerBase
    {
        private PulseDetonator _pulseDetonator;


        #region Fields
        [SerializeField] private BoolReference _doesPlayerHavePulseDetonator;
        [SerializeField] private int _shieldActiveDuration;
        [SerializeField] private float _shieldActiveTimer;
        private bool _controlsEnabled;
        #endregion

        #region Properties
        public float ShieldActiveTimer
        {
            get => _shieldActiveTimer;
            set
            {
                _shieldActiveTimer = value;
                OnPlayerShieldsValueChange(_shieldActiveTimer);
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
                    OnPlayerShieldsDeactivated();
                }
                if (_shieldsActive)
                {
                    OnPlayerShieldsActivated(_shieldActiveDuration);
                }
            }
        }
        #endregion

        #region Actions
        public static event Action OnPlayerShieldsDeactivated = null;
        public static event Action<int> OnPlayerShieldsActivated = null;
        public static event Action<float> OnPlayerShieldsValueChange = null;
        #endregion

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

        protected override void ActivateShields()
        {
            if (_doesPlayerHavePulseDetonator.Value)
            {
                _pulseDetonator.Detonate();
                return;
            }

            IsShieldsActive = true;
            PlayerManagerInstance.IsPlayerColliderEnabled = false;
        }

        protected override void DeactivateShields()
        {
            ShieldActiveTimer = _shieldActiveDuration;

            IsShieldsActive = false;
            PlayerManagerInstance.IsPlayerColliderEnabled = true;
        }

        public override void ProcessCollision(GameObject collider, Vector2 collisionPoint)
        {
            if (collider.GetComponent<Boss>()) return;

            else if (collider.TryGetComponent<Pickup>(out var pickup))
            {
                pickup.PickupEffect();
                Destroy(pickup.gameObject);
                return;
            }
            base.ProcessCollision(collider, collisionPoint);
        }

        public override void ReduceShields(float damage)
        {
            ShieldActiveTimer -= damage;
        }
    }
}