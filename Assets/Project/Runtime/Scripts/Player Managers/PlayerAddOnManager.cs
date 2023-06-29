using System;
using System.Collections.Generic;
using UnityEngine;

namespace CyberCruiser
{
    public class PlayerAddOnManager : GameBehaviour
    {
        [SerializeField] private PlayerSaveManager _playerSaveManager;
        [SerializeField] private PlayerShieldController _shieldController;
        [SerializeField] private List<AddOnActiveState> _addOnActiveStates = new();

        [SerializeField] private float _batteryPackEffectInSeconds;

        [SerializeField] private BoolValue _doesPlayerHaveHydrocoolant;
        [SerializeField] private BoolValue _doesPlayerHavePlasmaCache;
       

        public List<AddOnActiveState> AddOnActiveStates { get => _addOnActiveStates; }
        public bool IsBatteryPackActive { get; private set; }
        public bool IsPlasmaCacheActive { get; private set; }
        public bool IsHydrocoolantActive { get; private set; }
        public bool IsPulseDetonatorActive { get; private set; }
        public bool IsRamAddOnActive{ get; private set; }

        [Serializable]
        public class AddOnActiveState
        {
            public string Name;
            public bool IsAddOnActive;
        }

        private void OnEnable()
        {
            AddOnButton.OnAddonBuyOrSell += BuyOrSellAddOn;
            DisableAllAddOns();
        }

        private void OnDisable()
        {
            AddOnButton.OnAddonBuyOrSell -= BuyOrSellAddOn;
        }
 
        private void DisableAllAddOns()
        {
            for (int i = 0; i < _addOnActiveStates.Count; i++)
            {
                _addOnActiveStates[i].IsAddOnActive = false;
            }

            IsBatteryPackActive = false;
            IsPlasmaCacheActive = false;
            IsHydrocoolantActive = false;
            IsPulseDetonatorActive = false;
            IsRamAddOnActive = false;
        }

        private void BuyOrSellAddOn(AddOnScriptableObject addOn, bool isBuyingAddOn)
        {
            int buyOrRefundValue = addOn.IonCost;
            buyOrRefundValue = isBuyingAddOn ? -buyOrRefundValue : buyOrRefundValue;
            _playerSaveManager.ChangeIon(buyOrRefundValue);
            ChangeAddOnActiveState(addOn.ID, isBuyingAddOn);
        }

        private void ChangeAddOnActiveState(int addOnId, bool isActive)
        {
            _addOnActiveStates[addOnId].IsAddOnActive = isActive;
            switch (addOnId)
            {
                case 0:
                    //battery pack
                    IsBatteryPackActive = _addOnActiveStates[addOnId].IsAddOnActive;
                    break;
                //plasma cache
                case 1:
                    IsPlasmaCacheActive = _addOnActiveStates[addOnId].IsAddOnActive;
                    break;
                //hydro coolant
                case 2:
                    IsHydrocoolantActive = _addOnActiveStates[addOnId].IsAddOnActive;
                    break;
                //pulse detonator
                case 3:
                    IsPulseDetonatorActive = _addOnActiveStates[addOnId].IsAddOnActive;
                    break;
            }
        }
    }

}