using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum PlayerHealthState
{
    Healthy, Low, Critical
}

public class PlayerManager : GameBehaviour<PlayerManager>, IDamageable
{
    #region References
    public GameObject player;
    private Collider2D _playerCollider;
    #endregion

    #region Fields
    [SerializeField] private int _playerPlasma;
    [SerializeField] private int _plasmaCost;
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _currentHealth;
    [SerializeField] private PlayerHealthState _playerHealthState;
    [SerializeField] private GameObject _batteryPack, _hydrocoolant, _plasmaCache;

    public bool isDead;
    [SerializeField] private float _iFramesDuration;
    private bool _isPlayerImmuneToDamage;
    [SerializeField] private bool _isBatteryPack, _isHydrocoolant, _isPlasmaCache;
    [SerializeField] private List<GameObject> _addOnObjects = new();

    [SerializeField] private int _ramDamage;

    private Coroutine _iFramesCoroutine;
    #endregion

    #region Properties
    public int PlayerPlasma
    {
        get => _playerPlasma; 
        private set
        {
            _playerPlasma = value;
            OnPlasmaChange(_playerPlasma);
        }
    }

    public int PlasmaCost { get => _plasmaCost; private set => _plasmaCost = value; }

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

            OnPlayerCurrentHealthChange(GUIM.playerHealthBar, _currentHealth);
            if (_currentHealth <= 0)
            {
                isDead = true;
                Destroy();
            }
        }
    }

    public float PlayerMaxHealth
    {
        get => _maxHealth;
        private set
        {
            _maxHealth = value;
            OnPlayerMaxHealthSet(GUIM.playerHealthBar, _maxHealth);
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
            else if (_playerCollider.enabled == false)
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
    public static event Action<int> OnIonPickup = null;
    public static event Action<int> OnPlasmaChange = null;
    public static event Action<int> OnPlasmaPickupValue = null;
    public static event Action<UISlider, float> OnPlayerMaxHealthSet = null;
    public static event Action<UISlider, float> OnPlayerCurrentHealthChange = null;
    public static event Action<PlayerHealthState> OnPlayerHealthStateChange = null;
    #endregion

    new private void Awake()
    {
        _playerCollider = player.GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        Pickup.OnResourcePickup += AddResources;
        DisableAddOnSprites();
        SetAddOnBools();
        SetStats();
        _isPlayerImmuneToDamage = false;
    }

    private void OnDisable()
    {
        Pickup.OnResourcePickup -= AddResources;
    }

    private void SetStats()
    {
        PlayerMaxHealth = PSM.PlayerCurrentMaxHealth;
        PlayerPlasma = PSM.PlayerPlasma;
        PlasmaCost = PSM.PlasmaCost;
        _iFramesDuration = PSM.IFramesDuration;

        FullHeal();
    }

    private void DisableAddOnSprites()
    {
        foreach (GameObject sprite in _addOnObjects)
        {
            sprite.SetActive(false);
        }
    }

    private void SetAddOnBools()
    {
        _isBatteryPack = PSM.IsBatteryPack;
        _isHydrocoolant = PSM.IsHydrocoolant;
        _isPlasmaCache = PSM.IsPlasmaCache;
        SetAddOnSprites();
    }

    private void SetAddOnSprites()
    {
        _batteryPack.SetActive(_isBatteryPack);
        _hydrocoolant.SetActive(_isHydrocoolant);
        _plasmaCache.SetActive(_isPlasmaCache);
    }

    private void AddResources(int healthAmount, int plasmaAmount, int ionAmount)
    {
        PlayerCurrentHealth += healthAmount;
        PlayerPlasma += plasmaAmount;
        OnIonPickup(ionAmount);

        if(plasmaAmount > 0)
        {
            if(OnPlasmaPickupValue != null)
            {
                OnPlasmaPickupValue(plasmaAmount);
            }
        }
    }

    private void FullHeal()
    {
        PlayerCurrentHealth = PlayerMaxHealth;
    }

    public bool CheckPlasma()
    {
        if (PlayerPlasma < PlasmaCost)
        {
            Debug.Log("Not enough plasma");
            return false;
        }

        if (PlayerPlasma >= PlasmaCost)
        {
            PlayerPlasma -= PlasmaCost;
            return true;
        }

        else return false;
    }

    public void Damage(float damage)
    {
        if (!_isPlayerImmuneToDamage)
        {
            _isPlayerImmuneToDamage = true;
            PlayerCurrentHealth -= damage;

            StartIFrames();
        }
    }

    private IEnumerator Iframes()
    {
        Debug.Log("iFrames");
        _playerCollider.enabled = false;
        _isPlayerImmuneToDamage = true;
        yield return new WaitForSeconds(_iFramesDuration);
        CancelIFrames();
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

    private void DisablePlayerCollision()
    {
        _playerCollider.enabled = false;
        _isPlayerImmuneToDamage = true;
    }

    public void Destroy()
    {
        OnPlayerDeath?.Invoke();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ProcessCollision(collision.gameObject);
    }

    private void ProcessCollision(GameObject collider)
    {
        if (collider.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.Damage(_ramDamage);
            Damage(1);
        }

        else if (collider.TryGetComponent<CyberKrakenTentacle>(out var tentacle))
        {
            Damage(1);
        }

        else if (collider.TryGetComponent<Pickup>(out var pickup))
        {
            pickup.PickupEffect();
            Destroy(pickup.gameObject);
        }
    }
}
