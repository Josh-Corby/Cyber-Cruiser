using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System.Linq;

public class EnemySpawnerManager : GameBehaviour<EnemySpawnerManager>
{
    public static event Action OnBossDied = null;

    [SerializeField] private int enemiesToSpawn;
    [SerializeField] private EnemySpawner[] spawners;
    [SerializeField] private List<float> spawnerWeights;
    [SerializeField] private float spawnEnemyInterval;
    [SerializeField] private float spawnEnemyReduction;
    [SerializeField] private float enemySpeedIncrement;
    [SerializeField] private EnemySpawner bossSpawner;
    [SerializeField] private GameObject[] bossPrefabs;
    private GameObject currentBoss;
    private int bossCounter;
    private Coroutine spawnEnemiesCoroutine;
    public List<GameObject> enemiesAlive = new();
    public List<GameObject> gunshipsAlive = new();
    public bool spawnEnemies;
    public GameObject bossGoalPosition;

    public Transform dragonMovePoint;

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
        spawnEnemyInterval = 1f;
        bossCounter = 0;
        currentBoss = bossPrefabs[bossCounter];
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

    private void AddEnemy(List<GameObject> listToAddTo, GameObject enemy)
    {
        listToAddTo.Add(enemy);
    }

    private void RemoveEnemy(List<GameObject> listToRemoveFrom, GameObject enemy)
    {
        listToRemoveFrom.Remove(enemy);

        //if enemies arent being spawned a boss fight is happening
        if (!spawnEnemies)
        {
            //if there are no enemies alive the boss fight is over
            if (enemiesAlive.Count == 0)
            {
                Debug.Log("Boss dead");
                //make enemies spawn faster
                spawnEnemyInterval -= spawnEnemyReduction;
                //make enemies move faster
                foreach (EnemySpawner spawner in spawners)
                {
                    spawner.IncrementSpeedModifier(enemySpeedIncrement);
                }


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
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            EnemySpawner currentspawner = spawners[Random.Range(0, spawners.Length - 1)];
            currentspawner.StartSpawnProcess();
        }
    }

    private void NormalizeWeights()
    {
        float sum = spawnerWeights.Sum();
        for (int i = 0; i < spawnerWeights.Count; i++)
        {
            spawnerWeights[i] /= sum;
        }
    }
    private EnemySpawner GetRandomWeightedSpawner()
    {
        float randomNumber = Random.value;
        float weightSum = 0;
        for (int i = 0; i < spawnerWeights.Count; i++)
        {
            weightSum += spawnerWeights[i];
            if (weightSum >= randomNumber)
            {
                return spawners[i];
            }
        }
        return spawners[spawners.Length - 1];
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
}
