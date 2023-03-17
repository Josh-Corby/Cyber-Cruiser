using System;
using UnityEngine;

public class Pickup : GameBehaviour
{
    public static event Action<int> OnPlasmaPickup = null;
    [SerializeField] private int plasmaAmount;

    public enum PickupType
    {
        Weapon, Plasma
    }

    [SerializeField] private PickupType pickup;

    public void PickupEffect()
    {
        switch (pickup)
        {
            case PickupType.Plasma:
                OnPlasmaPickup(plasmaAmount);
                break;
        }
    }
}
