using UnityEngine;

public abstract class ShieldControllerBase : GameBehaviour, IShield
{
    [SerializeField] protected Collider2D _unitCollider;
    [SerializeField] protected Shield _shields;
    [SerializeField] protected bool _shieldsActiveOnSpawn;
    [SerializeField] protected bool isShieldImmuneToDamage;

    [SerializeField] protected int _shieldMaxStrength;
    [SerializeField] protected float _shieldCurrentStrength;
    [SerializeField] protected float _shieldCollisionDamage;

    [SerializeField] protected float _shieldRendererMaxAlpha;
    [SerializeField] protected float _shieldRendererCurrentAlpha;
    [SerializeField] protected GameObject _collisionParticles;

    public bool _shieldsActive;
    public bool reflectorShield;

    #region Properties
    protected virtual bool ShieldsActive
    {
        get
        {
            return _shieldsActive;
        }
        set
        {
            _shieldsActive = value;
        }
    }

    public int ShieldMaxStrength
    {
        get
        {
            return _shieldMaxStrength;
        }
        set
        {
            _shieldMaxStrength = value;
        }
    }

    public float ShieldCurrentStrength
    {
        get
        {
            return _shieldCurrentStrength;
        }
        set
        {
            _shieldCurrentStrength = value;
        }
    }

    public float ShieldCollisionDamage
    {
        get
        {
            return _shieldCollisionDamage;
        }
        set { }
    }

    public float ShieldRendererMaxAlpha
    {
        get
        {
            return _shieldRendererMaxAlpha;
        }
        set
        {
            _shieldRendererMaxAlpha = value;
        }
    }

    public float ShieldRendererCurrentAlpha
    {
        get
        {
            return _shieldRendererCurrentAlpha;
        }
        set
        {
            _shieldRendererCurrentAlpha = value;
            _shields.SpriteRendererColour = new Color(_shields.SpriteRendererColour.r, _shields.SpriteRendererColour.g, _shields.SpriteRendererColour.b, value);
        }
    }
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
        SetRendererMaxAlpha();
    }

    protected void SetRendererMaxAlpha()
    {
        ShieldRendererMaxAlpha = _shields.SpriteRendererColour.a;
    }

    public virtual void ActivateShields()
    {
        ShieldsActive = true;
        _shields.EnableShields();
        _unitCollider.enabled = false;
        ShieldCurrentStrength = ShieldMaxStrength;
    }

    public virtual void DeactivateShields()
    {
        ShieldsActive = false;
        _shields.DisableShields();
        _unitCollider.enabled = true;
    }

    public virtual void ProcessCollision(GameObject collider, Vector2 collisionPoint)
    {
        if (collider.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.Damage(ShieldCollisionDamage);
            if (!isShieldImmuneToDamage)
            {
                ReduceShields(1);
            }

            if (_collisionParticles != null)
            {
                GameObject collisionParticles = Instantiate(_collisionParticles, collisionPoint, Quaternion.identity);
                collisionParticles.transform.parent = null;
            }
        }

        else if (collider.TryGetComponent<IShield>(out var shield))
        {
            shield.ReduceShields(ShieldCollisionDamage);
            if (!isShieldImmuneToDamage)
            {
                ReduceShields(shield.ShieldCollisionDamage);
            }
        }

        else if (collider.TryGetComponent<Bullet>(out var bullet))
        {
            if (reflectorShield)
            {
                ReflectProjectile(bullet);
            }
            else
            {
                if (!isShieldImmuneToDamage)
                {
                    ReduceShields(bullet.damage);
                }
                Destroy(bullet.gameObject);
            }
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

        float currentPercentStrength = ShieldCurrentStrength / ShieldMaxStrength;
        float targetAlpha = ShieldRendererMaxAlpha * currentPercentStrength;
        ShieldRendererCurrentAlpha = targetAlpha;
    }


    public virtual void ReflectProjectile(Bullet bulletToReflect)
    {
        bulletToReflect.gameObject.transform.right = transform.right;
        bulletToReflect.speed /= 2;
    }
}
