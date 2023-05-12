using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRankManager : GameBehaviour
{

    [Header("Rank Info")]
    [SerializeField] private Rank _currentRank;
    [SerializeField] private Rank _rankBeforeRankUp;
    [SerializeField] private int _currentStars;
    [SerializeField] private int _starsBeforeStarGain;
    [SerializeField] private int _starsToGain;
    [SerializeField] private int _totalStarReward;

    public Rank CurrentRank { get => _currentRank; private set => _currentRank = value; }
    public Rank RankBeforeMissionStart { get => _rankBeforeRankUp; private set => _rankBeforeRankUp = value; }
    public int CurrentStars { get => _currentStars; private set => _currentStars = value; }
    public int StarsBeforeMissionStart { get => _starsBeforeStarGain; private set => _starsBeforeStarGain = value; }
    public int StarsToGain { get => _starsToGain; private set => _starsToGain = value; }
    public int TotalStarReward { get => _totalStarReward; private set => _totalStarReward = value; }


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
}
