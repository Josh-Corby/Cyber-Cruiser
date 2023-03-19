using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserMine : Enemy, IDamageable
{
    [SerializeField] private GameObject laserProjectile;
    private int rotationAngle;

    private float radius = 1;

    protected override void Start()
    {
        rotationAngle = 0;
    }

    public override void Destroy()
    {
        SpawnProjectile();
        base.Destroy();
    }

    private void SpawnProjectile()
    {
        for (int i = 0; i < 8; i++)
        {
            float angle = (i * 45f) * Mathf.Rad2Deg;

            float x = radius + Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);
            Vector3 spawnPos = transform.position + new Vector3(x, y, 0);

            Quaternion spawnRotation = Quaternion.Euler(0, 0, angle - 90f);

            Instantiate(laserProjectile, spawnPos, spawnRotation);
        }
    }
}
