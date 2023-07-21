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

        [SerializeField] private BoolValue _areAllEnemiesDead;
        [SerializeField] private BoolReference _isBossReadyToSpawn;

        private bool AreAllEnemiesDead { get => _areAllEnemiesDead.Value; set => _areAllEnemiesDead.Value = value; }

        private void OnEnable()
        {
            AnimatedPanelController.OnGameplayPanelClosed += ClearLists;
            GameManager.OnMissionStart += () => { AreAllEnemiesDead = true; };
            Enemy.OnEnemySpawned += AddEnemy;
            Enemy.OnEnemyDied += (enemyObject, enemyType) => RemoveEnemy(enemyObject);
            SlicerMovement.OnStartSeeking += RecieveUnit;
            GunshipMovement.OnGunshipSpawned += RecieveUnit;
        }

        private void OnDisable()
        {
            AnimatedPanelController.OnGameplayPanelClosed -= ClearLists;
            GameManager.OnMissionStart -= () => { AreAllEnemiesDead = true; };
            Enemy.OnEnemySpawned -= AddEnemy;
            Enemy.OnEnemyDied -= (enemyObject, enemyType) => RemoveEnemy(enemyObject);
            SlicerMovement.OnStartSeeking -= RecieveUnit;
            GunshipMovement.OnGunshipSpawned -= RecieveUnit;
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

            if(AreAllEnemiesDead)
            {
                AreAllEnemiesDead = false;
            }
        }

        private void RemoveEnemy(GameObject enemy)
        {
            RemoveFromList(_enemiesAlive, enemy);

            if(_enemiesAlive.Count == 0)
            {
                AreAllEnemiesDead = true;
            }
            
            if (_isBossReadyToSpawn.Value)
            {
                if (AreAllEnemiesDead)
                {
                    _enemySpawnerManager.SelectBossToSpawn();
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

        public GameObject CreateEnemyFromSO(EnemyScriptableObject enemyInfo)
        {
            GameObject enemy = enemyInfo.EnemyPrefab;
            enemy.GetComponent<Enemy>().EnemyInfo = enemyInfo;
            return enemy;
        }
    }
}