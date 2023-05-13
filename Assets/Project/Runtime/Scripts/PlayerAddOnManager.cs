using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAddOnManager : GameBehaviour
{
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private PlayerWeaponController _weaponController;
    [SerializeField] private PlayerShieldController _shieldController;


    [SerializeField] private List<AddOnActiveState> _addOnActiveStates = new();
    public List<AddOnActiveState> AddOnActiveStates { get => _addOnActiveStates; }

    #region AddOn Effect Values
    [SerializeField] private int _batteryPack = 5;
    [SerializeField] private int _plasmaCache = 1;
    [SerializeField] private float _hydroCoolant = 0.25f;
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
        for (int i = 0; i < _addOnActiveStates.Count; i++)
        {
            _addOnActiveStates[i].IsAddOnActive = false;
        }
    }

    private void BuyOrSellAddOn(AddOnScriptableObject addOn, bool isBuyingAddOn)
    {
        int buyOrRefundValue = addOn.IonCost;
        buyOrRefundValue = isBuyingAddOn ? -buyOrRefundValue : buyOrRefundValue;
        PSM.ChangeIon(buyOrRefundValue);
        _addOnActiveStates[addOn.ID].IsAddOnActive = isBuyingAddOn;
    }

    public void CheckAddOnStates()
    {
        for (int i = 0; i < _addOnActiveStates.Count; i++)
        {
            switch (i)
            {
                case 0:
                    //battery pack
                    if (_addOnActiveStates[i].IsAddOnActive)
                    {
                        _weaponController.WeaponUpgradeDuration += _batteryPack;
                    }
                    break;
                //plasma cache
                case 1:
                    if (_addOnActiveStates[i].IsAddOnActive)
                    {
                        _playerManager.PlasmaCost -= _plasmaCache;
                    }
                    break;
                //hydro coolant
                case 2:
                    if (_addOnActiveStates[i].IsAddOnActive)
                    {
                        _weaponController.HeatPerShot -= _hydroCoolant;
                    }
                    break;
                //pulse detonator
                case 3:
                    if (_addOnActiveStates[i].IsAddOnActive)
                    {
                        _shieldController.IsPulseDetonator = true;
                    }
                    break;
            }
        }
    }
}

[Serializable]
public class AddOnActiveState
{
    public string Name { get; }
    public bool IsAddOnActive { get; set; }
}