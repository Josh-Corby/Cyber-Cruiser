using System;
using UnityEngine;
using UnityEngine.UI;

public enum AddOnType
{
    BatteryPack, Hydrocoolant, PlasmaCache, PulseDetonator
}

[RequireComponent(typeof(Button))]
public class AddOnButton : GameBehaviour
{
    #region References
    [SerializeField] private AddOnScriptableObject _addOnInfo;
    private Button _addOnButton;
    #endregion

    #region Fields
    private AddOnType _addOnType;
    private string _name;
    private string _description;
    private int _ionCost;
    private bool _isAddOnEnabled;
    #endregion

    #region Properties
    public string Name { get => _name; }
    public string Description { get => _description; }
    #endregion

    #region Actions
    public static event Action<AddOnButton> OnMouseEnter = null;
    public static event Action OnMouseExit = null;
    public static event Action<AddOnType, int, bool> OnAddonBuyOrSell = null;
    #endregion

    private void Awake()
    {
        _addOnButton = GetComponent<Button>();
        AssignAddOnInfo();
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

    private void AssignAddOnInfo()
    {
        _addOnType = _addOnInfo.AddOnType;
        _name = _addOnType.ToString();
        _description = _addOnInfo.Description;
        _ionCost = _addOnInfo.IonCost;
    }

    private void ValidateButtonState(int playerIon)
    {
        if (!_isAddOnEnabled)
        {
            _addOnButton.interactable = playerIon >= _ionCost;
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
        OnAddonBuyOrSell?.Invoke(_addOnType, _ionCost, _isAddOnEnabled);
    }

    public void MouseEnter()
    {
        OnMouseEnter(this);
    }

    public void MouseExit()
    {
        OnMouseExit?.Invoke();
    }
}
