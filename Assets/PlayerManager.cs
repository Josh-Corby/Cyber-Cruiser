using UnityEngine;
using System;

public class PlayerManager : MonoBehaviour
{
    public static event Action OnPlayerDeath = null;

    private float shieldCooldown;
    private int plasmaCount;

    private float weaponPackCountdown;
    private float weaponPackDuration;

    [SerializeField] private int maxHealth;
    private int currentHealth;

    private void OnEnable()
    {
        GameManager.OnLevelCountDownStart += FullHeal;
    }

    private void OnDisable()
    {
        GameManager.OnLevelCountDownStart -= FullHeal;
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
}
