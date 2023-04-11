using System;
using System.Collections.Generic;

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

    public EnemyCategory[] enemyCategories;
    [SerializeField] private float totalCategoryWeight;
  
   
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
        spawnPositions.Clear();
        for (int i = 0; i < EnemiesToSpawn; i++)
        {
            EnemyCategory randomCategory = GetRandomWeightedCategory();
            GameObject randomEnemy = GetRandomWeightedType(randomCategory);
            Vector3 spawnPosition = ValidateSpawnPosition(GetRandomSpawnPosition());
            SpawnEnemy(randomEnemy, spawnPosition);     
        }    
        EnemiesToSpawn = 0;
    }

    private EnemyCategory GetRandomWeightedCategory()
    {
        float value = Random.value;

        for (int i = 0; i < enemyCategories.Length; i++)
        {
            if(value < enemyCategories[i].CategoryWeight)
            {
                GetRandomWeightedType(enemyCategories[i]);
                return enemyCategories[i];
            }
            value -= enemyCategories[i].CategoryWeight;
        }
        return enemyCategories[enemyCategories.Length-1];
    }

    private GameObject GetRandomWeightedType(EnemyCategory category)
    {
        float value = Random.value;

        for (int i = 0; i < category.CategoryTypes.Length; i++)
        {
            if(value < category.CategoryTypes[i].spawnWeight)
            {
                GameObject enemy = category.CategoryTypes[i].Enemy;
                return enemy;
            }
            value -= category.CategoryTypes[i].spawnWeight;
        }
        return null;
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
            if(spawnPositions.Count > 0)
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

            spawnPositions.Add(enemySpawnPosition);
        }
        return enemySpawnPosition;
    }

    private void SpawnEnemy(GameObject _enemy, Vector3 position)
    {
        GameObject enemy = Instantiate(_enemy, position, transform.rotation);
        AddSpeedModifier(enemy);

        //Rotate enemies to face correct direction on spawn
        enemy.transform.up = transform.right;
        OnEnemySpawned(ESM.enemiesAlive, enemy);
    }

    public GameObject SpawnEnemyAtRandomPosition(GameObject _enemy)
    {
        GameObject enemy = Instantiate(_enemy, GetRandomSpawnPosition(), transform.rotation);
        enemy.transform.up = transform.right;
        OnEnemySpawned(ESM.enemiesAlive, enemy);
        return enemy;
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