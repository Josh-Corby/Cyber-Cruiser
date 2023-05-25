using System;
using UnityEngine;

namespace CyberCruiser
{
    [RequireComponent(typeof(EnemyMovement))]
    public class Enemy : GameBehaviour, IDamageable
    {
        protected const string DEAD_ENEMY_LAYER_NAME = "DeadEnemy";

        #region References
        public EnemyScriptableObject EnemyInfo;
        private EnemyMovement _enemyMovement;
        private EnemyWeaponController _weapon;

        private GameObject _crashParticles;
        private GameObject _explosion;
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
        public static event Action<EnemyTypes> OnEnemyDeath = null;
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
            _explosion = EnemyInfo.GeneralStats.Explosion;

            GetComponents();
            _enemyMovement.AssignEnemyMovementInfo(EnemyInfo.MovementStats);
            OnEnemyAliveStateChange(gameObject, true);
        }

        private void GetComponents()
        {
            _animator = GetComponentInChildren<Animator>();
            _weapon = GetComponentInChildren<EnemyWeaponController>();
            _enemyMovement = GetComponent<EnemyMovement>();

            if(!_doesEnemyExplodeOnDeath)
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
            GameObject explosion = Instantiate(_explosion, transform.position, Quaternion.identity);
            explosion.transform.parent = null;
            Destroy();
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
            OnEnemyDeath?.Invoke(EnemyInfo.GeneralStats.Type);
            Destroy(gameObject);
        }
    }

        [Serializable]
        public struct EnemyStats
        {
            public EnemyTypes Type;
            public int MaxHealth;
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