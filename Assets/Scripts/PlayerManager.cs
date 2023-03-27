using UnityEngine;
using System;

public class PlayerManager : GameBehaviour<PlayerManager>, IDamageable
{
    private const string PLAYERPLASMA = "PlayerPlasma";

    [HideInInspector] public GameObject player;
    private PlayerShieldController shieldController;
    [SerializeField] private int plasmaCost;
    private float _currentHealth;
    [SerializeField] private float _maxHealth;
    [SerializeField] private int _playerPlasma;


    public static event Action OnPlayerDeath = null;
    public static event Action<int> OnPlasmaChange = null;
    public static event Action<UISlider, float> OnPlayerMaxHealthChange = null;
    public static event Action<UISlider, float> OnPlayerCurrentHealthChange = null;


    public float PlayerCurrentHealth
    {
        get
        {
            return _currentHealth;
        }
        set
        {
            _currentHealth = value;
            if (_currentHealth > _maxHealth)
            {
                _currentHealth = _maxHealth;
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
        shieldController = GetComponentInChildren<PlayerShieldController>();
        player = gameObject;
    }
    private void OnEnable()
    {
        GameManager.OnLevelCountDownStart += FullHeal;
        InputManager.OnShield += CheckShieldsState;
        Pickup.OnPlasmaIncrease += AddPlasma;
    }

    private void OnDisable()
    {
        GameManager.OnLevelCountDownStart -= FullHeal;
        InputManager.OnShield -= CheckShieldsState;
        Pickup.OnPlasmaIncrease -= AddPlasma;
    }

    private void Start()
    {
        OnPlayerMaxHealthChange(GUIM.playerHealthBar, _maxHealth);
        FullHeal();
        RestoreSavedPlasma();
    }

    private void FullHeal()
    {
        PlayerCurrentHealth = _maxHealth;
        OnPlayerCurrentHealthChange(GUIM.playerHealthBar, _currentHealth);
    }

    public void Heal(float heal)
    {
        PlayerCurrentHealth  = _maxHealth;
    }
    public void Damage(float damage)
    {
        PlayerCurrentHealth -= damage;
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
        PlayerPlasma = PlayerPrefs.GetInt(nameof(PLAYERPLASMA));
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
        if (shieldController.shieldsActive)
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
        PlayerPrefs.SetInt(nameof(PLAYERPLASMA), PlayerPlasma);
    }
}
