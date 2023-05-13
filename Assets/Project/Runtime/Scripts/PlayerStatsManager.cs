using System;
using UnityEngine;

public class PlayerStatsManager : GameBehaviour<PlayerStatsManager>
{
    private const string PLAYER_PLASMA = "PlayerPlasma";
    private const string PLAYER_ION = "PlayerIon";
    private int _playerIon;
    private int _playerPlasma;

    public int PlayerIon { get => _playerIon; }
    public int PlayerPlasma { get => _playerPlasma; }

    public static event Action<int> OnIonChange = null;
    public static event Action<int> OnPlasmaChange = null;

    private void OnEnable()
    {
        PlayerManager.OnIonPickup += ChangeIon;
        PlayerManager.OnPlasmaChange += SetPlasma;
        RestoreValues();
    }

    private void OnDisable()
    {
        PlayerManager.OnIonPickup -= ChangeIon;
        PlayerManager.OnPlasmaChange -= SetPlasma;
    }

    private void RestoreValues()
    {
        _playerIon = PlayerPrefs.GetInt(nameof(PLAYER_ION));
        _playerPlasma = PlayerPrefs.GetInt(nameof(PLAYER_PLASMA));
    }

    public void ChangeIon(int value)
    {
        _playerIon += value;
        _playerIon = ValidateValue(_playerIon);
        OnIonChange?.Invoke(_playerIon);
    }

    public void SetPlasma(int value)
    {
        _playerPlasma = value;
        _playerPlasma = ValidateValue(_playerPlasma);
        OnPlasmaChange?.Invoke(_playerPlasma);
    }

    private int ValidateValue(int value)
    {
        value = value < 0 ? 0 : value;
        return value;
    }

    private void SaveValues()
    {
        PlayerPrefs.SetInt(nameof(PLAYER_PLASMA), _playerPlasma);
        PlayerPrefs.SetInt(nameof(PLAYER_ION), _playerIon);
    }

    private void OnApplicationQuit()
    {
        SaveValues();
    }
}
