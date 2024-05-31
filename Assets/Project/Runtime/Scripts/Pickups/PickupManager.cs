using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CyberCruiser
{
    public class PickupManager : GameBehaviour
    {
        [SerializeField] private bool DEBUG_forcePickup;
        [SerializeField] private GameObject DEBUG_pickupToSpawn;

        [SerializeField] private List<GameObject> pickupsOnScreen = new List<GameObject>();
        [SerializeField] private PickupSpawner _pickupSpawner;
        [SerializeField] private PickupSpawner _upgradeSpawner;

        // Boss Drops
        [SerializeField] private List<GameObject> _initialBossDrops = new();
        [SerializeField] private List<GameObject> _unlockedBossDrops = new();
        [SerializeField] private List<GameObject> _bossDropsToSpawn = new();

        // Weapon Drops
        [SerializeField] private List<GameObject> _initialWeaponUpgrades;
        [SerializeField] private List<GameObject> _weaponDropsToSpawn;


        [SerializeField] private GameObject _normalPickup;
        [SerializeField] private GameObject _baseBossDrop;
        [SerializeField] private GameObject _pickupIndicator;
        [SerializeField] private float _indicatorAngle;

        protected readonly float _indicatorTimer = 2f;

        [SerializeField] private bool _spawnPowerupOnMissionStart = false;
        [SerializeField] private Transform _spawnPowerUpOnMissionStartTransform;
        [SerializeField] private Unlock[] _unlocks;

        private void OnEnable()
        {
            ResetUnlocks();

            DistanceManager.OnPickupDistanceReached += SpawnPlasmaDrop;
            DistanceManager.OnWeaponPackDistanceReached += SpawnWeaponDrop;
            GameManager.OnMissionStart += ResetBossDropsList;
            GameManager.OnMissionEnd += ClearPickups;
            Boss.OnBossDiedPosition += SpawnPickupAtPosition;
            Pickup.OnPickedUp += RemovePickup;
            PickupSpawner.OnPickupSpawned += AddPickup;
            PlayerWeaponController.OnEmergencyArsenalActivated += SpawnWeaponDrop;
            PlayerShieldController.OnSignalBeaconActivated += SpawnWeaponDrop;
            PlayerRankManager.OnRankUp += UnlockNewUpgrade;
            PlayerRankManager.OnRankLoaded += RestoreUnlocks;
            SaveManager.OnClearSaveData += ResetUnlocks;
        }

        private void OnDisable()
        {
            DistanceManager.OnPickupDistanceReached -= SpawnPlasmaDrop;
            DistanceManager.OnWeaponPackDistanceReached -= SpawnWeaponDrop;
            GameManager.OnMissionStart -= ResetBossDropsList;
            GameManager.OnMissionEnd -= ClearPickups;
            Boss.OnBossDiedPosition -= SpawnPickupAtPosition;
            Pickup.OnPickedUp -= RemovePickup;
            PickupSpawner.OnPickupSpawned -= AddPickup;
            PlayerWeaponController.OnEmergencyArsenalActivated -= SpawnWeaponDrop;
            PlayerShieldController.OnSignalBeaconActivated -= SpawnWeaponDrop;
            PlayerRankManager.OnRankUp -= UnlockNewUpgrade;
            PlayerRankManager.OnRankLoaded -= RestoreUnlocks;
            SaveManager.OnClearSaveData -= ResetUnlocks;
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
            int randomIndex = Random.Range(0, _weaponDropsToSpawn.Count);
            GameObject randomUpgradePrefab = _weaponDropsToSpawn[randomIndex];
            return randomUpgradePrefab;
        }

        private GameObject GetRandomBossPickup()
        {        
            if( _bossDropsToSpawn.Count == 0)
            {
                return _baseBossDrop;
            }

            if(DEBUG_forcePickup)
            {
                GameObject bossDrop = DEBUG_pickupToSpawn;
                return bossDrop;
            }


            int RandomBossDropIndex = Random.Range(0, _unlockedBossDrops.Count);
            GameObject randomBossDrop = _unlockedBossDrops[RandomBossDropIndex];
            _bossDropsToSpawn.RemoveAt(RandomBossDropIndex);
            return randomBossDrop;
        }

        private void AddPickup(GameObject pickup)
        {
            pickupsOnScreen.Add(pickup);
        }

        private void RemovePickup(Pickup pickup)
        {
            pickupsOnScreen.Remove(pickup.gameObject);
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
            _bossDropsToSpawn = new(_unlockedBossDrops);


            if(_spawnPowerupOnMissionStart)
            {
                SpawnPickupAtPosition(PickupType.Boss, _pickupSpawner.transform.position);
            }
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

        private void RestoreUnlocks(int currentRankID)
        {
            if (currentRankID <= 0) return;

            for (int i = 1; i <= currentRankID; i++)
            {
                UnlockNewUpgrade(i);
            }
        }

        private void UnlockNewUpgrade(int newRankID)
        {
            if (_unlocks.Length - 1 < newRankID)
            {
                Debug.Log("There is no unlock for rank " + (newRankID+1).ToString());
                return;
            }
            
            Itemtype unlockType = _unlocks[newRankID].ItemType;
            GameObject unlockedItem = _unlocks[newRankID].UnlockedItem;
            if(unlockType == Itemtype.BossDrop)
            {
                if(!_unlockedBossDrops.Contains(unlockedItem))
                {
                    _unlockedBossDrops.Add(unlockedItem);
                }
            }
            if(unlockType == Itemtype.Weapon)
            {
                if(!_weaponDropsToSpawn.Contains(unlockedItem))
                {
                    _weaponDropsToSpawn.Add(unlockedItem);
                }
            }
        }

        private void ResetUnlocks()
        {
            _unlockedBossDrops = new(_initialBossDrops);
            _weaponDropsToSpawn = new(_initialWeaponUpgrades);
        }
    }


    [Serializable]
    public struct Unlock
    {
        public GameObject UnlockedItem;
        public Itemtype ItemType;
    }

    [Serializable]
    public enum Itemtype
    {
        BossDrop,
        Weapon
    }
}