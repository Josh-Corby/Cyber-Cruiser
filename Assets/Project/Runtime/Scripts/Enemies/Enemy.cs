using System;
using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
public class Enemy : GameBehaviour, IDamageable
{
    protected const string DEAD_ENEMY_LAYER_NAME = "DeadEnemy";

    #region References
    public EnemyScriptableObject EnemyInfo;
    private EnemyMovement _enemyMovement;
    private EnemyWeaponController _weapon;
    private ExplodingObject _explosion;
    private GameObject _crashParticles;
    #endregion


    private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _deadSprite;

    #region Local Enemy General Stats
    private string _enemyName;
    protected float _maxHealth;
    protected float _currentHealth;
    protected bool _doesEnemyExplodeOnDeath;
    #endregion

    #region Actions
    public static event Action<GameObject, bool> OnEnemyAliveStateChange = null;
    public static event Action<GameObject> OnEnemyCrash = null;
    #endregion

    protected virtual void Awake()
    {
        AssignEnemyInfo();
    }

    private void AssignEnemyInfo()
    {
        _enemyName = EnemyInfo.EnemyName;
        gameObject.name = _enemyName;
        _maxHealth = EnemyInfo.GeneralStats.MaxHealth;
        _currentHealth = _maxHealth;
        _doesEnemyExplodeOnDeath = EnemyInfo.GeneralStats.DoesEnemyExplodeOnDeath;
        GetComponents();
        _enemyMovement.AssignEnemyMovementInfo(EnemyInfo.MovementStats);

        OnEnemyAliveStateChange(gameObject, true);
    }

    private void GetComponents()
    {
        _animator = GetComponentInChildren<Animator>();
        _weapon = GetComponentInChildren<EnemyWeaponController>();
        _enemyMovement = GetComponent<EnemyMovement>();

        if (_doesEnemyExplodeOnDeath)
        {
            _explosion = GetComponent<ExplodingObject>();
        }

        else
        {
            _crashParticles = transform.GetComponentInChildren<ParticleSystem>().gameObject;
            if (_crashParticles != null)
            {
                _crashParticles.SetActive(false);
            }
        }
    }

    public virtual void Damage(float damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            if (_doesEnemyExplodeOnDeath)
            {
                Explode();
                _doesEnemyExplodeOnDeath = false;
            }

            else
            {
                Crash();
            }
        }
    }

    private void Explode()
    {
        _explosion.Explode(Destroy);
    }

    protected virtual void Crash()
    {
        if (_enemyMovement != null)
        {
            _enemyMovement.IsEnemyDead = true;
        }

        if (_weapon != null)
        {
            _weapon.gameObject.SetActive(false);
        }

        if (_animator != null)
        {
            _animator.enabled = false;
        }

        _spriteRenderer.sprite = _deadSprite;
        _spriteRenderer.sortingOrder = -1;
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
}

[Serializable]
public struct EnemyStats
{
    public int MaxHealth;
    public bool DoesEnemyExplodeOnDeath;
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