using UnityEngine;
using System;

public class PlayerManager : GameBehaviour<PlayerManager>, IDamageable
{
    public static event Action OnPlayerDeath = null;
    public static event Action<int> OnPlasmaChange = null;

    [HideInInspector] public GameObject player;
    private PlayerShieldController shieldController;

    [SerializeField] private int plasmaCost;

    private float weaponPackCountdown;
    private float weaponPackDuration;



    private float _currentHealth;
    [SerializeField] private int maxHealth;
    public float PlayerHealth
    {
        get
        {
            return _currentHealth;
        }

        set
        {
            _currentHealth = value;

            if (_currentHealth <= 0)
            {
                Destroy();
            }

            if(_currentHealth > maxHealth)
            {
                _currentHealth = maxHealth;
            }
        }
    }

    [SerializeField] private int _playerPlasma;
    private const string PLAYERPLASMA = "PlayerPlasma";
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
        FullHeal();
        RestoreSavedPlasma();
    }

    private void FullHeal()
    {
        PlayerHealth += maxHealth;
    }

    public void Heal(float heal)
    {
        PlayerHealth  = maxHealth;
    }
    public void Damage(float damage)
    {
        PlayerHealth -= damage;
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
            enemy.Destroy();
            Damage(maxHealth);
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
        if (_playerPlasma >= plasmaCost)
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
        PlayerPrefs.SetInt(nameof(PLAYERPLASMA), _playerPlasma);
    }
}
