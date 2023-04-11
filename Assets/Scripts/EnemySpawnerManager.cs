using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;


public class EnemySpawnerManager : GameBehaviour<EnemySpawnerManager>
{
    private const float SPAWN_ENEMY_INTERVAL_BASE = 2f;
    private const float SPAWN_ENEMY_REDUCTION = 0.2f;
    private const float BOSS_WAIT_TIME = 2f;

    public bool spawnEnemies;
    public GameObject bossGoalPosition;
    public Transform dragonMovePoint;
    private bool bossReadyToSpawn;

    [SerializeField] private int enemiesToSpawn;
    [SerializeField] private float _enemySpawnInterval;

    private List<EnemySpawner> _enemySpawners = new();
    [SerializeField] private EnemySpawner bossSpawner;
    [SerializeField] private GameObject[] bossPrefabs;

    public EnemySpawner _topTentacleSpawner;
    public EnemySpawner _bottomTentacleSpawner;

    public List<GameObject> enemiesAlive = new();
    [HideInInspector] public List<GameObject> gunshipsAlive = new();
    private List<GameObject> _bossesToSpawn = new();
    private List<EnemySpawner> _spawnersSpawning = new();

    private Coroutine spawnEnemiesCoroutine;
    private Coroutine spawnBossCoroutine;

    [SerializeField] private EnemySpawnerInfo[] _enemySpawnerInfo;
    [SerializeField] private float _totalSpawnWeight;


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

    private void Awake()
    {
        InitializeEnemySpawners();
    }

    private void InitializeEnemySpawners()
    {
        foreach (EnemySpawnerInfo spawner in _enemySpawnerInfo)
        {
            _enemySpawners.Add(spawner.spawner);
        }
    }

    private void OnEnable()
    {
        Boss.OnBossDied += (p,v) => { StartCoroutine(ProcessBossDied()); };
        GameManager.OnLevelCountDownStart += RestartLevel;
        GameManager.OnLevelCountDownStart += CancelBossSpawn;

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
        Boss.OnBossDied -= (p,v) => { StartCoroutine(ProcessBossDied()); };
        GameManager.OnLevelCountDownStart -= RestartLevel;
        GameManager.OnLevelCountDownStart -= CancelBossSpawn;

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
        foreach (EnemySpawner spawner in _enemySpawners)
        {
            spawner.SpeedModifier = 0;
        }
    }

    private void SetSpawnersModifiers(float value)
    {
        foreach (EnemySpawner spawner in _enemySpawners)
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
            _spawnersSpawning.Clear();
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                GetRandomWeightedSpawners();
            }
            SpawnFromRandomSpawners();
        }
    }

    private void GetRandomWeightedSpawners()
    {
        float value = Random.value;
        for (int i = 0; i < _enemySpawnerInfo.Length; i++)
        {
            if(value < _enemySpawnerInfo[i].SpawnerWeight)
            {
                if (!_spawnersSpawning.Contains(_enemySpawnerInfo[i].spawner))
                {
                    _spawnersSpawning.Add(_enemySpawnerInfo[i].spawner);
                }
                _enemySpawnerInfo[i].spawner.EnemiesToSpawn += 1;
                return;
            }
            value -= _enemySpawnerInfo[i].SpawnerWeight;
        }
    }

    private List<EnemySpawner> GetRandomSpawners()
    {
        _spawnersSpawning.Clear();
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            EnemySpawner currentspawner = _enemySpawners[Random.Range(0, _enemySpawners.Count - 1)];
            currentspawner.EnemiesToSpawn += 1;
            if (!_spawnersSpawning.Contains(currentspawner))
            {
                _spawnersSpawning.Add(currentspawner);
            }
        }
        return _spawnersSpawning;
    }

    private void SpawnFromRandomSpawners()
    {
        
        foreach (EnemySpawner spawner in _spawnersSpawning)
        {
            //Debug.Log("tell spawner to spawn");
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
        if (bossReadyToSpawn)
        {
            CheckEnemiesAliveForBossSpawn();
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

    public void SetupForBossSpawn()
    {
        StopSpawningEnemies();
        bossReadyToSpawn = true;
        CheckEnemiesAliveForBossSpawn();
    }

    private void CheckEnemiesAliveForBossSpawn()
    {
        if (enemiesAlive.Count == 0)
        {
            if (spawnBossCoroutine != null)
            {
                StopCoroutine(spawnBossCoroutine);
            }
            Debug.Log("Spawning boss");
            spawnBossCoroutine = StartCoroutine(SpawnBoss());
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

    private IEnumerator SpawnBoss()
    {
        bossReadyToSpawn = false;
        GameObject bossToSpawn = GetRandomBossToSpawn();
        GUIM.EnableBossWarningUI(bossToSpawn);
        yield return new WaitForSeconds(BOSS_WAIT_TIME);
        bossSpawner.StartBossSpawn(bossToSpawn);
    }

    private void CancelBossSpawn()
    {
        if(spawnBossCoroutine != null)
        {
            StopCoroutine(spawnBossCoroutine);
        }
    }

    private IEnumerator ProcessBossDied()
    {

        Debug.Log("Boss dead");
        //make enemies spawn faster
        _enemySpawnInterval -= SPAWN_ENEMY_REDUCTION;
        //make enemies move faster
        foreach (EnemySpawner spawner in _enemySpawners)
        {
            SetSpawnersModifiers(0.1f);
        }

        //wait set time before spawning enemies again
        yield return new WaitForSeconds(BOSS_WAIT_TIME);
        //resume spawning enemies
        StartSpawningEnemies();
    }

    public void OnInspectorUpdate()
    {
        ValidateWeights();
        ApplyWeightsToSpawners();
    }

    private void ValidateWeights()
    {
        _totalSpawnWeight = ValidateSpawnRates(_enemySpawnerInfo, typeof(EnemySpawnerInfo), "SpawnerWeight", _totalSpawnWeight);
    }

    private void ApplyWeightsToSpawners()
    {
        for (int i = 0; i < _enemySpawnerInfo.Length; i++)
        {
            _enemySpawnerInfo[i].spawner.spawnerWeight = _enemySpawnerInfo[i].SpawnerWeight;
        }
    }

    public void ResetWeights()
    {
        for (int i = 0; i < _enemySpawnerInfo.Length; i++)
        {
            _enemySpawnerInfo[i].SpawnerWeight = 0;
            _enemySpawnerInfo[i].spawner.spawnerWeight = _enemySpawnerInfo[i].SpawnerWeight;
        }
    }
}

