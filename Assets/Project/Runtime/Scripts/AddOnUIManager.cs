using TMPro;
using UnityEngine;

public class AddOnUIManager : GameBehaviour
{
    [SerializeField] private TMP_Text _addOnName;
    [SerializeField] private TMP_Text _addOnDescription;
    private AddOn _currentAddon;

    private void OnEnable()
    {
        AddOn.OnMouseEnter += SetCurrentAddOn;
        AddOn.OnMouseExit += ClearAddOn;
    }

    private void OnDisable()
    {
        AddOn.OnMouseEnter -= SetCurrentAddOn;
        AddOn.OnMouseExit -= ClearAddOn;
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
