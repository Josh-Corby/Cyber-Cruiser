using System;
using UnityEngine;
using UnityEngine.UI;

namespace CyberCruiser
{
    [RequireComponent(typeof(Button))]
    public class AddOnButton : GameBehaviour
    {
        [SerializeField] private AddOnScriptableObject _addOnInfo;
        private int _addOnCost;
        private Button _addOnButton;
        private bool _doesPlayerHaveAddOn;

        public static event Action<AddOnScriptableObject> OnMouseEnter = null;
        public static event Action OnMouseExit = null;
        public static event Action<AddOnScriptableObject, bool> OnAddonBuyOrSell = null;

        private void Awake()
        {
            _addOnButton = GetComponent<Button>();
            _addOnCost = _addOnInfo.IonCost;
        }

        private void OnEnable()
        {
            PlayerStatsManager.OnIonChange -= ValidateButtonState;
            ValidateButtonState(PlayerStatsManagerInstance.PlayerIon);
        }

        private void OnDisable()
        {
            PlayerStatsManager.OnIonChange -= ValidateButtonState;
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