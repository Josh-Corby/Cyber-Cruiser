using UnityEngine;

public abstract class ShieldControllerBase : MonoBehaviour, IShield
{
    [SerializeField] protected Collider2D _unitCollider;
    [SerializeField] protected Shield _shields;

    [SerializeField] protected bool _shieldsActiveOnSpawn;

    public bool shieldsActive;
    public bool reflectorShield;

    protected void Awake()
    {
        _unitCollider = GetComponentInParent<Collider2D>();
        _shields = GetComponentInChildren<Shield>();
    }
    protected void Start()
    {
        if(!_shieldsActiveOnSpawn)
        {
            DeactivateShields();
        }
        else if (_shieldsActiveOnSpawn)
        {
            ActivateShields();
        }
    }

    public virtual void ActivateShields()
    {
        shieldsActive = true;
        _shields.EnableShields();
        _unitCollider.enabled = false;
    }

    public virtual void DeactivateShields()
    {
        shieldsActive = false;
        _shields.DisableShields();
        _unitCollider.enabled = true;
    }


    public virtual void ProcessCollision(GameObject collider, int damage)
    {
        DeactivateShields();
        if (collider.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.Damage(damage);
            DeactivateShields();
        }

        else if (collider.TryGetComponent<IShield>(out var shield))
        {
            shield.DeactivateShields();
            DeactivateShields();
        }

        else if (collider.TryGetComponent<Bullet>(out var bullet))
        {
            if (reflectorShield)
            {
                ReflectProjectile(bullet);
            }
            else
            {
                ReduceShields();
                Destroy(bullet.gameObject);
            }
        }
    }


    public virtual void ReduceShields()
    {
        throw new System.NotImplementedException();
    }

    public virtual void ReflectProjectile(Bullet bulletToReflect)
    {
        //Debug.Log(bulletToReflect + "reflected");
        bulletToReflect.gameObject.transform.right = transform.right;
        bulletToReflect.speed /= 2;
    }
}
