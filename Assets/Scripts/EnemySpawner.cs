using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : GameBehaviour
{
    private const int MIN_ENEMY_SPAWN_DISTANCE = 10;
    public static event Action<List<GameObject>, GameObject> OnEnemySpawned = null;
    public static event Action<Enemy> OnBossSpawned = null;

    [Range(0,1)]
    [HideInInspector] public float spawnerWeight;
    private List<Vector3> spawnPositions = new();
    private bool isSpawnPositionValid;
    [SerializeField] private Vector3 spawnArea;
    private float _speedModifier;
    private int _enemiesToSpawn;

    public EnemyCategory[] EnemyCategories;
    [SerializeField] private float _totalCategoryWeight;
    /*
    private GameObject EnemyIndicator;
    private Vector3 EnemyIndicatorPosition;
    private float IndicatorAngle;
    */
   
    public int EnemiesToSpawn
    {
        get
        {
            return _enemiesToSpawn;
        }
        set
        {
            _enemiesToSpawn = value;
        }
    }

    public float SpeedModifier
    {
        get
        {
            return _speedModifier;
        }
        set
        {
            _speedModifier = value;
        }
    }
   
    public void StartSpawnProcess()
    {
        for (int i = 0; i < EnemiesToSpawn; i++)
        {
            GameObject enemy = GetRandomType(GetRandomEnemyCategory());
            Vector3 spawnPosition = ValidateSpawnPosition(GetRandomSpawnPosition());
            SpawnEnemy(enemy,spawnPosition);     
        }    
        EnemiesToSpawn = 0;
    }

    private EnemyCategory GetRandomEnemyCategory()
    {
        EnemyCategory randomCategory = EnemyCategories[Random.Range(0, EnemyCategories.Length)];
        return randomCategory;
    }

    private GameObject GetRandomType(EnemyCategory category)
    {
        EnemyType randomType = category.CategoryTypes[Random.Range(0, category.CategoryTypes.Length)];
        return randomType.Enemy;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(-spawnArea.x / 2, spawnArea.x / 2);
        float y = Random.Range(-spawnArea.y / 2, spawnArea.y / 2);
        float z = 0;
        Vector3 spawnPosition = new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z + z);
        return spawnPosition;
    }

    private Vector3 ValidateSpawnPosition(Vector3 enemySpawnPosition)
    {
        if(spawnPositions.Count == 0)
        {
            return enemySpawnPosition;
        }

        isSpawnPositionValid = false;

        while (!isSpawnPositionValid)
        {
            foreach (Vector3 spawnPosition in spawnPositions)
            {
                if (Vector3.Distance(enemySpawnPosition, spawnPosition) > MIN_ENEMY_SPAWN_DISTANCE)
                {
                    continue;
                }

                else
                {
                    enemySpawnPosition = GetRandomSpawnPosition();
                    break;
                }
            }
            isSpawnPositionValid = true;
        }
        spawnPositions.Clear();
        return enemySpawnPosition;
    }

    private void SpawnEnemy(GameObject _enemy, Vector3 position)
    {
        GameObject enemy = Instantiate(_enemy, position, transform.rotation);
        AddSpeedModifier(enemy);
        OnEnemySpawned(ESM.enemiesAlive, enemy);
    }

    private void AddSpeedModifier(GameObject enemy)
    {
        if (_speedModifier > 0)
        {
            if (enemy.TryGetComponent<EnemyMovement>(out var enemyMovement))
            {
                enemyMovement._speed += _speedModifier;
            }

            else if (enemy.TryGetComponent<BossMovement>(out var bossMovement))
            {
                bossMovement.speed += _speedModifier;
            }

            else return;
        }
    }

    public void StartBossSpawn(GameObject bossToSpawn)
    {
        //CreateIndicator(transform.position);
        SpawnBoss(transform.position, bossToSpawn);
    }

    private void SpawnBoss(Vector3 spawnPosition, GameObject bossToSpawn)
    {
        GameObject boss = Instantiate(bossToSpawn, spawnPosition, transform.rotation);
        AddSpeedModifier(boss);
        OnEnemySpawned(ESM.enemiesAlive, boss);
        Enemy bossInfo = boss.GetComponent<Enemy>();
        boss.name = bossInfo.unitName;
        OnBossSpawned(bossInfo);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, spawnArea);
    }

    public void OnInspectorUpdate()
    {
        CheckCategoryWeights();
        //ValidateWeights();
    }

    public void CheckCategoryWeights()
    {
        for (int i = 0; i < EnemyCategories.Length; i++)
        {
            Debug.Log(EnemyCategories[i].CategoryWeight);
        }
    }

    public void ResetCategoryWeights()
    {
        for (int i = 0; i < EnemyCategories.Length; i++)
        {
            EnemyCategories[i].CategoryWeight = 0;
        }

    }

    public void CheckTypeWeights()
    {

    }

    public void ResetTypeWeights()
    {

    }

    public void ValidateWeights()
    {
        ValidateCategoryWeights(EnemyCategories, typeof(EnemyCategory), "CategoryWeight", _totalCategoryWeight);
    }

    public void ValidateCategoryWeights<T>(T[] structArray, Type structType, string fieldName, float totalWeight) where T : struct
    {
        totalWeight = 0;
        var valueList = new List<float>();


        foreach (T _struct in structArray)
        {
            if(_struct.GetType() != structType)
            {
                continue;
            }

            FieldInfo field = structType.GetField(fieldName);
            if (field != null && field.FieldType == typeof(float))
            {
                object boxedValue = field.GetValue(_struct);
                float value = (float)boxedValue;
                valueList.Add(value);
                Debug.Log(value);
            }

        }
        for (int i = 0; i < valueList.Count; i++)
        {
            totalWeight += valueList[i];
        }
        if (totalWeight > 1)
        {
            float factor = 1 / totalWeight;
            for (int i = 0; i < valueList.Count; i++)
            {
                valueList[i] *= factor;
            }
        }
        for (int i = 0; i < valueList.Count; i++)
        {
            valueList[i] = (float)Math.Round(valueList[i], 3);
        }
        Debug.Log(totalWeight);
        int x = 0;
        foreach (T _struct in structArray)
        {
            if (_struct.GetType() != structType)
            {
                continue;
            }

            FieldInfo field = structType.GetField(fieldName);
            if (field != null && field.FieldType == typeof(float))
            {
                field.SetValue(_struct, valueList[x]);
                x++;
            }
        }
    }

    /*
    private void CreateIndicator(Vector2 position)
    {
        GameObject Indicator = Instantiate(EnemyIndicator, transform.position, Quaternion.identity);

        Indicator.transform.position += EnemyIndicatorPosition;

        if (EnemyIndicatorPosition.x == 0)
        {
            Indicator.transform.position += new Vector3(position.x, 0);
        }


        if (EnemyIndicatorPosition.y == 0)
        {
            Indicator.transform.position += new Vector3(0, position.y);
        }

        Indicator.transform.rotation = Quaternion.Euler(0, 0, IndicatorAngle);
        Indicator.GetComponent<EnemyIndicator>().IndicatorTimer(spawnDelay));
    }

    */
}
