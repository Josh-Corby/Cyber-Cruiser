using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class PickupSpawner : GameBehaviour
{
    public static event Action<GameObject> OnPickupSpawned = null;

    [SerializeField] private GameObject _plasmaPickup;
    [SerializeField] private GameObject _healthPickup;
    [SerializeField] private GameObject[] _weaponUpgradePrefabs;
    [SerializeField] private Vector3 spawnSize;
    

    private void OnEnable()
    {
        DistanceManager.OnPlasmaDistanceReached += SpawnPickupAtRandomPosition;
        DistanceManager.OnWeaponUpgradeDistanceReached += SpawnWeaponUpgrade;
        Boss.OnBossDied += SpawnHealthPickupAtPosition;
    }

    private void OnDisable()
    {
        DistanceManager.OnPlasmaDistanceReached -= SpawnPickupAtRandomPosition;
        DistanceManager.OnWeaponUpgradeDistanceReached -= SpawnWeaponUpgrade;
        Boss.OnBossDied -= SpawnHealthPickupAtPosition;
    }
    private Vector3 GetRandomPosition()
    {
        float x = Random.Range(-spawnSize.x / 2, spawnSize.x / 2);
        float y = Random.Range(-spawnSize.y / 2, spawnSize.y / 2);
        float z = 0;
        Vector3 spawnPosition = new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z + z);
        return spawnPosition;
    }

    private void SpawnPickupAtRandomPosition(PickupType pickupType)
    {
        GameObject pickup = null;
        switch (pickupType)
        {
            case PickupType.Plasma:
                pickup = _plasmaPickup;
                break;
            case PickupType.Health:
                pickup = _healthPickup;
                break;
            case PickupType.Weapon:
                pickup = GetRandomWeaponUpgrade();
                break;
        }
        SpawnPickupAtRandomPosition(pickup);
    }

    private void SpawnPickupAtPosition(PickupType pickupType, Vector3 position)
    {
        GameObject pickup = null;
        switch (pickupType)
        {
            case PickupType.Plasma:
                pickup = _plasmaPickup;
                break;
            case PickupType.Health:
                pickup = _healthPickup;
                break;
            case PickupType.Weapon:
                pickup = GetRandomWeaponUpgrade();
                break;
        }
        SpawnPickupAtPosition(pickup, position);
        OnPickupSpawned(pickup);
    }

    private void SpawnHealthPickupAtPosition(Vector3 position)
    {
        GameObject healthPickup = Instantiate(_healthPickup, position, transform.rotation);
    }
    private void SpawnPickupAtPosition(GameObject pickup, Vector3 position)
    {
        GameObject _pickup = Instantiate(pickup, position, transform.rotation);
        OnPickupSpawned(_pickup);
    }

    private void SpawnPickupAtRandomPosition(GameObject pickup)
    {
        GameObject _pickup = Instantiate(pickup, GetRandomPosition(), transform.rotation);
        OnPickupSpawned(_pickup);
    }

    public void SpawnWeaponUpgrade()
    {
        GameObject weaponUpgrade = Instantiate(GetRandomWeaponUpgrade(), GetRandomPosition(), transform.rotation);
        OnPickupSpawned(weaponUpgrade);
    }

    private GameObject GetRandomWeaponUpgrade()
    {
        int randomIndex = Random.Range(0, _weaponUpgradePrefabs.Length);
        GameObject randomUpgradePrefab = _weaponUpgradePrefabs[randomIndex];
        return randomUpgradePrefab;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, spawnSize);
    }
}
