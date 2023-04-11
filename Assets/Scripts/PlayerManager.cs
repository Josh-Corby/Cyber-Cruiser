using UnityEngine;
using System;
using System.Collections;

public enum PlayerHealthState
{
    Healthy, Low, Critical
}
public class PlayerManager : GameBehaviour<PlayerManager>, IDamageable
{
    private const string PLAYER_PLASMA = "PlayerPlasma";
    private const float I_FRAMES_DURATION = 0.3f;

    private PlayerHealthState _playerHealthState;
    [HideInInspector] public GameObject player;
    [HideInInspector] public PlayerShipController playerShipController;
    private PlayerShieldController shieldController;
    [SerializeField] private int plasmaCost;
    private float _currentHealth;
    [SerializeField] private float _maxHealth;
    [SerializeField] private int _playerPlasma;
    [SerializeField] private Collider2D _playerCollider;
    private bool _hasPlayerTakenDamage;

    public static event Action OnPlayerDeath = null;
    public static event Action<int> OnPlasmaChange = null;
    public static event Action<UISlider, float> OnPlayerMaxHealthChange = null;
    public static event Action<UISlider, float> OnPlayerCurrentHealthChange = null;
    public static event Action<PlayerHealthState> OnPlayerHealthStateChange = null;

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

            if(_currentHealth <= 2 && _currentHealth >1)
            {
                PlayerHealthState = PlayerHealthState.Low;
            }

            if(_currentHealth <= 1)
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

    public PlayerHealthState PlayerHealthState
    {
        set
        {
            _playerHealthState = value;
            OnPlayerHealthStateChange(_playerHealthState);
        }
        get
        {
            return _playerHealthState;
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
            OnPlayerMaxHealthChange(GUIM.playerHealthBar, _maxHealth);
        }
    }

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

    private void Awake()
    {
        _playerCollider = GetComponent<Collider2D>();
        shieldController = GetComponentInChildren<PlayerShieldController>();
        playerShipController = GetComponent<PlayerShipController>();
        player = gameObject;
    }

    private void OnEnable()
    {
        GameManager.OnLevelCountDownStart += FullHeal;
        InputManager.OnShield += CheckShieldsState;
        Pickup.OnPlasmaPickup += AddPlasma;
        Pickup.OnHealthPickup += Heal;
    }

    private void OnDisable()
    {
        GameManager.OnLevelCountDownStart -= FullHeal;
        InputManager.OnShield -= CheckShieldsState;
        Pickup.OnPlasmaPickup -= AddPlasma;
        Pickup.OnHealthPickup -= Heal;
    }

    private void Start()
    {
        OnPlayerMaxHealthChange(GUIM.playerHealthBar, _maxHealth);
        FullHeal();
        RestoreSavedPlasma();
        _hasPlayerTakenDamage = false;
    }

    private void FullHeal()
    {
        PlayerCurrentHealth = PlayerMaxHealth;
        OnPlayerCurrentHealthChange(GUIM.playerHealthBar, _currentHealth);
    }

    public void Heal(int heal)
    {
        PlayerCurrentHealth  = _maxHealth;
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
        yield return new WaitForSeconds(I_FRAMES_DURATION);
        _playerCollider.enabled = true;
        _hasPlayerTakenDamage = false;
    }
    public void Destroy()
    {
        //Debug.Log("Player dead");
        OnPlayerDeath?.Invoke();
    }

    private void AddPlasma(int amount)
    {
        PlayerPlasma += amount;
    }

    private void ReducePlasma(int amount)
    {
        PlayerPlasma -= amount;
    }

    private void RestoreSavedPlasma()
    {
        PlayerPlasma = PlayerPrefs.GetInt(nameof(PLAYER_PLASMA));
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

        else if(collider.TryGetComponent<Pickup>(out var pickup))
        {
            pickup.PickupEffect();
            Destroy(pickup.gameObject);
            return;
        }
    }

    private void CheckShieldsState()
    {
        if (shieldController._shieldsActive)
        {
            Debug.Log("Shields already Active");
            return;
        }

        else
        {
            CheckPlasma();
        }
    }

    private void CheckPlasma()
    {
        if (PlayerPlasma >= plasmaCost)
        {
            ReducePlasma(plasmaCost);
            ActivateShields();
            return;
        }

        else
        {
            Debug.Log("Not enough plasma");
        }
    }

    private void ActivateShields()
    {
        shieldController.ActivateShields();
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt(nameof(PLAYER_PLASMA), PlayerPlasma);
    }
}
