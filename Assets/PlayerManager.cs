using UnityEngine;
using System;

public class PlayerManager : MonoBehaviour
{
    public static event Action OnPlayerDeath = null;
    [SerializeField] private PlayerShieldController shieldController;

    [SerializeField] private int plasmaCount;
    [SerializeField] private int plasmaCost;

    private float weaponPackCountdown;
    private float weaponPackDuration;

    [SerializeField] private int maxHealth;
    private int currentHealth;

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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ProcessCollision(collision.gameObject);
    }

    private void ProcessCollision(GameObject collider)
    {
        if (collider.GetComponent<Enemy>())
        {
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
        if(plasmaCount >= plasmaCost)
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
        plasmaCount += amount;
    }

    private void ReducePlasma(int amount)
    {
        plasmaCount -= amount;
    }

    private void ActivateShields()
    {
        shieldController.ActivateShields();
    }
}
