using CyberCruiser.Audio;
using UnityEngine;

namespace CyberCruiser
{
    [RequireComponent(typeof(SoundControllerBase))]
    public class ExplodingObject : GameBehaviour
    {
        private GameObject _explosionGraphic;

        [SerializeField] private EnemyScriptableObject _owner;

        public EnemyScriptableObject Owner { get => _owner; set => _owner = value; }
        #region Explosion Info
        [Header("Explosion Info")]

        [Tooltip("Radius that targeted units need to be in to take damage")]
        [SerializeField] private float _explosionRadius;

        [Tooltip("How much damage the explosion does")]
        [SerializeField] private float _explosionDamage;

        [Tooltip("Layer mask for objects to be hit by the explosion")]
        [SerializeField] private LayerMask _explosionMask;

        [SerializeField] private bool _explodeOnSpawn = true;

        [SerializeField] private bool _destroyOnExplode = true;
        #endregion

        #region Cluster Info
        [Header("Cluster Info")]

        [Tooltip("Does the explosion cluster, creating new objects")]
        [SerializeField] private bool _clusterOnDeath;

        [Tooltip("Does the cluster spawn a unit or not")]
        [SerializeField] private bool isClusterSpawningAUnit;

        [Tooltip("The unit the cluster spawns")]
        [SerializeField] private EnemyScriptableObject enemyToSpawn;

        [Tooltip("The non-unit object The Cluster Spawns")]
        [SerializeField] private GameObject objectToSpawn;

        [Tooltip("How many objects does the cluster spawn")]
        [SerializeField] private int _amountOfObjectsToSpawn;

        [Tooltip("How far from the origin of the explosion are the objects instantiated")]
        [SerializeField] private float _spawnRadius = 0.5f;

        private GameObject _objectSpawnedOnCluster;
        #endregion

        #region Sound Info
        [Header("Sound Info")]
        [SerializeField] private SoundControllerBase _soundController;
        [SerializeField] private ClipInfo _explosionClip;
        #endregion

        private void Awake()
        {
            _explosionGraphic = transform.GetChild(0).gameObject;
            _soundController = GetComponent<SoundControllerBase>();
        }

        private void Start()
        {
            if (_explosionGraphic.activeSelf)
            {
                _explosionGraphic.SetActive(false);
            }

            if (_clusterOnDeath)
            {
                ValidateObjectToSpawn();
            }

            if (_explodeOnSpawn)
            {
                Explode();
            }
        }

        private void ValidateObjectToSpawn()
        {
            if (isClusterSpawningAUnit)
            {
                _objectSpawnedOnCluster = EnemyManagerInstance.CreateEnemyFromSO(enemyToSpawn);

                if (_objectSpawnedOnCluster.TryGetComponent<Enemy>(out var enemy)){
                    enemy.Owner = _owner;
                }
            }

            else
            {
                _objectSpawnedOnCluster = objectToSpawn;
            }
        }

        public void Explode()
        {
            _explosionGraphic.SetActive(true);
            _soundController.PlayNewClip(_explosionClip);

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _explosionRadius, _explosionMask);

            foreach (Collider2D collider in colliders)
            {
                if (collider.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.Damage(_explosionDamage, _owner);
                }
            }

            if (_clusterOnDeath)
            {
                SpawnProjectile();
            }

            if (_destroyOnExplode)
            {
                Invoke(nameof(Destroy), 2f);
            }
        }

        private void SpawnProjectile()
        {
            for (int i = 0; i < _amountOfObjectsToSpawn; i++)
            {
                float angle = i * (360 / _amountOfObjectsToSpawn);

                float rad = angle * Mathf.Deg2Rad;

                float x = _spawnRadius * Mathf.Cos(rad);
                float y = _spawnRadius * Mathf.Sin(rad);
                Vector3 spawnPos = transform.position + new Vector3(x, y, 0);

                Quaternion spawnRotation;

                if (Mathf.Abs(angle) == 90)
                {
                    spawnRotation = Quaternion.Euler(0, 180, angle);

                }

                else if (Mathf.Abs(angle) == 180)
                {
                    spawnRotation = Quaternion.Euler(180, 0, angle);
                }

                else
                {
                    spawnRotation = Quaternion.Euler(0, 0, angle);
                }

               // Quaternion spawnRotation = Quaternion.Euler(0, 0, angle);
                GameObject _go = Instantiate(_objectSpawnedOnCluster, spawnPos, spawnRotation);
            }
        }

        private void Destroy()
        {
            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, _explosionRadius);
        }
    }
}