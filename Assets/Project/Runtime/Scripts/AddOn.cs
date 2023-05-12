using System;
using UnityEngine;
using UnityEngine.UI;

public enum AddOnTypes
{
    BatteryPack, Hydrocoolant, PlasmaCache, PulseDetonator
}
public class AddOn : GameBehaviour
{
    [SerializeField] private AddOnScriptableObject _addOnInfo;
    private Button _addOnButton;
    private AddOnTypes _addOnType;
    private string _name;
    private string _description;
    private int _ionCost;
    private bool _isAddOnEnabled;

    #region Properties
    public string Name { get => _name; }

    public string Description { get => _description; }

    public bool IsAddonEnabled
    {
        get => _isAddOnEnabled;
        private set => _isAddOnEnabled = value;
    }
    #endregion

    #region Actions
    public static event Action<AddOn> OnMouseEnter = null;
    public static event Action OnMouseExit = null;
    public static event Action<AddOnTypes, int, bool> OnAddOnToggled = null;
    #endregion

    private void Awake()
    {
        _addOnButton = GetComponent<Button>();
        AssignAddOnInfo();
    }

    private void OnEnable()
    {
        PlayerStatsManager.OnIonChange += (playerIon) => { ValidateButtonState(); };
        ValidateButtonState();
    }

    private void OnDisable()
    {
        PlayerStatsManager.OnIonChange -= (playerIon) => { ValidateButtonState(); };
    }

    private void AssignAddOnInfo()
    {
        _addOnType = _addOnInfo.AddOnType;
        _name = _addOnType.ToString();
        _description = _addOnInfo.Description;
        _ionCost = _addOnInfo.IonCost;
    }

    private void ValidateButtonState()
    {
        if (!IsAddonEnabled)
        {
            _addOnButton.interactable = CanPlayerAffordAddOn();
        }

        else
        {
            _addOnButton.interactable = true;
        }     
    }

    private bool CanPlayerAffordAddOn()
    {
        return PSM.CanPlayerAffordAddon(_ionCost);
    }

    #region UI Functions
    public void ToggleAddOnActiveState()
    {
        _isAddOnEnabled = !_isAddOnEnabled;
        OnAddOnToggled(_addOnType, _ionCost, _isAddOnEnabled);
    }

    public void MouseEnter()
    {
        OnMouseEnter(this);
    }

    public void MouseExit()
    {
        OnMouseExit?.Invoke();
    }  
    #endregion
}
