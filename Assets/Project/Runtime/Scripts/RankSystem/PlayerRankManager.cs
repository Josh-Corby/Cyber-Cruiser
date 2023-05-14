using System;
using UnityEngine;

public class PlayerRankManager : GameBehaviour<PlayerRankManager>
{
    private const string PLAYER_RANK = "PlayerRank";
    private const string PLAYER_STARS = "PlayerStars";

    #region Fields
    [SerializeField] private Rank _currentRank;
    private Rank _rankBeforeRankUp;
    private int _currentStars;
    private int _starsBeforeMissionStart;
    private int _starsToGain;
    [SerializeField]
    private int _totalStarReward;
    #endregion

    #region Proerties
    public Rank CurrentRank { get => _currentRank; }
    public Rank RankBeforeMissionStart { get => _rankBeforeRankUp; }
    public int CurrentStars { get => _currentStars; }
    public int StarsBeforeMissionStart { get => _starsBeforeMissionStart; }
    public int TotalStarReward { get => _totalStarReward; }
    #endregion

    private void OnEnable()
    {
        GameManager.OnMissionStart += StoreRankValues;
        MissionManager.OnMissionComplete += StartStarIncreaseProcess;      
    }

    private void OnDisable()
    {
        GameManager.OnMissionStart -= StoreRankValues;
        MissionManager.OnMissionComplete -= StartStarIncreaseProcess;
    }

    private void Start()
    {
        RestoreValues();
    }

    private void RestoreValues()
    {
        RestoreRank();
        RestoreStars();
    }

    private void RestoreRank()
    {
        if (!PlayerPrefs.HasKey(nameof(PLAYER_RANK)))
        {
            _currentRank = RM.GetRank(0);
            _rankBeforeRankUp = _currentRank;
        }

        else
        {
            _currentRank = RM.GetRank(0);
            //_currentRank = RM.GetRank(PlayerPrefs.GetInt(nameof(PLAYER_RANK)));
            _rankBeforeRankUp = _currentRank;
        }
    }

    private void RestoreStars()
    {
        if (!PlayerPrefs.HasKey(nameof(PLAYER_STARS)))
        {
            _currentStars = 0;
        }

        else
        {
            _currentStars = 0;
            //_currentStars = PlayerPrefs.GetInt(nameof(PLAYER_STARS));
            _starsBeforeMissionStart = _currentStars;
        }
    }

    private void StoreRankValues()
    {
        _rankBeforeRankUp = _currentRank;
        _starsBeforeMissionStart = _currentStars;
    }

    private void StartStarIncreaseProcess(int starsToGain)
    {
        _starsToGain = starsToGain;
        _totalStarReward = starsToGain;
        IncreaseStars();
    }

    private void IncreaseStars()
    {
        _currentStars = _starsToGain;

        if (_currentStars >= _currentRank.StarsToRankUp)
        {
            _starsToGain -= _currentRank.StarsToRankUp;
            RankUp();
        }
    }

    private void RankUp()
    {
        _currentRank = RM.RankUp(_currentRank.RankID);

        if (_starsToGain > 0)
        {
            IncreaseStars();
        }
    }

    private void SaveValues()
    {
        PlayerPrefs.SetInt(nameof(PLAYER_RANK), 0);
        //PlayerPrefs.SetInt(nameof(PLAYER_RANK), _currentRank.RankID);
        PlayerPrefs.SetInt(nameof(PLAYER_STARS), _currentStars);
    }

    private void OnApplicationQuit()
    {
        SaveValues();
    }
}
