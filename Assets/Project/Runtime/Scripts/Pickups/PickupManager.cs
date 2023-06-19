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

        [SerializeField] private GameObject _plasmaPickup;
        [SerializeField] protected GameObject _healthPickup;
        [SerializeField] private GameObject[] _weaponUpgradePrefabs;
        [SerializeField] private GameObject _pickupIndicator;

        [SerializeField] private float _indicatorAngle;
        protected readonly float _indicatorTimer = 2f;

        public static event Action OnPlasmaSpawned = null;
        public static event Action OnWeaponUpgradeSpawned = null;

        protected void OnEnable()
        {
            GameManager.OnMissionEnd += ClearPickups;
            DistanceManager.OnPlasmaDistanceReached += SpawnPickupAtRandomPosition;
            DistanceManager.OnWeaponUpgradeDistanceReached += () => StartCoroutine(SpawnWeaponUpgrade());
            Boss.OnBossDiedPosition += SpawnPickupAtPosition;
            Pickup.OnPickup += RemovePickup;
        }

        protected void OnDisable()
        {
            GameManager.OnMissionEnd -= ClearPickups;
            DistanceManager.OnPlasmaDistanceReached -= SpawnPickupAtRandomPosition;
            DistanceManager.OnWeaponUpgradeDistanceReached -= () => StartCoroutine(SpawnWeaponUpgrade());
            Boss.OnBossDiedPosition -= SpawnPickupAtPosition;
            Pickup.OnPickup -= RemovePickup;
        }

        protected void SpawnPickupAtRandomPosition(PickupType pickupType)
        {
            switch (pickupType)
            {
                case PickupType.Plasma:
                    _pickupSpawner.SpawnPickupAtRandomPosition(_plasmaPickup);
                    OnPlasmaSpawned?.Invoke();
                    break;
                case PickupType.Health:
                    _pickupSpawner.SpawnPickupAtRandomPosition(_healthPickup);
                    break;
                case PickupType.Weapon:
                    _upgradeSpawner.SpawnPickupAtRandomPosition(GetRandomWeaponUpgrade());
                    OnWeaponUpgradeSpawned?.Invoke();
                    break;
            }
        }

        private void SpawnPickupAtPosition(PickupType pickupType, Vector3 position)
        {
            switch (pickupType)
            {
                case PickupType.Plasma:
                    _pickupSpawner.SpawnPickupAtPosition(_plasmaPickup, position);
                    break;
                case PickupType.Health:
                    _pickupSpawner.SpawnPickupAtPosition(_healthPickup, position);
                    break;
                case PickupType.Weapon:
                    _upgradeSpawner.SpawnPickupAtPosition(GetRandomWeaponUpgrade(), position);
                    break;
            }
        }

        protected GameObject GetRandomWeaponUpgrade()
        {
            int randomIndex = Random.Range(0, _weaponUpgradePrefabs.Length);
            GameObject randomUpgradePrefab = _weaponUpgradePrefabs[randomIndex];
            return randomUpgradePrefab;
        }

        private IEnumerator SpawnWeaponUpgrade()
        {
            Vector2 position = _upgradeSpawner.GetRandomPosition();
            CreateIndicator(position, _upgradeSpawner);
            yield return new WaitForSeconds(_indicatorTimer);
            GameObject pickup = GetRandomWeaponUpgrade();
            _upgradeSpawner.SpawnPickupAtPosition(pickup, position);
        }

        public void AddPickup(GameObject pickup)
        {
            pickupsOnScreen.Add(pickup);
        }

        public void RemovePickup(GameObject pickup)
        {
            pickupsOnScreen.Remove(pickup);
        }

        protected void ClearPickups()
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

        protected void CreateIndicator(Vector2 position, PickupSpawner spawner)
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