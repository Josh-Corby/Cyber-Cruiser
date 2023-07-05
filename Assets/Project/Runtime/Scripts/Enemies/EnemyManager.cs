using System.Collections.Generic;
using UnityEngine;

namespace CyberCruiser
{
    public class EnemyManager : GameBehaviour<EnemyManager>
    {
        [SerializeField] private EnemySpawnerManager _enemySpawnerManager;
        public List<SlicerMovement> slicersSeeking = new();
        public List<GunshipMovement> GunshipsAlive = new();
        [SerializeField] private List<GameObject> _enemiesAlive = new();

        private void OnEnable()
        {
            Enemy.OnEnemySpawned += AddEnemy;
            Enemy.OnEnemyDied += (enemyObject, enemyType) => RemoveEnemy(enemyObject);
            SlicerMovement.OnStartSeeking += RecieveUnit;
            GunshipMovement.OnGunshipSpawned += RecieveUnit;
            AnimatedPanelController.OnGameplayPanelClosed += ClearLists;
        }

        private void OnDisable()
        {
            Enemy.OnEnemySpawned -= AddEnemy;
            Enemy.OnEnemyDied -= (enemyObject, enemyType) => RemoveEnemy(enemyObject);
            SlicerMovement.OnStartSeeking -= RecieveUnit;
            GunshipMovement.OnGunshipSpawned -= RecieveUnit;
            AnimatedPanelController.OnGameplayPanelClosed -= ClearLists;
        }

        private void RecieveUnit(GameObject unit)
        {
            if (unit.TryGetComponent<GunshipMovement>(out var gunship))
            {
                AddToList(GunshipsAlive, gunship);
                return;
            }

            if (unit.TryGetComponent<SlicerMovement>(out var slicer))
            {
                AddToList(slicersSeeking, slicer);
                return;
            }
        }

        private void AddEnemy(GameObject enemy)
        {
            _enemiesAlive.Add(enemy);
        }

        private void RemoveEnemy(GameObject enemy)
        {
            RemoveFromList(_enemiesAlive, enemy);

            if (_enemySpawnerManager.bossReadyToSpawn)
            {
                if (AreAllEnemiesDead())
                {
                    _enemySpawnerManager.StartBossSpawn();
                }
            }
        }

        private void ClearMovementLists()
        {
            slicersSeeking.Clear();
            GunshipsAlive.Clear();
        }

        private void ClearLists()
        {
            ClearListAndDestroyObjects(_enemiesAlive);
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