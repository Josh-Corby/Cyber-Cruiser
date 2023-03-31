using System;
using UnityEngine;

public enum PickupType
{
    Weapon, Plasma, Health
}

public enum WeaponUpgradeType
{
    None, Scatter_Random,Scatter_Fixed, Pulverizer
}
public class Pickup : GameBehaviour
{
    public static event Action<int> OnPlasmaPickup = null;
    public static event Action<int> OnHealthPickup = null;
    public static event Action<GameObject> OnPickup = null;
    public static event Action<WeaponUpgradeType, float> OnWeaponUpgradePickup = null;

    public PickupType _pickupType;
    public WeaponUpgradeType _upgradeType;
    [SerializeField] private float _speed;
    [SerializeField] private int _plasmaAmount;
    [SerializeField] private int _healthAmount;
    [SerializeField] private float _upgradeDuration;

    public void PickupEffect()
    {
        switch (_pickupType)
        {
            case PickupType.Plasma:
                OnPlasmaPickup(_plasmaAmount);           
                break;
            case PickupType.Health:
                OnHealthPickup(_healthAmount);
                break;
            case PickupType.Weapon:
                switch (_upgradeType) 
                {
                    case WeaponUpgradeType.None:
                        return;

                    case WeaponUpgradeType.Scatter_Fixed:
                    case WeaponUpgradeType.Scatter_Random:
                    case WeaponUpgradeType.Pulverizer:
                        OnWeaponUpgradePickup(_upgradeType, _upgradeDuration);
                        break;
                }
                break;
        }
        OnPickup(gameObject);
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
