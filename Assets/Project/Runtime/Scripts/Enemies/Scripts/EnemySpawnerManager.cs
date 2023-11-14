using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CyberCruiser
{
    public class EnemySpawnerManager : GameBehaviour<EnemySpawnerManager>
    {
        [Header("Spawners")]
        public EnemySpawner _topSpawner;
        public EnemySpawner _bottomSpawner;
        [SerializeField] private EnemySpawner _bossSpawner;
        [SerializeField] private EnemyScriptableObject[] _bosses;

        [Header("Spawn Rate Info")]
        [SerializeField] private int _enemiesToSpawnBase;
        [SerializeField] private float _spawnEnemyIntervalBase;
        [SerializeField] private float _spawnEnemyReduction;
        [SerializeField] private float _offsetPerEnemy;
        [SerializeField] private int _timesToReduce;
        [SerializeField] private EnemySpawnerInfo[] _enemySpawnerInfo;
        [SerializeField] private float _totalSpawnWeight;

        private int _currentEnemiesToSpawn;
        private float _currentEnemySpawnInterval;
        private float _enemySpawnTimer;
        private int _timesReduced;
        private EnemyScriptableObject _currentBossToSpawn;

        [Header("Transform References")]
        public GameObject bossGoalPosition;

        [Header("Spawn bools")]
        [SerializeField] private BoolValue _isBossReadyToSpawn;
        [SerializeField] private BoolReference _areAllEnemiesDead;

        [HideInInspector] public bool bossReadyToSpawn;
        private const float BOSS_WAIT_TIME = 2f;
        private bool _spawnEnemies;

        private List<EnemySpawner> _enemySpawners = new();
        private List<EnemySpawner> _spawnersSpawning = new();
        private List<EnemyScriptableObject> _bossesToSpawn = new();

        private bool IsBossReadyToSpawn { get => _isBossReadyToSpawn.Value; set => _isBossReadyToSpawn.Value = value; }

        public static event Action OnSpawnEnemyGroup = null;
        public static event Action<EnemyScriptableObject> OnBossSelected = null;

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
            DistanceManager.OnBossDistanceReached += SelectBossToSpawn;
            BossUIManager.OnBossWarningComplete += SpawnBoss;
            Boss.OnBossDiedPosition += (p, v) => ProcessBossDied();
        }

        private void OnDisable()
        {
            DistanceManager.OnBossDistanceReached -= SelectBossToSpawn;
            BossUIManager.OnBossWarningComplete -= SpawnBoss;
            Boss.OnBossDiedPosition -= (p, v) => ProcessBossDied();
        }

        private void Start()
        {
            ResetSpawning();
        }

        private void Update()
        {
            while (_spawnEnemies)
            {
                if (IsWaitingForSpawnTimer())
                {
                    return;
                }

                _spawnersSpawning.Clear();
                OnSpawnEnemyGroup?.Invoke();

                for (int i = 0; i < _currentEnemiesToSpawn; i++)
                {
                    GetRandomWeightedSpawners();
                }

                SpawnFromRandomSpawners();
                _enemySpawnTimer = _currentEnemySpawnInterval;
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

        public void ResetSpawning()
        {
            StopSpawningEnemies();
            ResetBossesToSpawn();
            ResetSpawnersModifiers();
            _currentEnemiesToSpawn = _enemiesToSpawnBase;
            _currentEnemySpawnInterval = _spawnEnemyIntervalBase;
            _timesReduced = 0;
        }

        private void ResetSpawnersModifiers()
        {
            SetSpawnersModifiers(0);
        }

        private void SetSpawnersModifiers(float value)
        {
            foreach (EnemySpawner spawner in _enemySpawners)
            {
                spawner.SpeedModifier += value;
            }
        }

        public void StartSpawningEnemies()
        {
            _spawnEnemies = true;
            _enemySpawnTimer = _currentEnemySpawnInterval;
        }

        public void StopSpawningEnemies()
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
            for (int i = 0; i < _currentEnemiesToSpawn; i++)
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
            IsBossReadyToSpawn = true;

            if (_areAllEnemiesDead.Value == true)
            {
                SelectBossToSpawn();
            }
        }

        public void SelectBossToSpawn()
        {
            StopSpawningEnemies();
            IsBossReadyToSpawn = false;
            _currentBossToSpawn = GetRandomBossToSpawn();
            OnBossSelected?.Invoke(_currentBossToSpawn);
        }

        private void SpawnBoss()
        {
            //Debug.Log("spawning boss");
            _bossSpawner.SpawnBoss(_currentBossToSpawn);
        }

        private EnemyScriptableObject GetRandomBossToSpawn()
        {
            if (_bossesToSpawn.Count == 0)
            {
                ResetBossesToSpawn();
            }

            int index = Random.Range(0, _bossesToSpawn.Count - 1);
            EnemyScriptableObject boss = _bossesToSpawn[index];
            _bossesToSpawn.RemoveAt(index);

            return boss;
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

            //resume spawning enemies
            Invoke(nameof(StartSpawningEnemies), BOSS_WAIT_TIME);
        }

        private void ChangeSpawnInterval()
        {
            //Debug.Log("Spawn interval change");
            if (_timesReduced >= _timesToReduce)
            {
                _currentEnemiesToSpawn += 1;
                _currentEnemySpawnInterval = _spawnEnemyIntervalBase + (_currentEnemiesToSpawn * _offsetPerEnemy);
            }

            if (_timesReduced < _timesToReduce)
            {
                _currentEnemySpawnInterval -= _spawnEnemyReduction;
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
}