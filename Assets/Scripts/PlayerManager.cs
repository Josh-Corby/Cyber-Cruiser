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

    private float _iFramesDuration;
    private bool _hasPlayerTakenDamage;
    [SerializeField] private bool _isBatteryPack, _isHydrocoolant, _isPlasmaCache;
    [SerializeField] private List<GameObject> _addOnObjects = new();    
    #endregion

    #region Properties
    public int PlayerPlasma
    {
        get
        {
            return _playerPlasma;
        }
        set
        {
            _playerPlasma = value;
            OnPlasmaChange(_playerPlasma);
        }
    }

    public int PlasmaCost
    {
        get
        {
            return _plasmaCost;
        }
        set
        {
            _plasmaCost = value;
        }
    }

    public float PlayerCurrentHealth
    {
        get
        {
            return _currentHealth;
        }
        set
        {
            _currentHealth = value;

            if (_currentHealth >= PlayerMaxHealth)
            {
                _currentHealth = PlayerMaxHealth;
                PlayerHealthState = PlayerHealthState.Healthy;
            }

            if (_currentHealth <= 2 && _currentHealth > 1)
            {
                PlayerHealthState = PlayerHealthState.Low;
            }

            if (_currentHealth <= 1)
            {
                PlayerHealthState = PlayerHealthState.Critical;
            }

            OnPlayerCurrentHealthChange(GUIM.playerHealthBar, _currentHealth);
            if (_currentHealth <= 0)
            {
                Destroy();
            }
        }
    }

    public float PlayerMaxHealth
    {
        get
        {
            return _maxHealth;
        }
        set
        {
            _maxHealth = value;
            OnPlayerMaxHealthSet(GUIM.playerHealthBar, _maxHealth);
        }
    }

    public PlayerHealthState PlayerHealthState
    {
        set
        {
            _playerHealthState = value;

            if(OnPlayerHealthStateChange != null)
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
        _hasPlayerTakenDamage = false;
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
    }

    private void FullHeal()
    {
        PlayerCurrentHealth = PlayerMaxHealth;
    }

    public bool CheckPlasma()
    {
        if(PlayerPlasma < PlasmaCost)
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
        if (!_hasPlayerTakenDamage)
        {
            _hasPlayerTakenDamage = true;
            PlayerCurrentHealth -= damage;
            StartCoroutine(PlayerDamage());
        }
    }

    private IEnumerator PlayerDamage()
    {
        _playerCollider.enabled = false;
        yield return new WaitForSeconds(_iFramesDuration);
        _playerCollider.enabled = true;
        _hasPlayerTakenDamage = false;
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
            enemy.Damage(10);
            Damage(1);
            return;
        }

        else if (collider.TryGetComponent<Pickup>(out var pickup))
        {
            pickup.PickupEffect();
            Destroy(pickup.gameObject);
            return;
        }
    }
}
