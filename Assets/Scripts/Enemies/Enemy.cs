using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Enemy : GameBehaviour, IDamageable
{
    public static event Action<List<GameObject>, GameObject> OnEnemyDied = null;

    [SerializeField] protected EnemyScriptableObject _unitInfo;
    private EnemyMovement _unitMovement;
    private EnemyWeaponController _weapon;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private Rigidbody2D _rb2D;


    public string unitName;
    protected float _maxHealth;
    private float _currentHealth;
    private bool _explodeOnDeath;
    private float _explosionRadius;
    private float _explosionDamage;
    private GameObject _explosionEffect;


    protected const string DEAD_ENEMY_LAYER_NAME = "DeadEnemy";

    public float CurrentHealth
    {
        get
        {
            return _currentHealth;
        }
        set
        {
            _currentHealth = value;
        }
    }

    public float MaxHealth
    {
        get
        {
            return _maxHealth;
        }
        set
        {
            _maxHealth = value;
        }
    }
    protected virtual void Awake()
    {
        AssignEnemyInfo();
    }
    protected virtual void Start()
    {
        _currentHealth = _maxHealth;
    }

    private void AssignEnemyInfo()
    {
        _unitMovement = GetComponent<EnemyMovement>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _weapon = GetComponentInChildren<EnemyWeaponController>();
        _collider = GetComponent<Collider2D>();
        _rb2D = GetComponent<Rigidbody2D>();

        unitName = _unitInfo.unitName;
        _maxHealth = _unitInfo.maxHealth;
        _explodeOnDeath = _unitInfo.explodeOnDeath;
        _explosionRadius = _unitInfo.explosionRadius;
        _explosionDamage = _unitInfo.explosionDamage;
        _explosionEffect = _unitInfo.explosionEffect;

        _unitMovement.AssignEnemyMovementInfo(_unitInfo);
    }

    public virtual void Damage(float damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
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
        //Destroy(_rb2D);

        if (_weapon != null)
        {
            _weapon.DisableWeapon();
        }
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = Color.grey;
        }

        //wait a frame for rigidbody to be destroyed
        yield return new WaitForEndOfFrame();
        //disable collider so layer can be safely changed
        //_collider.enabled = false;

        //change object layer to layer that only collides with cull area
        gameObject.layer = LayerMask.NameToLayer(DEAD_ENEMY_LAYER_NAME);
        //enable collider
        //_collider.enabled = true;
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
