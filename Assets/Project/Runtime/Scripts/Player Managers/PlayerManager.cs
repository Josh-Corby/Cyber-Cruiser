using System;
using System.Collections;
using UnityEngine;

namespace CyberCruiser
{
    public class PlayerManager : GameBehaviour<PlayerManager>, IDamageable
    {
        #region References
        public GameObject player;
        [SerializeField] private PlayerAddOnManager _addOnManager;
        private Collider2D _playerCollider;

        [SerializeField] private PlayerShipController _playerShipController;
        [SerializeField] private PlayerShieldController _playerShieldController;
        [SerializeField] private PlayerWeaponController _playerWeaponController;
        [SerializeField] private ParticleSystem _collisionParticles;
        #endregion

        [SerializeField] private IntValue _currentPlasma;
        [SerializeField] private IntValue _currentIon;
        [SerializeField] private IntReference _ramDamage;


        #region Fields
        private const int BASE_MAX_HEALTH = 5;
        private const int BASE_PLASMA_COST = 5;
        private const float BASE_I_FRAMES_DURATION = 0.3f;
       

        [SerializeField] private int _plasmaCost;
        [SerializeField] private int _maxHealth;
        [SerializeField] private float _currentHealth;
        [SerializeField] private PlayerHealthState _playerHealthState;

        public bool isDead;
        [SerializeField] private float _iFramesDuration;
        private bool _isPlayerImmuneToDamage;
        private Coroutine _iFramesCoroutine;
        #endregion

        #region Properties
        private int PlasmaCost { get => _plasmaCost; set => _plasmaCost = value; }
 
        private int CurrentPlasma
        {
            get => _currentPlasma.Value; 
            set
            {
                value = value < 0 ? 0 : value;
                _currentPlasma.Value = value;
                OnPlasmaChange?.Invoke(CurrentPlasma);
            }
        }

        private int CurrentIon { get => _currentIon.Value; set => _currentIon.Value = value; }

        private float PlayerCurrentHealth
        {
            get => _currentHealth;
            set
            {
                _currentHealth = value;
                if (_currentHealth > 0)
                {
                    isDead = false;
                    if (_currentHealth >= PlayerMaxHealth)
                    {
                        _currentHealth = PlayerMaxHealth;
                    }

                    if (_currentHealth > 2)
                    {
                        PlayerHealthState = PlayerHealthState.Healthy;
                    }

                    else if (_currentHealth <= 2 && _currentHealth > 1)
                    {
                        PlayerHealthState = PlayerHealthState.Low;
                    }

                    else if (_currentHealth <= 1)
                    {
                        PlayerHealthState = PlayerHealthState.Critical;
                    }
                }

                OnPlayerCurrentHealthChange(_currentHealth);
                if (_currentHealth <= 0)
                {
                    isDead = true;
                    Destroy();
                }
            }
        }

        private int PlayerMaxHealth
        {
            get => _maxHealth;
            set
            {
                _maxHealth = value;
                OnPlayerMaxHealthSet(_maxHealth);
            }
        }

        public bool IsPlayerColliderEnabled
        {
            set
            {
                //if playercollider is already disabled and disabled by property call
                if (_playerCollider.enabled == false && value == false)
                {
                    EndIFrames();
                    DisablePlayerCollision();
                    return;
                }

                _playerCollider.enabled = value;

                //start iframes when collider is enabled by property call
                if (_playerCollider.enabled == true)
                {
                    StartIFrames();
                }

                //end iframes if player collider is disabled by property call
                else
                {
                    EndIFrames();
                }
            }
        }

        public PlayerHealthState PlayerHealthState
        {
            set
            {
                _playerHealthState = value;

                if (OnPlayerHealthStateChange != null)
                {
                    OnPlayerHealthStateChange(value);
                }
            }
            get
            {
                return _playerHealthState;
            }
        }
        #endregion

        #region Actions
        public static event Action OnPlayerDeath = null;
        public static event Action<int> OnPlasmaChange = null;
        public static event Action<int> OnPlasmaPickupValue = null;
        public static event Action<int> OnPlayerMaxHealthSet = null;
        public static event Action<float> OnPlayerCurrentHealthChange = null;
        public static event Action<PlayerHealthState> OnPlayerHealthStateChange = null;
        #endregion

        protected override void Awake()
        {
            base.Awake();
            _playerCollider = player.GetComponent<Collider2D>();
        }

        private void OnEnable()
        {
            ResetStats();
            EnablePlayerControls();

            _isPlayerImmuneToDamage = false;
            _playerCollider.enabled = true;
            GameManager.OnIsTimeScalePaused += SetPlayerControls;
            Pickup.OnResourcePickup += AddResources;
        }

        private void OnDisable()
        {
            GameManager.OnIsTimeScalePaused += SetPlayerControls;
            Pickup.OnResourcePickup -= AddResources;
        }

        private void SetPlayerControls(bool isControlsDisabled)
        {
            if (isControlsDisabled)
            {
                DisablePlayerControls();
            }
            else
            {
                EnablePlayerControls();
            }
        }

        private void EnablePlayerControls()
        {
            //Debug.Log("Controls enabled");
            _playerShipController.EnableControls();
            _playerWeaponController.EnableControls();
            _playerShieldController.EnableControls();
        }

        private void DisablePlayerControls()
        {
            //Debug.Log("Controls disabled");
            _playerShipController.DisableControls();
            _playerWeaponController.DisableControls();
            _playerShieldController.DisableControls();
        }

        private void ResetStats()
        {
            _plasmaCost = BASE_PLASMA_COST;
            _iFramesDuration = BASE_I_FRAMES_DURATION;
            PlayerMaxHealth = BASE_MAX_HEALTH;
            FullHeal();

        }

        private void AddResources(int healthAmount, int plasmaAmount, int ionAmount)
        {
            PlayerCurrentHealth += healthAmount;
            CurrentIon += ionAmount;

            if (plasmaAmount > 0)
            {
                CurrentPlasma += plasmaAmount;
                OnPlasmaPickupValue?.Invoke(plasmaAmount);
            }
        }

        private void FullHeal()
        {
            PlayerCurrentHealth = PlayerMaxHealth;
        }

        public bool CheckPlasma()
        {
            if (CurrentPlasma < _plasmaCost)
            {
                Debug.Log("Not enough plasma");
                return false;
            }

            if (CurrentPlasma >= _plasmaCost)
            {
                CurrentPlasma -= _plasmaCost;
                return true;
            }

            else return false;
        }

        #region Player Damage Functions
        public void Damage(float damage)
        {
            if (!_isPlayerImmuneToDamage)
            {
                _isPlayerImmuneToDamage = true;
                PlayerCurrentHealth -= damage;
                if (_collisionParticles != null)
                {
                    Debug.Log("PlayerDamaged");
                    //_collisionParticles.Stop();
                    _collisionParticles.Play();
                }
                StartIFrames();
            }
        }

        private void StartIFrames()
        {
            if (_iFramesCoroutine != null)
            {
                StopCoroutine(_iFramesCoroutine);

            }
            CancelIFrames();
            _iFramesCoroutine = StartCoroutine(Iframes());
        }

        private void EndIFrames()
        {
            if (_iFramesCoroutine != null)
            {
                StopCoroutine(_iFramesCoroutine);
            }
            DisablePlayerCollision();
        }

        private void CancelIFrames()
        {
            _isPlayerImmuneToDamage = false;
            _playerCollider.enabled = true;
        }

        private IEnumerator Iframes()
        {
            _playerCollider.enabled = false;
            _isPlayerImmuneToDamage = true;
            yield return new WaitForSeconds(_iFramesDuration);
            CancelIFrames();
        }

        private void DisablePlayerCollision()
        {
            _playerCollider.enabled = false;
            _isPlayerImmuneToDamage = true;
        }

        public void Destroy()
        {
            OnPlayerDeath?.Invoke();
            DisablePlayerControls();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            ProcessCollision(collision.gameObject);
        }

        private void ProcessCollision(GameObject collider)
        {
            if (collider.TryGetComponent<Pickup>(out var pickup))
            {
                if (collider.TryGetComponent<PickupEffectBase>(out var addOnPickup))
                {
                    addOnPickup.OnPickup();
                }
                pickup.PickupEffect();

                return;
            }

            if (collider.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.Damage(_ramDamage.Value);
                Damage(1);
            }

            else if (collider.TryGetComponent<CyberKrakenTentacle>(out var tentacle))
            {
                Damage(1);
            }
        }
        #endregion
    }
        public enum PlayerHealthState
        {
            Healthy, Low, Critical
        }
}