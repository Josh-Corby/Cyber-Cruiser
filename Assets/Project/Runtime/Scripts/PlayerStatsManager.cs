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
    [SerializeField] private bool _isBatteryPack, _isHydrocoolant, _isPlasmaCache, _isPulseDetonator;

    [Header("Rank Info")]
    [SerializeField] private Rank _currentRank;
    [SerializeField] private Rank _rankBeforeRankUp;
    [SerializeField] private int _currentStars;
    [SerializeField] private int _starsBeforeStarGain;
    [SerializeField] private int _starsToGain;
    [SerializeField] private int _totalStarReward;

    [Header("Player prefs accessor strings")]
    private const string PLAYER_PLASMA = "PlayerPlasma";
    private const string PLAYER_ION = "PlayerIon";
    private const string PLAYER_RANK = "PlayerRank";
    private const string PLAYER_STARS = "PlayerStars";
    #endregion

    #region Properties
    public Rank CurrentRank { get => _currentRank; private set => _currentRank = value; }
    public Rank RankBeforeMissionStart { get => _rankBeforeRankUp; private set => _rankBeforeRankUp = value; }
    public int CurrentStars { get => _currentStars; private set => _currentStars = value; }
    public int StarsBeforeMissionStart { get => _starsBeforeStarGain; private set => _starsBeforeStarGain = value; }
    public int StarsToGain { get => _starsToGain; private set => _starsToGain = value; }
    public int TotalStarReward { get => _totalStarReward; private set => _totalStarReward = value; }
    public int PlasmaCost { get => _plasmaCost; private set => _plasmaCost = value; }
    public float PlayerCurrentMaxHealth { get => _playerCurrentMaxHealth; private set => _playerCurrentMaxHealth = value; }
    public float HeatPerShot { get => _heatPerShot; private set => _heatPerShot = value; }
    public float WeaponUpgradeDuration { get => _weaponUpgradeDuration; private set => _weaponUpgradeDuration = value; }
    public float IFramesDuration { get => _iFramesDuration; private set => _iFramesDuration = value; }
    public bool IsPulseDetonator { get => _isPulseDetonator; private set { _isPulseDetonator = value; } }
    public int SetPlayerPlasma
    {
        get => _playerPlasma;
        private set
        {
            _playerPlasma = value;
            OnPlasmaChange(value);
        }
    }
    public int SetPlayerIon
    {
        get => _playerIon;
        private set
        {
            _playerIon = value;
            OnIonChange(value);
        }
    }
    public bool IsBatteryPack
    {
        get => _isBatteryPack;
        private set
        {
            _isBatteryPack = value;
            ToggleBatteryPack(_isBatteryPack);
        }
    }
    public bool IsHydrocoolant
    {
        get => _isHydrocoolant;
        private set
        {
            _isHydrocoolant = value;
            ToggleHydrocoolant(_isHydrocoolant);
        }
    }
    public bool IsPlasmaCache
    {
        get => _isPlasmaCache;
        private set
        {
            _isPlasmaCache = value;
            TogglePlasmaCache(_isPlasmaCache);
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
        AddOn.OnAddOnToggled += ToggleAddOnBool;
        PlayerManager.OnIonPickup += ChangeIon;
        PlayerManager.OnPlasmaChange += ChangePlasma;
        GameManager.OnMissionStart += StoreRankValues;
        GameManager.OnMissionEnd += DisableAllAddOns;
        MissionManager.OnMissionComplete += StartStarIncreaseProcess;
    }

    private void OnDisable()
    {
        AddOn.OnAddOnToggled -= ToggleAddOnBool;
        PlayerManager.OnIonPickup -= ChangeIon;
        PlayerManager.OnPlasmaChange -= ChangePlasma;
        GameManager.OnMissionStart -= StoreRankValues;
        GameManager.OnMissionEnd -= DisableAllAddOns;
        MissionManager.OnMissionComplete -= StartStarIncreaseProcess;
    }

    private void Start()
    {
        RestoreValues();
    }

    #region Data Changing Functions
    private void RestoreValues()
    {
        SetPlayerIon = PlayerPrefs.GetInt(nameof(PLAYER_ION));
        SetPlayerPlasma = PlayerPrefs.GetInt(nameof(PLAYER_PLASMA));
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

    public void ChangeIon(int value)
    {
        SetPlayerIon += value;
    }

    public void ChangePlasma(int value)
    {
        SetPlayerPlasma = value;
    }
    #endregion

    #region Rank Functions
    private void StoreRankValues()
    {
        RankBeforeMissionStart = CurrentRank;
        StarsBeforeMissionStart = CurrentStars;
    }

    private void StartStarIncreaseProcess(int starsToGain)
    {
        Debug.Log(starsToGain);
        StarsToGain = starsToGain;
        TotalStarReward = starsToGain;

        IncreaseStars();
    }

    private void IncreaseStars()
    {
        _currentStars = StarsToGain;

        if (_currentStars >= CurrentRank.StarsToRankUp)
        {
            StarsToGain -= CurrentRank.StarsToRankUp;
            RankUp();
        }
    }

    private void RankUp()
    {
        Debug.Log("player rank up");
        CurrentRank = RM.RankUp(CurrentRank.RankID);

        if (StarsToGain > 0)
        {
            IncreaseStars();
        }
    }
    #endregion

    #region Addon Functions
    private void DisableAllAddOns()
    {
        if (IsBatteryPack)
        {
            IsBatteryPack = false;
        }
        if (IsHydrocoolant)
        {
            IsHydrocoolant = false;
        }
        if (IsPlasmaCache)
        {
            IsPlasmaCache = false;
        }
        if (IsPulseDetonator)
        {
            IsPulseDetonator = false;
        }
    }

    private void ToggleAddOnBool(AddOnTypes addOnType, int cost, bool value)
    {
        //If AddOn is being bought check if player can afford it
        if (value)
        {
            CanPlayerAffordAddon(cost);
        }

        //Spend or refund ions depending on bool state
        SetPlayerIon += value ? -cost : cost;
        //Process ion change
        OnIonChange(SetPlayerIon);

        //Find function related to addon type
        switch (addOnType)
        {
            case AddOnTypes.BatteryPack:
                IsBatteryPack = value;
                break;
            case AddOnTypes.Hydrocoolant:
                IsHydrocoolant = value;
                break;
            case AddOnTypes.PlasmaCache:
                IsPlasmaCache = value;
                break;
            case AddOnTypes.PulseDetonator:
                IsPulseDetonator = value;
                break;
        }
    }

    private bool CanPlayerAffordAddon(int cost)
    {
        return SetPlayerIon > cost;
    }
    #endregion

    #region AddOnEffects
    private void ToggleBatteryPack(bool value)
    {
        WeaponUpgradeDuration += value ? 5 : -5;
    }

    private void ToggleHydrocoolant(bool value)
    {
        HeatPerShot += value ? -0.25f : 0.25f;
    }

    private void TogglePlasmaCache(bool value)
    {
        PlasmaCost += value ? -1 : 1;
    }
    #endregion

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt(nameof(PLAYER_PLASMA), SetPlayerPlasma);
        PlayerPrefs.SetInt(nameof(PLAYER_ION), SetPlayerIon);

        PlayerPrefs.SetInt(nameof(PLAYER_RANK), 0);
        //PlayerPrefs.SetInt(nameof(PLAYER_RANK), CurrentRank.RankID);
        PlayerPrefs.SetInt(nameof(PLAYER_STARS), CurrentStars);
    }
}