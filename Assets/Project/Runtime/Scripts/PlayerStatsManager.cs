using DG.Tweening.CustomPlugins;
using System;
using UnityEngine;

public class PlayerStatsManager : GameBehaviour<PlayerStatsManager>
{
    #region Fields
    [SerializeField] private int _playerIon;
    [SerializeField] private int _playerPlasma;
    [SerializeField] private int _plasmaCost;
    [SerializeField] private float _playerCurrentMaxHealth = 5;
    [SerializeField] private float _heatPerShot = 1.75f;
    [SerializeField] private float _weaponUpgradeDuration = 10;

    private float _iFramesDuration = 0.3f;


    [Header("Player prefs accessor strings")]
    private const string PLAYER_PLASMA = "PlayerPlasma";
    private const string PLAYER_ION = "PlayerIon";
    private const string PLAYER_RANK = "PlayerRank";
    private const string PLAYER_STARS = "PlayerStars";
    #endregion

    #region Properties
    private int PlayerPlasma
    {
        get => _playerPlasma;
        set
        {
            value = value <=0 ? 0 : value;
            _playerPlasma = value;
            OnPlasmaChange?.Invoke(_playerPlasma);
        }
    }
    private int PlayerIon
    {
        get => _playerIon;
        set
        {
            value = value <= 0 ? 0 : value;
            _playerIon = value;
            OnIonChange?.Invoke(_playerIon);
        }
    }
    #endregion

    #region Actions
    public static event Action<int> OnIonChange = null;
    public static event Action<int> OnPlasmaChange = null;
    public static event Action<int> OnStarsGained = null;
    #endregion

    private void OnEnable()
    {
        PlayerManager.OnIonPickup += ChangeIon;
        PlayerManager.OnPlasmaChange += ChangePlasma;
    }

    private void OnDisable()
    {
        PlayerManager.OnIonPickup -= ChangeIon;
        PlayerManager.OnPlasmaChange -= ChangePlasma;
    }

    private void Start()
    {
        RestoreValues();
    }

    #region Restore Values
    private void RestoreValues()
    {
        PlayerIon = PlayerPrefs.GetInt(nameof(PLAYER_ION));
        PlayerPlasma = PlayerPrefs.GetInt(nameof(PLAYER_PLASMA));
        PlasmaCost = 5;
        RestoreRank();
        RestoreStars();
    }

    private void RestoreRank()
    {
        if (!PlayerPrefs.HasKey(nameof(PLAYER_RANK)))
        {
            Debug.Log("no current player rank");
            CurrentRank = RM.GetRank(0);
            RankBeforeMissionStart = CurrentRank;
        }

        else
        {
            CurrentRank = RM.GetRank(PlayerPrefs.GetInt(nameof(PLAYER_RANK)));
            RankBeforeMissionStart = CurrentRank;
            Debug.Log("rank of " + CurrentRank.Name + " restored");
        }
    }

    private void RestoreStars()
    {
        if (!PlayerPrefs.HasKey(nameof(PLAYER_STARS)))
        {
            Debug.Log("no saved stars");
            CurrentStars = 0;
        }
        else
        {
            CurrentStars = PlayerPrefs.GetInt(nameof(PLAYER_STARS));
            StarsBeforeMissionStart = CurrentStars;
            Debug.Log(CurrentStars + " stars restored");
        }
    }
    #endregion

    #region Data Changing Functions
    public void ChangeIon(int value)
    {
        PlayerIon += value;
    }

    public void ChangePlasma(int value)
    {
        PlayerPlasma = value;
    }
    #endregion

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt(nameof(PLAYER_PLASMA), PlayerPlasma);
        PlayerPrefs.SetInt(nameof(PLAYER_ION), PlayerIon);

        PlayerPrefs.SetInt(nameof(PLAYER_RANK), 0);
        //PlayerPrefs.SetInt(nameof(PLAYER_RANK), CurrentRank.RankID);
        PlayerPrefs.SetInt(nameof(PLAYER_STARS), CurrentStars);
    }
}
