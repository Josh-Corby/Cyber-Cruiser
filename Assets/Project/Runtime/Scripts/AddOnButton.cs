using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AddOnButton : GameBehaviour
{
    [SerializeField] private AddOnScriptableObject _addOnInfo;
    private int _addOnCost;
    private Button _addOnButton;
    private bool _isAddOnEnabled;

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
        ValidateButtonState(PSM.PlayerIon);
    }

    private void OnDisable()
    {
        PlayerStatsManager.OnIonChange -= ValidateButtonState;
    }

    private void ValidateButtonState(int playerIon)
    {
        if (!_isAddOnEnabled)
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
        _isAddOnEnabled = !_isAddOnEnabled;
        BuyOrSellAddOn();
    }

    private void BuyOrSellAddOn()
    {
        OnAddonBuyOrSell?.Invoke(_addOnInfo, _isAddOnEnabled);
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
