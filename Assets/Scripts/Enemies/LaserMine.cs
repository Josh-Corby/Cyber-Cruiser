using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserMine : Enemy, IDamageable
{
    [SerializeField] private GameObject laserProjectile;
    private bool bulletCollision = false;

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

            Instantiate(laserProjectile, spawnPos, spawnRotation);
        }
    }
}