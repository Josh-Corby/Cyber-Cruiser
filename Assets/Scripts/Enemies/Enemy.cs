using UnityEngine;
using System;

public class Enemy : GameBehaviour, IDamageable
{
    protected const string DEAD_ENEMY_LAYER_NAME = "DeadEnemy";

    #region References
    [HideInInspector] public EnemyScriptableObject _unitInfo;
    private EnemyMovement _unitMovement;
    private EnemyWeaponController _weapon;
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _deadSprite;
    private GameObject _crashParticles;
    private GameObject _explosionEffect;
    #endregion

    #region Fields
    public string unitName;
    [SerializeField] private float _currentHealth;
    protected float _maxHealth;
    private float _explosionRadius;
    private float _explosionDamage;
    protected bool _explodeOnDeath;
    #endregion

    #region Properties
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
    #endregion

    #region Actions
    public static event Action<GameObject, bool> OnEnemyAliveStateChange = null;
    public static event Action<GameObject> OnEnemyCrash = null;
    #endregion

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
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _weapon = GetComponentInChildren<EnemyWeaponController>();
        unitName = _unitInfo.unitName;
        gameObject.name = unitName;
        _maxHealth = _unitInfo.maxHealth;
        _explodeOnDeath = _unitInfo.explodeOnDeath;

        if (_explodeOnDeath)
        {
            _explosionRadius = _unitInfo.explosionRadius;
            _explosionDamage = _unitInfo.explosionDamage;
            _explosionEffect = _unitInfo.explosionEffect;
        }

        if (TryGetComponent<EnemyMovement>(out var enemyMovement))
        {
            _unitMovement = enemyMovement;
            _unitMovement.AssignEnemyMovementInfo(_unitInfo);
        }

        if (!_explodeOnDeath)
        {

            _crashParticles = transform.GetComponentInChildren<ParticleSystem>().gameObject;
            if (_crashParticles == null)
            {
                return;
            }
            _crashParticles.SetActive(false);
        }

        OnEnemyAliveStateChange(gameObject, true);
    }

    public virtual void Damage(float damage)
    {
        Debug.Log("enemy damaged");
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            if (_explodeOnDeath)
            {
                Explode();
            }
            else
            {
                Crash();
            }
        }
    }

    protected void Explode()
    {
        GameObject explosionEffect = Instantiate(_explosionEffect, transform);
        explosionEffect.GetComponent<ExplosionGraphic>().explosionRadius = _explosionRadius;
        explosionEffect.transform.SetParent(null);
        explosionEffect.transform.localScale = Vector3.one * 22;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _explosionRadius);
        foreach (Collider2D collider in colliders)
        {
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

    protected virtual void Crash()
    {
        if(_unitMovement != null)
        {
            _unitMovement.isEnemyDead = true;
        }

        if (_weapon != null)
        {
            _weapon.gameObject.SetActive(false);
        }

        if (_spriteRenderer != null)
        {
            _spriteRenderer.sprite = _deadSprite;
            _spriteRenderer.sortingOrder = -1;
        }

        _crashParticles.SetActive(true);

        //change object layer to layer that only collides with cull area
        gameObject.layer = LayerMask.NameToLayer(DEAD_ENEMY_LAYER_NAME);

        //remove enemy from enemies alive so it doesn't make boss spawner wait for it
        OnEnemyAliveStateChange(gameObject, false);

        OnEnemyCrash(gameObject);
    }

    public virtual void Destroy()
    {
        OnEnemyAliveStateChange(gameObject, false);
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

[Serializable]
public struct EnemyCategory
{
    public string CategoryName;
    [Range(0, 1)]
    public float CategoryWeight;
    public EnemyType[] CategoryTypes;
    [HideInInspector]
    [Range(0, 1)]
    public float TotalTypeWeights;
}

[Serializable]
public struct EnemyType
{
    public EnemyScriptableObject EnemySO;
    [Range(0, 1)]
    public float spawnWeight;
}
