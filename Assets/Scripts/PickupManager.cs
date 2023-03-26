using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : GameBehaviour
{
    private List<GameObject> pickupsOnScreen = new List<GameObject>();

    private void OnEnable()
    {
        GameManager.OnLevelCountDownStart += ClearPickups;
        PickupSpawner.OnPlasmaSpawned += AddPickup;
        Pickup.OnPlasmaPickup += RemovePickup;
    }

    private void OnDisable()
    {
        GameManager.OnLevelCountDownStart -= ClearPickups;
        PickupSpawner.OnPlasmaSpawned -= AddPickup;
        Pickup.OnPlasmaPickup -= RemovePickup;
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
