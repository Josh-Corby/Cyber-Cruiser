using UnityEngine;

public class ClusterObject : Enemy, IDamageable
{
    [SerializeField] private GameObject _objectToSpawn;
    private int _amountOfObjects;
    private float _spawnRadius = 0.5f;

    protected override void Awake()
    {
        base.Awake();
        AssignInfo();
    }

    private void AssignInfo()
    {
        if (_unitInfo.spawnEnemy)
        {
            _objectToSpawn = EM.CreateEnemyFromSO(_unitInfo.enemyToSpawn);
        }
        else
        {
            _objectToSpawn = _unitInfo.objectToSpawn;
        }
        _amountOfObjects = _unitInfo.amountOfObjects;
    }

    public override void Damage(float damage)
    {
        base.Damage(damage);
    }

    public override void Destroy()
    {
        SpawnProjectile();
        base.Destroy();
    }

    private void SpawnProjectile()
    {
        for (int i = 0; i < _amountOfObjects; i++)
        {
            float angle = i * (360 / _amountOfObjects);

            float rad = angle * Mathf.Deg2Rad;

            float x = _spawnRadius * Mathf.Cos(rad);
            float y = _spawnRadius * Mathf.Sin(rad);
            Vector3 spawnPos = transform.position + new Vector3(x, y, 0);

            Quaternion spawnRotation = Quaternion.Euler(0, 0, angle);
            GameObject _go = Instantiate(_objectToSpawn, spawnPos, spawnRotation);
        }
    }
}
