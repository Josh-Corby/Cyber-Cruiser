using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class EnemySpawnerManager : GameBehaviour<EnemySpawnerManager>
{
    public static event Action OnBossDied = null;

    [SerializeField] private EnemySpawner[] spawners;
    [SerializeField] private int spawnEnemyInterval;

    [SerializeField] private EnemySpawner bossSpawner;
    [SerializeField] private GameObject[] bossPrefabs;
    private bool bossSpawned;

    private GameObject currentBoss;
    private int bossCounter;

    private Coroutine spawnEnemiesCoroutine;

    public List<GameObject> enemiesAlive = new List<GameObject>();

    public bool spawnEnemies;

    private void OnEnable()
    {
        GameManager.OnLevelCountDownStart += RestartLevel;
        GameplayUIManager.OnCountdownDone += StartSpawningEnemies;
        PlayerManager.OnPlayerDeath += StopSpawningEnemies;

        EnemySpawner.OnEnemySpawned += AddEnemy;

        DistanceCounter.OnBossDistanceReached += SpawnBoss;

        Enemy.OnEnemyDied += RemoveEnemy;
    }

    private void OnDisable()
    {
        GameManager.OnLevelCountDownStart -= RestartLevel;
        GameplayUIManager.OnCountdownDone -= StartSpawningEnemies;
        PlayerManager.OnPlayerDeath -= StopSpawningEnemies;

        EnemySpawner.OnEnemySpawned -= AddEnemy;

        DistanceCounter.OnBossDistanceReached -= SpawnBoss;

        Enemy.OnEnemyDied -= RemoveEnemy;
    }

    private void Start()
    {
        bossCounter = 0;
        currentBoss = bossPrefabs[bossCounter];
        bossSpawned = false;
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
    }

    private void SpawnBoss()
    {
        StopSpawningEnemies();
        bossSpawner.StartBossSpawn(currentBoss);
        //bossCounter += 1;
        //currentBoss = bossPrefabs[bossCounter];

        spawnEnemies = false;
    }

    private void AddEnemy(GameObject enemy)
    {
        enemiesAlive.Add(enemy);
    }

    private void RemoveEnemy(GameObject enemy)
    {
        enemiesAlive.Remove(enemy);

        //if enemies arent being spawned a boss fight is happening
        if (!spawnEnemies)
        {
            //if there are no enemies alive the boss fight is over
            if(enemiesAlive.Count == 0)
            {
                Debug.Log("Boss dead");
                //broadcast boss death
                OnBossDied?.Invoke();
                //resume spawning enemies
                StartSpawningEnemies();
            }
        }
    }

    private void RestartLevel()
    {
        StopSpawningEnemies();
        ClearEnemiesAlive();
    }

    private IEnumerator SpawnEnemies()
    {
        while (spawnEnemies)
        {
            yield return new WaitForSeconds(spawnEnemyInterval);
            ChooseRandomSpawner();
        }
    }

    private void ChooseRandomSpawner()
    {
        EnemySpawner currentspawner = spawners[Random.Range(0, spawners.Length - 1)];
        currentspawner.StartSpawnProcess();
    }

    private void ClearEnemiesAlive()
    {
        for (int i = 0; i < enemiesAlive.Count; i++)
        {
            GameObject enemy = enemiesAlive[i];
            enemiesAlive.Remove(enemy);
            Destroy(enemy);
        }
    }
}
