using UnityEngine;

namespace CyberCruiser
{
    public class ShieldControllerBase : GameBehaviour
    {
        #region References
        [Tooltip("Unit collider to be toggled when the shield is enabled")]
        [SerializeField] protected Collider2D _unitCollider;
        [SerializeField] protected GameObject _collisionParticles;
        protected Shield _shields;
        #endregion

        #region Fields
        protected bool _shieldsActive;
        private float _shieldCurrentStrength;

        [Tooltip("Strength of shield when activated")]
        [SerializeField] private int _shieldMaxStrength;

        [Tooltip("Damage shield does when colliding with a damageable target")]
        [SerializeField] protected float _shieldCollisionDamage;

        [Tooltip("Does the shield reflect projectiles")]
        [SerializeField] protected bool _reflectorShield;

        [Tooltip("Is the shield active when it is enabled")]
        [SerializeField] protected bool _shieldsActiveOnSpawn;

        [Tooltip("Is the shield immune to its duration being reduced by damage")]
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

        public float ShieldCollisionDamage { get => _shieldCollisionDamage; set => _shieldCollisionDamage = value; }
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
                CheckShieldPickups();
            }
        }

        protected virtual void CheckShieldPickups()
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
}