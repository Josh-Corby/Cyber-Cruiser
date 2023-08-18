using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

namespace CyberCruiser
{
    [RequireComponent(typeof(EnemyMovement))]
    public class Enemy : GameBehaviour, IDamageable
    {
        protected const string DEAD_ENEMY_LAYER_NAME = "DeadEnemy";

        #region References
        public EnemyScriptableObject EnemyInfo;
        protected EnemyMovement _enemyMovement;
        private EnemyWeapon _weapon;
        private GameObject _crashParticles;
        private EnemyStats _stats;
        #endregion

        private Animator _animator;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Sprite _deadSprite;
        private SimpleFlash _flash;

        protected float _currentHealth;
        public int RamDamage { get => _stats.RamDamage; }

        public EnemyScriptableObject Owner { get; set; }

        #region Actions
        public static event Action<GameObject> OnEnemySpawned = null;
        public static event Action<GameObject, EnemyTypes> OnEnemyDied = null;
        public static event Action<GameObject> OnEnemyCulled = null;
        #endregion

        protected virtual void Awake()
        {
            AssignEnemyInfo();
        }

        private void AssignEnemyInfo()
        {
            _stats = EnemyInfo.GeneralStats;
            gameObject.name = _stats.Name;
            _currentHealth = _stats.MaxHealth;

            if(Owner == null)
            {
                Owner = EnemyInfo;
            }

            GetComponents();
            _enemyMovement.AssignEnemyMovementInfo(EnemyInfo.MovementStats);
            OnEnemySpawned(gameObject);
        }

        private void GetComponents()
        {
            _flash = GetComponent<SimpleFlash>();
            _animator = GetComponentInChildren<Animator>();
            _weapon = GetComponentInChildren<EnemyWeapon>();
            _enemyMovement = GetComponent<EnemyMovement>();

            if(!_stats.DoesEnemyExplodeOnDeath)
            {
                _crashParticles = transform.GetComponentInChildren<ParticleSystem>().gameObject;
                if (_crashParticles != null)
                {
                    _crashParticles.SetActive(false);
                }
            }
        }

        public virtual void Damage(float damage, EnemyScriptableObject instigator)
        {
            _currentHealth -= damage;

            if(damage < 0)
            {
                if (_flash != null)
                {
                    _flash.Flash();
                }
            }       

            if (_currentHealth <= 0)
            {
                EnemyInfo.OnEnemyDied();
                if (_stats.DoesEnemyExplodeOnDeath)
                {
                    Explode();
                    _stats.DoesEnemyExplodeOnDeath = false;
                }

                else
                {
                    Crash();
                }
            }
        }

        private void Explode()
        {
            GameObject explosion = _stats.Explosion;
            if(explosion.TryGetComponent<ExplodingObject>(out var enemyExplosion))
            {
                enemyExplosion.Owner = Owner;
            }

            GameObject spawnedExplosion = Instantiate(explosion, transform.position, Quaternion.identity);
            spawnedExplosion.transform.parent = null;
            Destroy();
        }

        protected virtual void Crash()
        {
            //start crashing movement
            if (_enemyMovement != null)
            {
                _enemyMovement.IsEnemyDead = true;
            }

            //stop weapon firing
            if (_weapon != null)
            {
                _weapon.gameObject.SetActive(false);
            }

            //stop animating
            if (_animator != null)
            {
                _animator.enabled = false;
            }

            //change enemy sprite
            _spriteRenderer.sprite = _deadSprite;

            //move sprite behind other sprites
            _spriteRenderer.sortingOrder = -1;

            //enable crashing particles
            _crashParticles?.SetActive(true);

            //change object layer to layer that only collides with cull area
            gameObject.layer = LayerMask.NameToLayer(DEAD_ENEMY_LAYER_NAME);

            //remove enemy from enemies alive so it doesn't make boss spawner wait for it to fully die
            OnEnemyDied(gameObject, EnemyInfo.GeneralStats.Type);
        }

        public virtual void Destroy()
        {
            OnEnemyDied(gameObject, EnemyInfo.GeneralStats.Type);
            Destroy(gameObject);
        }

        public virtual void Cull()
        {
            OnEnemyCulled(gameObject);
            Destroy(gameObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.gameObject.TryGetComponent<PlayerManager>(out var playerManager))
            {          
                if(_stats.DoesEnemyExplodeOnDeath)
                {
                    Explode();
                    return; 
                }

                playerManager.Damage(RamDamage, EnemyInfo);
            }

            if(collision.gameObject.TryGetComponent<Shield>(out var playerShield))
            {
                if (_stats.DoesEnemyExplodeOnDeath)
                {
                    Explode();
                    return;
                }
            }
        }
    }

    [Serializable]
    public struct EnemyStats
    {
        public string Name;
        public EnemyTypes Type;
        public int MaxHealth;
        public int RamDamage;
        public bool DoesEnemyExplodeOnDeath;
        public GameObject Explosion;
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
}