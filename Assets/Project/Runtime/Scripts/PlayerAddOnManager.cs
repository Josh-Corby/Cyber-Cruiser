using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAddOnManager : GameBehaviour
{
    #region AddOn Bools
    [SerializeField] private bool _isBatteryPack;
    [SerializeField] private bool _isHydrocoolant;
    [SerializeField] private bool _isPlasmaCache;
    [SerializeField] private bool _isPulseDetonator;
    #endregion

    #region AddOn Effect Values
    [SerializeField] private int _batteryPack = 5;
    [SerializeField] private int _plasmaCache = 1;
    [SerializeField] private float _hydroCoolant = 0.25f;
    #endregion

    #region AddOn Properties
    public bool IsBatteryPack { get => _isBatteryPack; private set => IsBatteryPack = value; }
    public bool IsHydrocoolant { get => _isHydrocoolant; private set => _isHydrocoolant = value; }
    public bool IsPlasmaCache { get => _isPlasmaCache; private set => _isPlasmaCache = value; }
    public bool IsPulseDetonator { get => _isPulseDetonator; private set { _isPulseDetonator = value; } }
    #endregion

    private void OnEnable()
    {
        AddOnButton.OnAddonBuyOrSell += BuyOrSellAddOn;
        GameManager.OnMissionEnd += DisableAllAddOns;
    }

    private void OnDisable()
    {
        AddOnButton.OnAddonBuyOrSell -= BuyOrSellAddOn;
        GameManager.OnMissionEnd -= DisableAllAddOns;
    }

    private void DisableAllAddOns()
    {
        if (IsBatteryPack)
        {
            IsBatteryPack = false;
        }

        if (IsHydrocoolant)
        {
            IsHydrocoolant = false;
        }

        if (IsPlasmaCache)
        {
            IsPlasmaCache = false;
        }

        if (IsPulseDetonator)
        {
            IsPulseDetonator = false;
        }
    }

    private void BuyOrSellAddOn(AddOnType addOnType, int value, bool isBuyingAddOn)
    {
        value = isBuyingAddOn ? -value : value;
        PSM.ChangeIon(value);
        ToggleAddOnBool(addOnType, isBuyingAddOn);
    }

    private void ToggleAddOnBool(AddOnType addOnType, bool value)
    {
        switch (addOnType)
        {
            case AddOnType.BatteryPack:
                IsBatteryPack = value;
                break;
            case AddOnType.Hydrocoolant:
                IsHydrocoolant = value;
                break;
            case AddOnType.PlasmaCache:
                IsPlasmaCache = value;
                break;
            case AddOnType.PulseDetonator:
                IsPulseDetonator = value;
                break;
        }
    }
}
