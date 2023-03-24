using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Enemy : GameBehaviour, IDamageable
{
    public static event Action<List<GameObject>, GameObject> OnEnemyDied = null;

    private EnemyWeaponController _weapon;
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private Collider2D _collider;
    private Rigidbody2D _rb2D;

    public string enemyName;
    public float maxHealth;
    [HideInInspector] public float currentHealth;
    [SerializeField] private bool explodeOnDeath;
    public float explosionRadius;
    [SerializeField] private float explosionDamage;
    [SerializeField] private GameObject explosionGraphic;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _weapon = GetComponentInChildren<EnemyWeaponController>();
        _collider = GetComponent<Collider2D>();
        _rb2D = GetComponent<Rigidbody2D>();
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
                StartCoroutine(Die());
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

    protected IEnumerator Die()
    {
        if(TryGetComponent<EnemyMovement>(out var movement))
        {
            movement.isEnemyDead = true;
        }  
        //remove the rigidbody so the object doesnt break when the collider is disabled
        Destroy(_rb2D);

        if (_weapon != null)
        {
            _weapon.DisableWeapon();
        }
        _spriteRenderer.color = Color.black;

        //wait a frame for rigidbody to be destroyed
        yield return new WaitForEndOfFrame();
        _collider.enabled = false;
    }

    public virtual void Destroy()
    {
        if (ESM.enemiesAlive.Contains(gameObject))
        {       
            OnEnemyDied(ESM.enemiesAlive, gameObject);
        }     
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (explodeOnDeath)
        {
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}
