using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawnerManager : GameBehaviour<EnemySpawnerManager>
{
    [Header("Spawners")]
    public EnemySpawner _topSpawner;
    public EnemySpawner _bottomSpawner;
    [SerializeField] private EnemySpawner _bossSpawner;
    [SerializeField] private EnemyScriptableObject[] _bosses;

    [Header("Spawn Rate Info")]
    [SerializeField] private int _enemiesToSpawnBase;
    [SerializeField] private int _enemiesToSpawn;
    [SerializeField] private float _spawnEnemyIntervalBase;
    [SerializeField] private float _enemySpawnInterval;
    private float _enemySpawnTimer;
    [SerializeField] private float _spawnEnemyReduction;
    [SerializeField] private float _offsetPerEnemy;
    [SerializeField] private int _timesToReduce;
    [SerializeField] private int _timesReduced;
    [SerializeField] private EnemySpawnerInfo[] _enemySpawnerInfo;
    [SerializeField] private float _totalSpawnWeight;

    [Header("Transform References")]
    public GameObject bossGoalPosition;
    public Transform dragonMovePoint;

    [Header("Spawn bools")]
    [HideInInspector] public bool bossReadyToSpawn;
    private const float BOSS_WAIT_TIME = 2f;
    private bool _spawnEnemies;

    [Header("Lists")]
    private List<EnemySpawner> _enemySpawners = new();
    private List<EnemySpawner> _spawnersSpawning = new();
    private List<EnemyScriptableObject> _bossesToSpawn = new();

    [Header("Coroutines")]
    private Coroutine _spawnEnemiesCoroutine;
    private Coroutine _spawnBossCoroutine;

    #region Actions
    public static event Action OnSpawnEnemyGroup = null;
    public static event Action<EnemyScriptableObject> OnBossSelected = null;
    #endregion

    protected override void Awake()
    {
        base.Awake();
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
        Boss.OnBossDied += (p, v) => { ProcessBossDied(); };
        GameManager.OnMissionStart += RestartLevel;
        WaveCountdownManager.OnCountdownDone += StartSpawningEnemies;
        PlayerManager.OnPlayerDeath += StopSpawningEnemies;
    }

    private void OnDisable()
    {
        DistanceManager.OnBossDistanceReached -= SetupForBossSpawn;
        Boss.OnBossDied -= (p, v) => { ProcessBossDied(); };
        GameManager.OnMissionStart -= RestartLevel;
        WaveCountdownManager.OnCountdownDone -= StartSpawningEnemies;
        PlayerManager.OnPlayerDeath -= StopSpawningEnemies;
    }

    private void Start()
    {
        RestartLevel();
    }

    private void Update()
    {
        while (_spawnEnemies)
        {
            if (IsWaitingForSpawnTimer() == true)
            {
                return;
            }

            _spawnersSpawning.Clear();
            OnSpawnEnemyGroup?.Invoke();
            for (int i = 0; i < _enemiesToSpawn; i++)
            {
                GetRandomWeightedSpawners();
            }
            SpawnFromRandomSpawners();
            _enemySpawnTimer = _enemySpawnInterval;
        }
    }

    private bool IsWaitingForSpawnTimer()
    {
        if (_enemySpawnTimer > 0)
        {
            _enemySpawnTimer -= Time.deltaTime;
            return true;
        }
        else return false;
    }

    private void RestartLevel()
    {
        StopSpawningEnemies();
        CancelBossSpawn();
        ResetBossesToSpawn();
        _enemiesToSpawn = _enemiesToSpawnBase;
        _enemySpawnInterval = _spawnEnemyIntervalBase;
        ResetSpawnersModifiers();
        _timesReduced = 0;
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
        _enemySpawnTimer = _enemySpawnInterval;
    }

    private void StopSpawningEnemies()
    {
        _spawnEnemies = false;
        _enemySpawnTimer = 0;
    }

    private void GetRandomWeightedSpawners()
    {
        float value = Random.value;
        for (int i = 0; i < _enemySpawnerInfo.Length; i++)
        {
            if (value < _enemySpawnerInfo[i].SpawnerWeight)
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
        for (int i = 0; i < _enemiesToSpawn; i++)
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
        foreach (EnemyScriptableObject boss in _bosses)
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
        if (_spawnBossCoroutine != null)
        {
            StopCoroutine(_spawnBossCoroutine);
        }
        _spawnBossCoroutine = StartCoroutine(SpawnBoss());
    }

    private IEnumerator SpawnBoss()
    {
        bossReadyToSpawn = false;
        EnemyScriptableObject bossToSpawn = GetRandomBossToSpawn();
        OnBossSelected(bossToSpawn);
        yield return new WaitForSeconds(BOSS_WAIT_TIME);
        _bossSpawner.SpawnBoss(bossToSpawn);
    }

    /// <summary>
    /// Select a random gameobject from bosses left to spawn
    /// </summary>
    /// <returns>return boss selected</returns>
    private EnemyScriptableObject GetRandomBossToSpawn()
    {
        int index = Random.Range(0, _bossesToSpawn.Count - 1);
        EnemyScriptableObject boss = _bossesToSpawn[index];
        _bossesToSpawn.RemoveAt(index);

        if (_bossesToSpawn.Count == 0)
        {
            ResetBossesToSpawn();
        }
        return boss;
    }

    private void CancelBossSpawn()
    {
        if (_spawnBossCoroutine != null)
        {
            StopCoroutine(_spawnBossCoroutine);
        }
    }

    private void ProcessBossDied()
    {
        //Increment difficulty
        ChangeSpawnInterval();

        //make enemies move faster
        foreach (EnemySpawner spawner in _enemySpawners)
        {
            SetSpawnersModifiers(0.1f);
        }

        Invoke(nameof(StartSpawningEnemies), BOSS_WAIT_TIME);
        //resume spawning enemies
        StartSpawningEnemies();
    }

    private void ChangeSpawnInterval()
    {
        Debug.Log("Spawn interval change");
        if (_timesReduced >= _timesToReduce)
        {
            _enemiesToSpawn += 1;
            _enemySpawnInterval = _spawnEnemyIntervalBase + (_enemiesToSpawn * _offsetPerEnemy);
        }

        if (_timesReduced < _timesToReduce)
        {
            _enemySpawnInterval -= _spawnEnemyReduction;
            _timesReduced += 1;
        }
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

