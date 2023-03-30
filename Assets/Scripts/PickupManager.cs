using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PickupManager : GameBehaviour
{
    private List<GameObject> pickupsOnScreen = new List<GameObject>();
    private PickupSpawner _pickupSpawner;

    private void Awake()
    {
        _pickupSpawner = GetComponentInChildren<PickupSpawner>();
    }
    private void OnEnable()
    {
        GameManager.OnLevelCountDownStart += ClearPickups;
        GameManager.OnPlasmaDropDistanceRequested += GenerateNewPlasmaDropDistance;
        GameManager.OnWeaponUpgradeDropDistanceRequested += GenerateNewWeaponUpgradeDropDistance;
        PickupSpawner.OnPickupSpawned += AddPickup;
        Pickup.OnPickup += RemovePickup;
    }

    private void OnDisable()
    {
        GameManager.OnLevelCountDownStart -= ClearPickups;
        GameManager.OnPlasmaDropDistanceRequested -= GenerateNewPlasmaDropDistance;
        GameManager.OnWeaponUpgradeDropDistanceRequested -= GenerateNewWeaponUpgradeDropDistance;
        PickupSpawner.OnPickupSpawned -= AddPickup;
        Pickup.OnPickup -= RemovePickup;
    }

    public void GenerateNewPlasmaDropDistance(int currentDistanceMilestone, Action<int> callback)
    {
        int plasmaDropDistance = Random.Range(currentDistanceMilestone + 15, currentDistanceMilestone + 99);
        callback(plasmaDropDistance);
        Debug.Log("Plasma spawn distance is " + plasmaDropDistance);
    }

    private void GenerateNewWeaponUpgradeDropDistance(int previousBossDistance, int currentBossDistance, Action<int> callback)
    {
        int weaponUpgradeDropDistance = Random.Range(previousBossDistance + 15, currentBossDistance);
        callback(weaponUpgradeDropDistance);
        Debug.Log("Weapon upgrade drop distance is " + weaponUpgradeDropDistance);
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
}
