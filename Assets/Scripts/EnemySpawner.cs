using System;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : GameBehaviour
{
    private const int MIN_ENEMY_SPAWN_DISTANCE = 2;

    #region Fields
    public EnemyCategory[] enemyCategories;
    [SerializeField] private Vector3 spawnArea;
    [Range(0, 1)][HideInInspector] public float spawnerWeight;
    private List<Vector3> spawnPositions = new();
    private bool isSpawnPositionValid;
    private int _enemiesToSpawn;
    private float _speedModifier;
    [SerializeField] private float totalCategoryWeight;
    #endregion

    #region Properties
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
    #endregion

    #region Actions
    public static event Action<EnemyScriptableObject> OnBossSpawned = null;
    #endregion

    public void StartSpawnProcess()
    {
        spawnPositions.Clear();
        for (int i = 0; i < EnemiesToSpawn; i++)
        {
            EnemyCategory randomCategory = GetRandomWeightedCategory();
            EnemyScriptableObject randomEnemyInfo = GetRandomWeightedType(randomCategory);
            GameObject enemy = EM.CreateEnemyFromSO(randomEnemyInfo);
            Vector3 spawnPosition = ValidateSpawnPosition(GetRandomSpawnPosition());
            spawnPositions.Add(spawnPosition);
            SpawnEnemy(enemy, spawnPosition);
        }
        EnemiesToSpawn = 0;
    }
    private EnemyCategory GetRandomWeightedCategory()
    {
        float value = Random.value;

        for (int i = 0; i < enemyCategories.Length; i++)
        {
            if (value < enemyCategories[i].CategoryWeight)
            {
                GetRandomWeightedType(enemyCategories[i]);
                return enemyCategories[i];
            }
            value -= enemyCategories[i].CategoryWeight;
        }
        return enemyCategories[enemyCategories.Length - 1];
    }

    private EnemyScriptableObject GetRandomWeightedType(EnemyCategory category)
    {
        float value = Random.value;

        for (int i = 0; i < category.CategoryTypes.Length; i++)
        {
            if (value < category.CategoryTypes[i].spawnWeight)
            {
                EnemyScriptableObject enemy = category.CategoryTypes[i].EnemySO;
                return enemy;
            }
            value -= category.CategoryTypes[i].spawnWeight;
        }
        return null;
    }

    public Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(-spawnArea.x / 2, spawnArea.x / 2);
        float y = Random.Range(-spawnArea.y / 2, spawnArea.y / 2);
        float z = 0;
        Vector3 spawnPosition = new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z + z);
        return spawnPosition;
    }

    private Vector3 ValidateSpawnPosition(Vector3 enemySpawnPosition)
    {
        if (spawnPositions.Count == 0)
        {
            spawnPositions.Add(enemySpawnPosition);
            return enemySpawnPosition;
        }

        isSpawnPositionValid = false;
        while (!isSpawnPositionValid)
        {
            foreach (Vector3 spawnPosition in spawnPositions)
            {
                if (spawnPosition == enemySpawnPosition)
                {
                    continue;
                }

                if (Vector3.Distance(enemySpawnPosition, spawnPosition) > MIN_ENEMY_SPAWN_DISTANCE)
                {
                    continue;
                }

                else
                {
                    enemySpawnPosition = ValidateSpawnPosition(GetRandomSpawnPosition());
                    break;
                }
            }

            isSpawnPositionValid = true;
            spawnPositions.Add(enemySpawnPosition);
        }
        return enemySpawnPosition;
    }

    private void SpawnEnemy(GameObject enemy, Vector3 position)
    {
        Instantiate(enemy, position, transform.rotation);
        AddSpeedModifier(enemy);
    }

    public GameObject SpawnEnemyAtRandomPosition(GameObject enemy)
    {
        Instantiate(enemy, GetRandomSpawnPosition(), transform.rotation);
        AddSpeedModifier(enemy);
        return enemy;
    }

    private void AddSpeedModifier(GameObject enemy)
    {
        if (_speedModifier > 0)
        {
            if (enemy.TryGetComponent<EnemyMovement>(out var enemyMovement))
            {
                enemyMovement.ApplySpeedModifier(_speedModifier);
            }
        }
    }

    public void SpawnBoss(EnemyScriptableObject bossInfo)
    {
        GameObject bossObject = bossInfo.unitPrefab;
        bossObject.GetComponent<Enemy>()._unitInfo = bossInfo;
        Instantiate(bossObject, transform.position, transform.rotation);
        AddSpeedModifier(bossObject);
        OnBossSpawned(bossInfo);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, spawnArea);
    }

    public void OnInspectorUpdate()
    {
        ValidateWeights();
        ValidateTypeWeights();
    }

    public void ValidateWeights()
    {
        totalCategoryWeight = ValidateSpawnRates(enemyCategories, typeof(EnemyCategory), "CategoryWeight", totalCategoryWeight);
    }

    public void ResetCategoryWeights()
    {
        for (int i = 0; i < enemyCategories.Length; i++)
        {
            enemyCategories[i].CategoryWeight = 0;
        }
    }

    private void ValidateTypeWeights()
    {
        for (int i = 0; i < enemyCategories.Length; i++)
        {
            EnemyCategory category = enemyCategories[i];
            category.TotalTypeWeights = ValidateSpawnRates(category.CategoryTypes, typeof(EnemyType), "spawnWeight", category.TotalTypeWeights);
        }
    }

    public void ResetTypeWeights()
    {
        for (int i = 0; i < enemyCategories.Length; i++)
        {
            EnemyCategory currentCategory = enemyCategories[i];
            for (int x = 0; x < currentCategory.CategoryTypes.Length; x++)
            {
                currentCategory.CategoryTypes[x].spawnWeight = 0;
            }
        }
    }
}
[System.Serializable]
public struct EnemySpawnerInfo
{
    public EnemySpawner spawner;
    [Range(0, 1)]
    public float SpawnerWeight;
}