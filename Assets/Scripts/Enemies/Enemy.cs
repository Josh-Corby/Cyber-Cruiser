using UnityEngine;
using System;
using System.Collections.Generic;

public class Enemy : GameBehaviour, IDamageable
{
    public static event Action<List<GameObject>, GameObject> OnEnemyDied = null;

    private EnemyMovement movement;
    private EnemyWeaponController weapon;
    private SpriteRenderer spriteRenderer;

    public string enemyName;
    public float maxHealth;
    [HideInInspector] public float currentHealth;
    [SerializeField] private bool explodeOnDeath;
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionDamage;
    [SerializeField] private GameObject explosionGraphic;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        movement = GetComponent<EnemyMovement>();
        weapon = GetComponentInChildren<EnemyWeaponController>();
    }
    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public virtual void Damage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            if (explodeOnDeath)
            {
                Explode();
            }
            else
            {
                Die();
            }
        }
    }

    private void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D collider in colliders)
        {
            GameObject explosionEffect = Instantiate(explosionGraphic, transform);
            explosionEffect.GetComponent<ExplosionGraphic>().explosionRadius = explosionRadius;
            explosionEffect.transform.SetParent(null);
            explosionEffect.transform.localScale = Vector3.one * 10;

            if (!collider.TryGetComponent<PlayerManager>(out var player))
            {
                continue;
            }
            else
            {
                player.Damage(explosionDamage);
            }
        }
        Destroy();
    }

    protected void Die()
    {
        movement.isEnemyDead = true;
        weapon.DisableWeapon();
        spriteRenderer.color = Color.black;

    }
    public virtual void Destroy()
    {
        if (ESM.enemiesAlive.Contains(gameObject))
        {       
            OnEnemyDied(ESM.enemiesAlive, gameObject);
        }     
        Destroy(gameObject);
    }
}
