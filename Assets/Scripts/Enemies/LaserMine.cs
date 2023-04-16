using UnityEngine;

public class LaserMine : Enemy, IDamageable
{
    [SerializeField] private int objectsToSpawn;
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
        for (int i = 0; i < objectsToSpawn; i++)
        {
            
            float angle = i * (360/objectsToSpawn);

            float rad = angle * Mathf.Deg2Rad;

            float x = radius * Mathf.Cos(rad);
            float y = radius * Mathf.Sin(rad);
            Vector3 spawnPos = transform.position + new Vector3(x, y, 0);

            Quaternion spawnRotation = Quaternion.Euler(0, 0, angle);

            GameObject _go = Instantiate(laserProjectile, spawnPos, spawnRotation);
        }
    }  
}
