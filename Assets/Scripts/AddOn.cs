using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum AddOnTypes
{
    BatteryPack, Hydrocoolant, PlasmaCache, PulseDetonator
}
public class AddOn : MonoBehaviour
{
    [SerializeField] private AddOnScriptableObject _addOnInfo;
    private string _name, _description;
    private int _ionCost;
    private AddOnTypes _addOnType;
    private bool _isAddOnEnabled;

    public string Name { get => _name; }


    public string Description { get => _description; }

    public static event Action<AddOn> OnMouseEnter = null;
    public static event Action OnMouseExit = null;
    public static event Action<AddOnTypes, int, bool> OnAddOnToggled = null;

    private void Awake()
    {
        AssignAddOnInfo();
    }

    private void AssignAddOnInfo()
    {
        _addOnType = _addOnInfo.AddOnType;
        _name = _addOnType.ToString();
        _description = _addOnInfo.Description;
        _ionCost = _addOnInfo.IonCost;
    }

    public void MouseEnter()
    {
        OnMouseEnter(this);
    }

    public void MouseExit()
    {
        OnMouseExit?.Invoke();
    }

    public void ToggleAddOn()
    {
        _isAddOnEnabled = !_isAddOnEnabled;
        OnAddOnToggled(_addOnType, _ionCost, _isAddOnEnabled);
    }
}
