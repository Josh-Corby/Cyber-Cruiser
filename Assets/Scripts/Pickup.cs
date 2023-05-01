using System;
using UnityEngine;

public enum PickupType
{
    Weapon, Plasma, Health
}

public enum WeaponUpgradeType
{
    None, Scatter_Random, Scatter_Fixed, Pulverizer, Homing, ChainLightning
}

public class Pickup : GameBehaviour
{
    #region Fields
    public PickupType _pickupType;
    public WeaponUpgradeType _upgradeType;
    [SerializeField] private float _speed;
    [SerializeField] private int _healthAmount;
    [SerializeField] private int _plasmaAmount;
    [SerializeField] private int _ionAmount;
    #endregion

    #region Actions
    public static event Action<int, int, int> OnResourcePickup = null;
    public static event Action<GameObject> OnPickup = null;
    public static event Action<WeaponUpgradeType> OnWeaponUpgradePickup = null;
    #endregion

    public void PickupEffect()
    {
        switch (_pickupType)
        {
            case PickupType.Plasma:
            case PickupType.Health:
                OnResourcePickup(_healthAmount, _plasmaAmount, _ionAmount);
                break;
            case PickupType.Weapon:
                switch (_upgradeType)
                {
                    case WeaponUpgradeType.None:
                        return;

                    case WeaponUpgradeType.Scatter_Fixed:
                    case WeaponUpgradeType.Scatter_Random:
                    case WeaponUpgradeType.Pulverizer:
                    case WeaponUpgradeType.Homing:
                    case WeaponUpgradeType.ChainLightning:
                        OnWeaponUpgradePickup(_upgradeType);
                        break;
                }
                break;
        }
        OnPickup(gameObject);
    }

    private void Update()
    {
        if (GM.IsPaused) return;
        MoveForward();
    }

    private void MoveForward()
    {
        transform.position += _speed * Time.deltaTime * transform.up;
    }
}
