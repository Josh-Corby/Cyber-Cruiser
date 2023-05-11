using UnityEngine;

public class ShieldControllerBase : GameBehaviour
{
    #region References
    [SerializeField] protected Collider2D _unitCollider;
    [SerializeField] protected Shield _shields;
    [SerializeField] protected GameObject _collisionParticles;
    #endregion

    #region Fields
    protected bool _shieldsActive;
    [SerializeField] private int _shieldMaxStrength;
    [SerializeField] private float _shieldCurrentStrength;
    private float _shieldCollisionDamage;

    [SerializeField] protected bool _reflectorShield;
    [SerializeField] protected bool _shieldsActiveOnSpawn;
    [SerializeField] protected bool _isShieldImmuneToDamage;
    #endregion

    #region Properties
    protected virtual bool IsShieldsActive
    {
        get => _shieldsActive;
        set
        {
            if (value == true)
            {
                ShieldCurrentStrength = ShieldMaxStrength;
            }
            _shieldsActive = value;
            _shields.ToggleShields(value);
        }
    }

    protected int ShieldMaxStrength { get => _shieldMaxStrength; set => _shieldMaxStrength = value; }

    protected float ShieldCurrentStrength { get => _shieldCurrentStrength; set => _shieldCurrentStrength = value; }

    protected float ShieldCollisionDamage { get => _shieldCollisionDamage; set => _shieldCollisionDamage = value; }
    #endregion

    protected virtual void Awake()
    {
        _unitCollider = GetComponentInParent<Collider2D>();
        _shields = GetComponentInChildren<Shield>();
    }

    protected void Start()
    {
        if (!_shieldsActiveOnSpawn)
        {
            DeactivateShields();
        }
        else if (_shieldsActiveOnSpawn)
        {
            ActivateShields();
        }
    }

    protected virtual void ActivateShields()
    {
        IsShieldsActive = true;
        _unitCollider.enabled = false;
    }

    protected virtual void DeactivateShields()
    {
        IsShieldsActive = false;
        _unitCollider.enabled = true;
    }

    public virtual void ProcessCollision(GameObject collider, Vector2 collisionPoint)
    {
        if (collider.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.Damage(ShieldCollisionDamage);
            if (!_isShieldImmuneToDamage)
            {
                ReduceShields(1);
            }

            if (_collisionParticles != null)
            {
                GameObject collisionParticles = Instantiate(_collisionParticles, collisionPoint, Quaternion.identity);
                collisionParticles.transform.parent = null;
            }
        }

        else if (collider.TryGetComponent<ShieldControllerBase>(out var shield))
        {
            shield.ReduceShields(ShieldCollisionDamage);
            if (!_isShieldImmuneToDamage)
            {
                ReduceShields(shield.ShieldCollisionDamage);
            }
        }

        else if (collider.TryGetComponent<Bullet>(out var bullet))
        {
            if (!_isShieldImmuneToDamage)
            {
                ReduceShields(bullet.Damage);
            }
            if (_reflectorShield)
            {
                ReflectProjectile(bullet);
                return;
            }        
            Destroy(bullet.gameObject);
        }
    }

    public virtual void ReduceShields(float damage)
    {
        ShieldCurrentStrength -= damage;

        if (ShieldCurrentStrength <= 0)
        {
            DeactivateShields();
            return;
        }
        _shields.SetTargetAlpha(ShieldCurrentStrength, ShieldMaxStrength);
    }

    protected virtual void ReflectProjectile(Bullet bulletToReflect)
    {
        bulletToReflect.Reflect(gameObject);
    }
}
