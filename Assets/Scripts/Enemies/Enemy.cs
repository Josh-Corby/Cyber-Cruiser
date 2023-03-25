using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Enemy : GameBehaviour, IDamageable
{
    public static event Action<List<GameObject>, GameObject> OnEnemyDied = null;

    public EnemyScriptableObject _unitInfo;
    private EnemyMovement _unitMovement;
    private EnemyWeaponController _weapon;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private Rigidbody2D _rb2D;


    public string unitName;
    public float maxHealth;
    public float currentHealth;
    private bool _explodeOnDeath;
    private float _explosionRadius;
    private float _explosionDamage;
    private GameObject _explosionEffect;



    private void Awake()
    {
        AssignEnemyInfo();
    }
    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    private void AssignEnemyInfo()
    {
        _unitMovement = GetComponent<EnemyMovement>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _weapon = GetComponentInChildren<EnemyWeaponController>();
        _collider = GetComponent<Collider2D>();
        _rb2D = GetComponent<Rigidbody2D>();

        unitName = _unitInfo.unitName;
        maxHealth = _unitInfo.maxHealth;
        _explodeOnDeath = _unitInfo.explodeOnDeath;
        _explosionRadius = _unitInfo.explosionRadius;
        _explosionDamage = _unitInfo.explosionDamage;
        _explosionEffect = _unitInfo.explosionEffect;

        _unitMovement.AssignEnemyMovementInfo(_unitInfo);
    }

    public virtual void Damage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            if (_explodeOnDeath)
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
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _explosionRadius);
        foreach (Collider2D collider in colliders)
        {
            GameObject explosionEffect = Instantiate(_explosionEffect, transform);
            explosionEffect.GetComponent<ExplosionGraphic>().explosionRadius = _explosionRadius;
            explosionEffect.transform.SetParent(null);
            explosionEffect.transform.localScale = Vector3.one * 10;

            if (!collider.TryGetComponent<PlayerManager>(out var player))
            {
                continue;
            }
            else
            {
                player.Damage(_explosionDamage);
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
        _spriteRenderer.color = Color.grey;

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
        if (_explodeOnDeath)
        {
            Gizmos.DrawWireSphere(transform.position, _explosionRadius);
        }
    }
}
