using UnityEngine;
using System;

public class PlayerManager : MonoBehaviour
{
    public static event Action OnPlayerDeath = null;
    public static event Action<int> OnPlasmaChange = null;

    private PlayerShieldController shieldController;

    [SerializeField] private int currentPlasma;
    [SerializeField] private int plasmaCost;
    private int maxPlasma = 20;

    private float weaponPackCountdown;
    private float weaponPackDuration;

    [SerializeField] private int maxHealth;
    private int currentHealth;


    private void Awake()
    {
        shieldController = GetComponentInChildren<PlayerShieldController>();
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
            currentHealth -= 1;
            if (currentHealth <= 0)
            {
                OnPlayerDeath?.Invoke();
            }
            else
            {
                Debug.Log("Health left: " + currentHealth);
            }
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
        if(currentPlasma >= plasmaCost)
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
}
