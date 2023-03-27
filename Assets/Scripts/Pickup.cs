using System;
using UnityEngine;

public enum PickupType
{
    Scatter, Pulverizer, Plasma
}
public class Pickup : GameBehaviour
{
    public static event Action<int> OnPlasmaIncrease = null;
    public static event Action<GameObject> OnPlasmaPickup = null;
    public static event Action<PickupType> OnWeaponUpgradePickup = null;

    [SerializeField] private int plasmaAmount;
    [SerializeField] private float _speed;
    [SerializeField] private PickupType pickup;

    public void PickupEffect()
    {
        switch (pickup)
        {
            case PickupType.Plasma:
                OnPlasmaIncrease(plasmaAmount);
                OnPlasmaPickup(gameObject);
                break;
            case PickupType.Scatter:
                OnWeaponUpgradePickup(PickupType.Scatter);
                break;
            case PickupType.Pulverizer:
                OnWeaponUpgradePickup(PickupType.Pulverizer);
                break;
        }
    }

    private void Update()
    {
        MoveForward();
    }

    private void MoveForward()
    {
        transform.position += _speed * Time.deltaTime * transform.up;
    }
}
