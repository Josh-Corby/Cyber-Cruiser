using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AddOnManager : GameBehaviour
{
    private const string PLAYER_ION = "PlayerIon";
    [SerializeField] private int _playerIon;
    [SerializeField] private TMP_Text _ionText;

    [SerializeField] private AddOn _currentAddon;
    [SerializeField] private TMP_Text _addOnName;
    [SerializeField] private TMP_Text _addOnDescription;

    public int PlayerIon
    {
        get
        {
            return _playerIon;
        }
        set
        {
            _playerIon = value;
            _ionText.text = _playerIon.ToString();
        }
    }

    private void OnEnable()
    {
        PlayerManager.OnIonPickup += ChangeIon;
        AddOn.OnMouseEnter += SetCurrentAddOn;
        AddOn.OnMouseExit += ClearAddOn;
    }

    private void OnDisable()
    {
        PlayerManager.OnIonPickup -= ChangeIon;
        AddOn.OnMouseEnter -= SetCurrentAddOn;
        AddOn.OnMouseExit -= ClearAddOn;

    }

    void Start()
    {
        RestoreIon();
    }

    private void RestoreIon()
    {
        PlayerIon = PlayerPrefs.GetInt(nameof(PLAYER_ION));
    }

    private void ChangeIon(int value)
    {
        PlayerIon += value;
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

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt(nameof(PLAYER_ION), PlayerIon);
    }
}
