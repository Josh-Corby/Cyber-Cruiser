using UnityEngine;
using System;

public class PlayerManager : GameBehaviour<PlayerManager>, IDamageable
{
    public static event Action OnPlayerDeath = null;
    public static event Action<int> OnPlasmaChange = null;

    [HideInInspector] public GameObject player;
    private PlayerShieldController shieldController;

    [SerializeField] private int currentPlasma;
    [SerializeField] private int plasmaCost;
    private int maxPlasma = 20;

    private float weaponPackCountdown;
    private float weaponPackDuration;

    [SerializeField] private int maxHealth;
    private float currentHealth;


    private void Awake()
    {
        shieldController = GetComponentInChildren<PlayerShieldController>();
        player = gameObject;
    }
    private void OnEnable()
    {
        GameManager.OnLevelCountDownStart += FullHeal;
        InputManager.OnShield += CheckShieldsState;
    }

    private void OnDisable()
    {
        GameManager.OnLevelCountDownStart -= FullHeal;
        InputManager.OnShield -= CheckShieldsState;
    }

    private void Start()
    {
        FullHeal();
    }

    private void FullHeal()
    {
        currentHealth = maxHealth;
        currentPlasma = maxPlasma;

        OnPlasmaChange(currentPlasma);
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
        }
        else
        {
            Debug.Log("Health left: " + currentHealth);
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
        if (currentPlasma >= plasmaCost)
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

    private void AddPlasma(int amount)
    {
        currentPlasma += amount;
        OnPlasmaChange(currentPlasma);
    }

    private void ReducePlasma(int amount)
    {
        currentPlasma -= amount;
        OnPlasmaChange(currentPlasma);
    }

    private void ActivateShields()
    {
        shieldController.ActivateShields();
    }

    public void Damage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Destroy();
        }
    }

    public void Destroy()
    {
        Debug.Log("Player dead");
        OnPlayerDeath?.Invoke();
    }
}
