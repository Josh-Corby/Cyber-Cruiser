using System;
using UnityEngine;
using UnityEngine.UI;

namespace CyberCruiser
{
    [RequireComponent(typeof(Button))]
    public class AddOnButton : GameBehaviour
    {
        [SerializeField] private PickUpScriptableObject _addOnInfo;
        private int _addOnCost;
        private Button _addOnButton;
        [SerializeField] public bool _doesPlayerHaveAddOn;
        [SerializeField] private AddonType _addonType;

        [SerializeField] private IntReference _playerIonReference;

        public static event Action<PickUpScriptableObject> OnMouseEnter = null;
        public static event Action OnMouseExit = null;
        public static event Action<PickUpScriptableObject, bool> OnAddonBuyOrSell = null;

        private enum AddonType
        {
            BatteryPack, HydroCoolant, PlasmaCache, PulseDetonator
        }

        private void Awake()
        {
            _addOnButton = GetComponent<Button>();
            //_addOnCost = _addOnInfo.IonCost;       
        }

        private void OnEnable()
        {
            PlayerSaveManager.OnIonChange -= ValidateButtonState;
            ValidateButtonState(_playerIonReference.Value);
        }

        private void OnDisable()
        {
            PlayerSaveManager.OnIonChange -= ValidateButtonState;
        }

        private void ValidateButtonState(int playerIon)
        {
            if (!_doesPlayerHaveAddOn)
            {
                _addOnButton.interactable = playerIon >= _addOnCost;
            }

            else
            {
                _addOnButton.interactable = true;
            }
        }

        public void ToggleAddOnActiveState()
        {
            _doesPlayerHaveAddOn = !_doesPlayerHaveAddOn;
            BuyOrSellAddOn();
        }

        private void BuyOrSellAddOn()
        {
            OnAddonBuyOrSell?.Invoke(_addOnInfo, _doesPlayerHaveAddOn);
        }

        public void MouseEnter()
        {
            OnMouseEnter(_addOnInfo);
        }

        public void MouseExit()
        {
            OnMouseExit?.Invoke();
        }
    }
}