using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class EnemySpawnerManager : GameBehaviour<EnemySpawnerManager>
{
    private const float SPAWN_ENEMY_INTERVAL_BASE = 1.5f;
    private const float SPAWN_ENEMY_REDUCTION = 0.1f;

    public bool spawnEnemies;
    public GameObject bossGoalPosition;
    public Transform dragonMovePoint;

    [SerializeField] private int enemiesToSpawn;
    [SerializeField] private float _enemySpawnInterval;
    [SerializeField] private EnemySpawner[] spawners;
    [SerializeField] private EnemySpawner bossSpawner;
    [SerializeField] private GameObject[] bossPrefabs;

    [HideInInspector] public List<GameObject> enemiesAlive = new();
    [HideInInspector] public List<GameObject> gunshipsAlive = new();
    [SerializeField] private List<GameObject> _bossesToSpawn = new();
    private List<EnemySpawner> _spawnersSpawning = new();

    public static event Action OnBossDied = null;  
    private Coroutine spawnEnemiesCoroutine;

    public float EnemySpawnInterval
    {
        get
        {
            return _enemySpawnInterval;
        }
        set
        {
            _enemySpawnInterval = value;
        }
    }
  
    private void OnEnable()
    {
        GameManager.OnLevelCountDownStart += RestartLevel;
        GameManager.OnBossDistanceReached += SpawnBoss;
        GameplayUIManager.OnCountdownDone += StartSpawningEnemies;
        PlayerManager.OnPlayerDeath += StopSpawningEnemies;

        EnemySpawner.OnEnemySpawned += AddEnemy;
        Enemy.OnEnemyDied += RemoveEnemy;

        Gunship.OnGunshipSpawn += AddEnemy;
        Gunship.OnGunshipDied += RemoveEnemy;

        LaserMine.OnEnemySpawned += AddEnemy;
    }

    private void OnDisable()
    {
        GameManager.OnLevelCountDownStart -= RestartLevel;
        GameManager.OnBossDistanceReached -= SpawnBoss;
        GameplayUIManager.OnCountdownDone -= StartSpawningEnemies;
        PlayerManager.OnPlayerDeath -= StopSpawningEnemies;

        EnemySpawner.OnEnemySpawned -= AddEnemy;
        Enemy.OnEnemyDied -= RemoveEnemy;

        Gunship.OnGunshipSpawn -= AddEnemy;
        Gunship.OnGunshipDied -= RemoveEnemy;
    }

    private void Start()
    {
        RestartLevel();
    }

    private void RestartLevel()
    {
        StopSpawningEnemies();
        ClearEnemiesAlive();
        ResetBossesToSpawn();
        _enemySpawnInterval = SPAWN_ENEMY_INTERVAL_BASE;
        ResetSpawnersModifiers();
    }

    private void ResetSpawnersModifiers()
    {
        foreach (EnemySpawner spawner in spawners)
        {
            spawner.SpeedModifier = 0;
        }
    }

    private void SetSpawnersModifiers(float value)
    {
        foreach (EnemySpawner spawner in spawners)
        {
            spawner.SpeedModifier += value;
        }
    }

    private void StartSpawningEnemies()
    {
        spawnEnemies = true;
        spawnEnemiesCoroutine = StartCoroutine(SpawnEnemies());
    }

    private void StopSpawningEnemies()
    {
        if (spawnEnemiesCoroutine != null)
        {
            StopCoroutine(spawnEnemiesCoroutine);
        }
        spawnEnemies = false;
    }

    private IEnumerator SpawnEnemies()
    {
        while (spawnEnemies)
        {
            yield return new WaitForSeconds(_enemySpawnInterval);
            SpawnFromRandomSpawners(GetRandomSpawners());
        }
    }

    private List<EnemySpawner> GetRandomSpawners()
    {
        _spawnersSpawning.Clear();
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            EnemySpawner currentspawner = spawners[Random.Range(0, spawners.Length - 1)];
            currentspawner.EnemiesToSpawn += 1;
            if (!_spawnersSpawning.Contains(currentspawner))
            {
                _spawnersSpawning.Add(currentspawner);
            }
        }
        return _spawnersSpawning;
    }

    private void SpawnFromRandomSpawners(List<EnemySpawner> spawners)
    {
        foreach (EnemySpawner spawner in spawners)
        {
            spawner.StartSpawnProcess();
        }
    }

    /// <summary>
    /// Add given object to given list
    /// </summary>
    /// <param name="listToAddTo"></param>
    /// <param name="enemy"></param>
    private void AddEnemy(List<GameObject> listToAddTo, GameObject enemy)
    {
        listToAddTo.Add(enemy);
    }

    /// <summary>
    /// remove given object from given list
    /// </summary>
    /// <param name="listToRemoveFrom"></param>
    /// <param name="enemy"></param>
    private void RemoveEnemy(List<GameObject> listToRemoveFrom, GameObject enemy)
    {
        if (listToRemoveFrom.Contains(enemy))
        {
            listToRemoveFrom.Remove(enemy);
        }

        //if enemies arent being spawned a boss fight is happening
        if (!spawnEnemies)
        {
            //if there are no enemies alive the boss fight is over
            if (enemiesAlive.Count == 0)
            {
                Debug.Log("Boss dead");
                //make enemies spawn faster
                _enemySpawnInterval -= SPAWN_ENEMY_REDUCTION;
                //make enemies move faster
                foreach (EnemySpawner spawner in spawners)
                {
                    SetSpawnersModifiers(0.1f);
                }
                //broadcast boss death
                OnBossDied?.Invoke();

                //resume spawning enemies
                StartSpawningEnemies();
            }
        }
    }

    private void ClearEnemiesAlive()
    {
        if (enemiesAlive.Count > 0)
        {
            for (int i = enemiesAlive.Count - 1; i >= 0; i--)
            {
                GameObject enemyToRemove = enemiesAlive[i];
                enemiesAlive.RemoveAt(i);

                Destroy(enemyToRemove);
            }
        }
        gunshipsAlive.Clear();
    }

    private void ResetBossesToSpawn()
    {
        foreach (GameObject boss in bossPrefabs)
        {
            if (!_bossesToSpawn.Contains(boss))
            {
                _bossesToSpawn.Add(boss);
            }
        }
    }

    /// <summary>
    /// Select a random gameobject from bosses left to spawn
    /// </summary>
    /// <returns>return boss selected</returns>
    private GameObject GetRandomBossToSpawn()
    {
        int index = Random.Range(0, _bossesToSpawn.Count - 1);
        GameObject boss = _bossesToSpawn[index];
        _bossesToSpawn.RemoveAt(index);

        if (_bossesToSpawn.Count == 0)
        {
            ResetBossesToSpawn();
        }
        return boss;
    }

    private void SpawnBoss()
    {
        StopSpawningEnemies();
        bossSpawner.StartBossSpawn(GetRandomBossToSpawn());
    }
}
