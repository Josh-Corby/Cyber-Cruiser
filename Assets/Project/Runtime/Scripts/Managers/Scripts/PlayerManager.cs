using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CyberCruiser
{
    public enum PlayerHealthState
    {
        Healthy, Low, Critical
    }

    public class PlayerManager : GameBehaviour<PlayerManager>, IDamageable
    {
        public GameObject player;
        private Collider2D _playerCollider;
        [SerializeField] private ParticleSystem _collisionParticles;

        #region Player Systems
        [Header(" Systems References")]
        [SerializeField] private PlayerShipController _playerShipController;
        [SerializeField] private PlayerShieldController _playerShieldController;
        [SerializeField] private PlayerWeaponController _playerWeaponController;
        [SerializeField] private PlayerUIManager _playerUIManager;

        [SerializeField] private BoolValue _holdOneAddOn;
        [SerializeField] private GameObject _currentPickup;

        [SerializeField] private List<PickupInfo> _currentAddOns = new();
        #endregion

        #region SO References
        [Header("SO References")]
        [SerializeField] private IntValue _currentPlasma;
        [SerializeField] private IntValue _currentIon;
        [SerializeField] private BoolValue _isPlayerDead;
        [SerializeField] private IntReference _currentRamDamageReference;
        [SerializeField] private IntReference _currentPlasmaCost;
        #endregion

        #region Pickups
        [Header("Pickups")]

        #region Retaliation Matrix
        [Header("Retaliation Matrix")]
        [SerializeField] private ExplodingObject _retaliationMatrix;
        [SerializeField] private BoolReference _doesPlayerHaveRetaliationMatrix;
        #endregion

        #region Attractor Unit
        [Header("Attractor Unit")]
        [SerializeField] private AttractorUnit _attractorUnit;
        #endregion
        #endregion

        #region Constants
        private const int BASE_MAX_HEALTH = 5;
        private const int BASE_PLASMA_COST = 5;
        private const float BASE_I_FRAMES_DURATION = 0.3f;
        #endregion

        #region Player Current Info
        [Header("Player Current Info")]
        private int _maxHealth;
        private float _currentHealth;
        private PlayerHealthState _playerHealthState;
        private float _iFramesDuration;
        private bool _isPlayerImmuneToDamage;
        #endregion

        private Coroutine _iFramesCoroutine;

        #region Properties
        public bool IsPlayerDead { get => _isPlayerDead.Value; private set => _isPlayerDead.Value = value; }

        private int PlasmaCost { get => _currentPlasmaCost.Value; }

        private int CurrentPlasma
        {
            get => _currentPlasma.Value;
            set
            {
                value = value < 0 ? 0 : value;
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
                    IsPlayerDead = false;
                    if (_currentHealth >= PlayerMaxHealth)
                    {
                        _currentHealth = PlayerMaxHealth;
                    }

                    if (_currentHealth > 2)
                    {
                        CurrentHealthState = PlayerHealthState.Healthy;
                    }

                    else if (_currentHealth <= 2 && _currentHealth > 1)
                    {
                        CurrentHealthState = PlayerHealthState.Low;
                    }

                    else if (_currentHealth <= 1)
                    {
                        CurrentHealthState = PlayerHealthState.Critical;
                    }
                }

                _playerUIManager.ChangeSliderValue(PlayerSliderTypes.Health, _currentHealth);

                if (_currentHealth <= 0)
                {
                    IsPlayerDead = true;
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
                _playerUIManager.EnableSliderAtMaxValue(PlayerSliderTypes.Health, _maxHealth);
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

        public PlayerHealthState CurrentHealthState
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
        public static event Action<int> OnPlasmaSpent = null;
        public static event Action<int> OnPlasmaPickupValue = null;
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

            _attractorUnit.gameObject.SetActive(false);
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
            _playerShipController.EnableControls();
            _playerWeaponController.EnableControls();
            _playerShieldController.EnableControls();
        }

        private void DisablePlayerControls()
        {
            _playerShipController.DisableControls();
            _playerWeaponController.DisableControls();
            _playerShieldController.DisableControls();
        }

        private void ResetStats()
        {
            _iFramesDuration = BASE_I_FRAMES_DURATION;
            PlayerMaxHealth = BASE_MAX_HEALTH;
            FullHeal();
        }

        public void EnableAttractorUnit()
        {
            _attractorUnit.gameObject.SetActive(true);
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

        public bool ComparePlasmaToCost()
        {
            if (CurrentPlasma < PlasmaCost)
            {
                Debug.Log("Not enough plasma");
                return false;
            }

            if (CurrentPlasma >= PlasmaCost)
            {
                CurrentPlasma -= PlasmaCost;
                OnPlasmaSpent?.Invoke(PlasmaCost);
                return true;
            }

            else return false;
        }

        public void QuarterHeal()
        {
            PlayerCurrentHealth += PlayerMaxHealth / 4;
        }

        public void Overload()
        {
            _playerWeaponController.OverheatToMax();
            _playerShieldController.DeactivateShield();
        }

        #region Player Damage Functions
        public void Damage(float damage, EnemyScriptableObject instigator)
        {
            if (damage <= 0 || _isPlayerImmuneToDamage)
            {
                return;
            }

            _isPlayerImmuneToDamage = true;

            if (DoesPlayerDieFromDamage(damage))
            {
                Debug.Log("Player killed by " + instigator.name);
                instigator.OnPlayerKilled();
            }

            PlayerCurrentHealth -= damage;

            RetaliationMatrixCheck();
            PlayCollisionParticles();
            StartIFrames();
        }

        public bool DoesPlayerDieFromDamage(float damage)
        {
            return PlayerCurrentHealth - damage <=0;
        }

        private void PlayCollisionParticles()
        {
            if (_collisionParticles != null)
            {
                _collisionParticles.Play();
            }
        }

        private void RetaliationMatrixCheck()
        {
            if (_doesPlayerHaveRetaliationMatrix.Value)
            {
                _retaliationMatrix.Explode();
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
            _playerShipController.OnPlayerDeath();
        }
        #endregion

        private void OnCollisionEnter2D(Collision2D collision)
        {
            ProcessCollision(collision.gameObject);
        }

        private void ProcessCollision(GameObject collider)
        {
            if (collider.TryGetComponent<Pickup>(out var pickup))
            {
                OnPickup(pickup);
                return;
            }

            if (collider.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.Damage(_currentRamDamageReference.Value, null);
            }
        }

        public void OnPickup(Pickup pickup)
        {
            PickUpAddOn(pickup);
            pickup.PickupEffect();
        }

        private void PickUpAddOn(Pickup addOn)
        {
            PickupEffectBase[] newEffects = addOn.gameObject.GetComponents<PickupEffectBase>();

            Debug.Log("There are " + newEffects.Length + " new effects");
            for (int i = 0; i < newEffects.Length; i++)
            {
                newEffects[i].OnPickup();
            }

            //check if pickup has addon effects
            if (newEffects.Length > 0)
            {
                //if one pickup mode is enabled
                if (_holdOneAddOn.Value)
                {
                    if(_currentPickup != null)
                    {
                        //if there are previous addons
                        PickupEffectBase[] currentEffects = _currentPickup.GetComponents<PickupEffectBase>();
                        Debug.Log("There are " +currentEffects.Length + " effects to be removed");

                        //undo its effects
                        if (currentEffects.Length > 0)
                        {
                            for (int i = 0; i < currentEffects.Length; i++)
                            {
                                currentEffects[i].OnDropped();
                            }
                        }


                        Destroy(_currentPickup,1f);
                    }       

                    _currentPickup = addOn.gameObject;
                    _currentPickup.SetActive(false);
                }

                //if there is no limit to pickups
                if(!_holdOneAddOn.Value)
                {
                    _currentAddOns.Add(addOn.Info);
                    Destroy(addOn.gameObject);
                }
            }
        }

        public void ToggleUseOneAddOn()
        {
            _holdOneAddOn.Value = !_holdOneAddOn.Value;
        }
    }
}