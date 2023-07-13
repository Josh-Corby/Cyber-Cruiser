using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CyberCruiser
{
    public class PickupManager : GameBehaviour
    {
        [SerializeField] private List<GameObject> pickupsOnScreen = new List<GameObject>();

        [SerializeField] private PickupSpawner _pickupSpawner;
        [SerializeField] private PickupSpawner _upgradeSpawner;

        [SerializeField] private List<GameObject> _bossDropsPool = new();
        [SerializeField] private List<GameObject> _bossDropsToSpawn = new();
        [SerializeField] private GameObject[] _weaponUpgradesPool;
        [SerializeField] private GameObject _normalPickup;
        [SerializeField] private GameObject _baseBossDrop;

        [SerializeField] private GameObject _pickupIndicator;

        [SerializeField] private float _indicatorAngle;
        protected readonly float _indicatorTimer = 2f;

        private void OnEnable()
        {
            GameManager.OnMissionStart += ResetBossDropsList;
            GameManager.OnMissionEnd += ClearPickups;
            Boss.OnBossDiedPosition += SpawnPickupAtPosition;
            Pickup.OnPickedUp += RemovePickup;
            PickupSpawner.OnPickupSpawned += AddPickup;         
        }

        private void OnDisable()
        {
            GameManager.OnMissionStart -= ResetBossDropsList;
            GameManager.OnMissionEnd -= ClearPickups;
            Boss.OnBossDiedPosition -= SpawnPickupAtPosition;
            Pickup.OnPickedUp -= RemovePickup;
            PickupSpawner.OnPickupSpawned -= AddPickup;
        }

        public void SpawnPlasmaDrop()
        {
            SpawnPickupAtRandomPosition(PickupType.Normal);
        }

        public void SpawnWeaponDrop()
        {
            SpawnPickupAtRandomPosition(PickupType.Weapon);
        }

        private void SpawnPickupAtRandomPosition(PickupType pickupType)
        {
            switch (pickupType)
            {
                case PickupType.Normal:
                    _pickupSpawner.SpawnPickupAtRandomPosition(_normalPickup);
                    break;
                case PickupType.Boss:
                    _pickupSpawner.SpawnPickupAtRandomPosition(GetRandomBossPickup());
                    break;
                case PickupType.Weapon:
                    _upgradeSpawner.SpawnPickupAtRandomPosition(GetRandomWeaponUpgrade());
                    break;
            }
        }

        private void SpawnPickupAtPosition(PickupType pickupType, Vector3 position)
        {
            switch (pickupType)
            {
                case PickupType.Normal:
                    _pickupSpawner.SpawnPickupAtPosition(_normalPickup, position);
                    break;
                case PickupType.Boss:
                    _pickupSpawner.SpawnPickupAtPosition(GetRandomBossPickup(), position);
                    break;
                case PickupType.Weapon:
                    _upgradeSpawner.SpawnPickupAtPosition(GetRandomWeaponUpgrade(), position);
                    break;
            }
        }
    

        private GameObject GetRandomWeaponUpgrade()
        {
            int randomIndex = Random.Range(0, _weaponUpgradesPool.Length);
            GameObject randomUpgradePrefab = _weaponUpgradesPool[randomIndex];
            return randomUpgradePrefab;
        }

        private GameObject GetRandomBossPickup()
        {        
            if( _bossDropsToSpawn.Count == 0)
            {
                return _baseBossDrop;
            }

            int RandomBossDropIndex = Random.Range(0, _bossDropsPool.Count);
            GameObject randomBossDrop = _bossDropsPool[RandomBossDropIndex];
            _bossDropsToSpawn.RemoveAt(RandomBossDropIndex);
            return randomBossDrop;
        }

        private void AddPickup(GameObject pickup)
        {
            pickupsOnScreen.Add(pickup);
        }

        private void RemovePickup(GameObject pickup)
        {
            pickupsOnScreen.Remove(pickup);
        }

        private void ClearPickups()
        {
            if (pickupsOnScreen.Count > 0)
            {
                for (int i = pickupsOnScreen.Count - 1; i >= 0; i--)
                {
                    GameObject pickup = pickupsOnScreen[i];
                    pickupsOnScreen.RemoveAt(i);

                    Destroy(pickup);
                }
            }
            pickupsOnScreen.Clear();
        }

        private void ResetBossDropsList()
        {
            _bossDropsToSpawn = _bossDropsPool;
        }

        // not currently used functions
        private IEnumerator SpawnWeaponUpgrade()
        {
            Vector2 position = _upgradeSpawner.GetRandomPosition();
            CreateIndicator(position, _upgradeSpawner);
            yield return new WaitForSeconds(_indicatorTimer);
            GameObject pickup = GetRandomWeaponUpgrade();
            _upgradeSpawner.SpawnPickupAtPosition(pickup, position);
        }

        private void CreateIndicator(Vector2 position, PickupSpawner spawner)
        {
            GameObject Indicator = Instantiate(_pickupIndicator, position, Quaternion.identity);

            //Indicator.transform.position += spawner._pickupIndicatorOffset;

            //if (spawner._pickupIndicatorOffset.x == 0)
            //{
            //    Indicator.transform.position += new Vector3(position.x, 0);
            //}

            //if (spawner._pickupIndicatorOffset.y == 0)
            //{
            //    Indicator.transform.position += new Vector3(0, position.y);
            //}

            Indicator.transform.rotation = Quaternion.Euler(0, 0, _indicatorAngle);
            Indicator.GetComponent<Indicator>().timer = _indicatorTimer;
        }
    }
}