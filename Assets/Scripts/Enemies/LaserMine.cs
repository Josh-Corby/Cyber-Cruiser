using UnityEngine;
using System;
using System.Collections.Generic;

public class LaserMine : Enemy, IDamageable
{
    public static event Action<List<GameObject>, GameObject> OnEnemySpawned = null;

    [SerializeField] private GameObject laserProjectile;
    private bool bulletCollision = false;
    [SerializeField] private bool spawnEnemies;
    private float radius = 0.5f;

    protected override void Start()
    {
        bulletCollision = false;
    }
    public override void Damage(float damage)
    {
        bulletCollision = true;
        base.Damage(damage);
    }

    public override void Destroy()
    {
        if (bulletCollision)
        {
            SpawnProjectile();
        }
        base.Destroy();
    }

    private void SpawnProjectile()
    {
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f;

            float rad = angle * Mathf.Deg2Rad;

            float x = radius * Mathf.Cos(rad);
            float y = radius * Mathf.Sin(rad);
            Vector3 spawnPos = transform.position + new Vector3(x, y, 0);

            Quaternion spawnRotation = Quaternion.Euler(0, 0, angle);

            GameObject _go = Instantiate(laserProjectile, spawnPos, spawnRotation);
            OnEnemySpawned(ESM.enemiesAlive, _go);

        }
    }
}
