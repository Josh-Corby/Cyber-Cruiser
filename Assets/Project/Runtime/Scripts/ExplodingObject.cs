using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingObject : GameBehaviour
{
    #region Explosion
    [SerializeField] private GameObject _explosionEffect;
    [SerializeField] private float _explosionRadius;
    [SerializeField] private float _explosionDamage;
    [SerializeField] private LayerMask _explosionMask;
    #endregion

    #region Cluster
    [SerializeField] private bool clusterOnDeath;
    [SerializeField] private bool isClusterSpawningAUnit;
    [SerializeField] private EnemyScriptableObject enemyToSpawn;
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private int _amountOfObjectsToSpawn;
    [SerializeField] private float _spawnRadius = 0.5f;
    private GameObject _objectToSpawn;
    #endregion

    private void Awake()
    {
        if(clusterOnDeath)
        {
            ValidateObjectToSpawn();
        }
    }

    private void ValidateObjectToSpawn()
    {
        if (isClusterSpawningAUnit)
        {
            _objectToSpawn = EM.CreateEnemyFromSO(enemyToSpawn);
        }

        else
        {
            _objectToSpawn = objectToSpawn;
        }
    }

    public void Explode(Action destroyBaseObjectCallBack)
    {
        GameObject explosionEffect = Instantiate(_explosionEffect, transform);
        explosionEffect.GetComponent<ExplosionGraphic>().ExplosionRadius = _explosionRadius;
        explosionEffect.transform.SetParent(null);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _explosionRadius, _explosionMask);

        foreach (Collider2D collider in colliders)
        {
            if(collider.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.Damage(_explosionDamage);
            }
        }

        if (clusterOnDeath)
        {
            SpawnProjectile();
        }

        destroyBaseObjectCallBack();
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

            Quaternion spawnRotation = Quaternion.Euler(0, 0, angle);
            GameObject _go = Instantiate(_objectToSpawn, spawnPos, spawnRotation);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
}
