using System.Collections.Generic;
using UnityEngine;

namespace CyberCruiser
{
    public class EnemyManager : GameBehaviour<EnemyManager>
    {
        public List<SlicerMovement> slicersSeeking = new();
        [HideInInspector] public List<GunshipMovement> gunshipsToProcess = new();
        [SerializeField] private List<GameObject> _enemiesAlive = new();
        [SerializeField] private List<GameObject> _crashingEnemies = new();

        private void OnEnable()
        {
            Enemy.OnEnemyAliveStateChange += IsEnemyAlive;
            Enemy.OnEnemyCrash += AddCrashingEnemy;
            SlicerMovement.OnStartSeeking += RecieveUnit;
            GunshipMovement.OnGunshipSpawned += RecieveUnit;
            GameManager.OnMissionEnd += ClearLists;
            EnemySpawnerManager.OnSpawnEnemyGroup += ClearMovementLists;
        }

        private void OnDisable()
        {
            Enemy.OnEnemyAliveStateChange -= IsEnemyAlive;
            Enemy.OnEnemyCrash -= AddCrashingEnemy;
            SlicerMovement.OnStartSeeking -= RecieveUnit;
            GunshipMovement.OnGunshipSpawned -= RecieveUnit;
            GameManager.OnMissionEnd -= ClearLists;
            EnemySpawnerManager.OnSpawnEnemyGroup -= ClearMovementLists;
        }

        private void RecieveUnit(GameObject unit)
        {
            if (unit.TryGetComponent<GunshipMovement>(out var gunship))
            {
                AddToList(gunshipsToProcess, gunship);
                return;
            }

            if (unit.TryGetComponent<SlicerMovement>(out var slicer))
            {
                AddToList(slicersSeeking, slicer);
                return;
            }
        }

        private void IsEnemyAlive(GameObject enemy, bool aliveState)
        {
            if (aliveState)
            {
                AddToList(_enemiesAlive, enemy);
            }
            if (!aliveState)
            {
                RemoveFromList(_enemiesAlive, enemy);
                //If an enemy was removed from enemies alive, and if the boss is ready to be spawned, check how many enemies are alive
                if (EnemySpawnerManagerInstance.bossReadyToSpawn)
                {
                    if (AreAllEnemiesDead())
                    {
                        EnemySpawnerManagerInstance.StartBossSpawn();
                    }
                }
            }
        }

        private void AddCrashingEnemy(GameObject enemy)
        {
            _crashingEnemies.Add(enemy);
        }

        private void ClearMovementLists()
        {
            slicersSeeking.Clear();
            gunshipsToProcess.Clear();
        }

        private void ClearLists()
        {
            ClearList(_enemiesAlive);
            ClearList(_crashingEnemies);
            ClearMovementLists();
        }

        public bool AreAllEnemiesDead()
        {
            return _enemiesAlive.Count == 0;
        }

        public GameObject CreateEnemyFromSO(EnemyScriptableObject enemyInfo)
        {
            GameObject enemy = enemyInfo.EnemyPrefab;
            enemy.GetComponent<Enemy>().EnemyInfo = enemyInfo;
            return enemy;
        }
    }
}