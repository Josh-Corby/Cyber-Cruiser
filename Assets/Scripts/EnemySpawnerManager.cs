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

    private bool _spawnEnemies;
    public GameObject bossGoalPosition;
    public Transform dragonMovePoint;
    [HideInInspector] public bool bossReadyToSpawn;

    [SerializeField] private int enemiesToSpawn;
    private float _enemySpawnInterval;

    private List<EnemySpawner> _enemySpawners = new();
    [SerializeField] private EnemySpawner bossSpawner;
    [SerializeField] private GameObject[] bossPrefabs;

    public EnemySpawner _topSpawner;
    public EnemySpawner _bottomSpawner;

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

    public static event Action<GameObject> OnBossSelected = null;

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
        DistanceManager.OnBossDistanceReached += SetupForBossSpawn;
        Boss.OnBossDied += (p,v) => { StartCoroutine(ProcessBossDied()); };
        GameManager.OnMissionStart += RestartLevel;
        WaveCountdownManager.OnCountdownDone += StartSpawningEnemies;
        PlayerManager.OnPlayerDeath += StopSpawningEnemies;
    }

    private void OnDisable()
    {
        DistanceManager.OnBossDistanceReached -= SetupForBossSpawn;
        Boss.OnBossDied -= (p,v) => { StartCoroutine(ProcessBossDied()); };
        GameManager.OnMissionStart -= RestartLevel;
        WaveCountdownManager.OnCountdownDone -= StartSpawningEnemies;
        PlayerManager.OnPlayerDeath -= StopSpawningEnemies;     
    }

    private void Start()
    {
        RestartLevel();
    }

    private void RestartLevel()
    {
        StopSpawningEnemies();
        CancelBossSpawn();
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
        _spawnEnemies = true;
        spawnEnemiesCoroutine = StartCoroutine(SpawnEnemies());
    }

    private void StopSpawningEnemies()
    {
        if (spawnEnemiesCoroutine != null)
        {
            StopCoroutine(spawnEnemiesCoroutine);
        }
        _spawnEnemies = false;
    }

    private IEnumerator SpawnEnemies()
    {
        while (_spawnEnemies)
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

        if (EM.AreAllEnemiesDead())
        {
            StartBossSpawn();
        }
    }

    public void StartBossSpawn()
    {
        if (spawnBossCoroutine != null)
        {
            StopCoroutine(spawnBossCoroutine);
        }
        spawnBossCoroutine = StartCoroutine(SpawnBoss());
    }

    private IEnumerator SpawnBoss()
    {
        bossReadyToSpawn = false;
        GameObject bossToSpawn = GetRandomBossToSpawn();
        OnBossSelected(bossToSpawn);
        yield return new WaitForSeconds(BOSS_WAIT_TIME);
        bossSpawner.StartBossSpawn(bossToSpawn);
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

    private void CancelBossSpawn()
    {
        if(spawnBossCoroutine != null)
        {
            StopCoroutine(spawnBossCoroutine);
        }
    }

    private IEnumerator ProcessBossDied()
    {
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

