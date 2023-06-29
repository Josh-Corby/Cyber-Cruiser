using CyberCruiser.Audio;
using UnityEngine;

namespace CyberCruiser
{
    [RequireComponent(typeof(SoundControllerBase))]
    public class ExplodingObject : GameBehaviour
    {
        #region Explosion
        [SerializeField] private float _explosionRadius;
        [SerializeField] private float _explosionDamage;
        [SerializeField] private LayerMask _explosionMask;
        [SerializeField] private float _explosionDuration;
        #endregion

        [SerializeField] private SoundControllerBase _soundController;
        [SerializeField] private ClipInfo _explosionClip;

        #region Cluster
        [SerializeField] private bool clusterOnDeath;
        [SerializeField] private bool isClusterSpawningAUnit;
        [SerializeField] private EnemyScriptableObject enemyToSpawn;
        [SerializeField] private GameObject objectToSpawn;
        [SerializeField] private int _amountOfObjectsToSpawn;
        [SerializeField] private float _spawnRadius = 0.5f;
        private GameObject _objectSpawnedOnCluster;
        #endregion

        private void Awake()
        {
            _soundController = GetComponent<SoundControllerBase>();
        }

        private void Start()
        {
            if (clusterOnDeath)
            {
                ValidateObjectToSpawn();
            }

            Explode();
            _soundController.PlayNewClip(_explosionClip);
        }

        private void ValidateObjectToSpawn()
        {
            if (isClusterSpawningAUnit)
            {
                _objectSpawnedOnCluster = EnemyManagerInstance.CreateEnemyFromSO(enemyToSpawn);
            }

            else
            {
                _objectSpawnedOnCluster = objectToSpawn;
            }
        }

        public void Explode()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _explosionRadius, _explosionMask);

            foreach (Collider2D collider in colliders)
            {
                if (collider.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.Damage(_explosionDamage);
                }
            }

            if (clusterOnDeath)
            {
                SpawnProjectile();
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

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, _explosionRadius);
        }
    }
}