using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AddOnUIManager : GameBehaviour
{
    [SerializeField] private TMP_Text _ionText;

    [SerializeField] private AddOn _currentAddon;
    [SerializeField] private TMP_Text _addOnName;
    [SerializeField] private TMP_Text _addOnDescription;

    private void OnEnable()
    {
        PlayerStatsManager.OnIonChange += ChangeIonText;
        AddOn.OnMouseEnter += SetCurrentAddOn;
        AddOn.OnMouseExit += ClearAddOn;
    }

    private void OnDisable()
    {
        PlayerStatsManager.OnIonChange -= ChangeIonText;
        AddOn.OnMouseEnter -= SetCurrentAddOn;
        AddOn.OnMouseExit -= ClearAddOn;
    }

    private void ChangeIonText(int value)
    {
        _ionText.text = value.ToString();
    }
    private void SetCurrentAddOn(AddOn addOn)
    {
        _currentAddon = addOn;
        SetAddOnUI();
    }

    private void SetAddOnUI()
    {
        _addOnName.text = _currentAddon.Name;
        _addOnDescription.text = _currentAddon.Description;
    }

    private void ClearAddOn()
    {
        _currentAddon = null;
        ClearAddOnUI();
    }

    private void ClearAddOnUI()
    {
        _addOnName.text = "";
        _addOnDescription.text = "";
    }
}
