using System;
using UnityEngine;

namespace CyberCruiser
{
    public class PlayerRankManager : GameBehaviour
    {
        private const string PLAYER_RANK = "PlayerRank";
        private const string PLAYER_STARS = "PlayerStars";

        #region Fields
        [SerializeField] private Rank _currentRank;
        private Rank _rankBeforeMissionStart;
        [SerializeField] private int _currentStars;
        [SerializeField] private int _starsBeforeMissionStart;
        [SerializeField] private int _starsToGain;
        [SerializeField] private int _totalStarReward;
        [SerializeField] private GameManager _Gamemanager;
        #endregion

        #region Properties
        public Rank CurrentRank { get => _currentRank; }
        public Rank RankBeforeMissionStart { get => _rankBeforeMissionStart; }
        public int CurrentStars
        {
            get => _currentStars;

            set
            {
                _currentStars = (int)MathF.Max(0, value);
                if(_currentStars >= _currentRank.StarsToRankUp)
                {
                    RankUp();
                }
            }
        }
        public int StarsBeforeMissionStart { get => _starsBeforeMissionStart; }
        public int TotalStarReward { get => _totalStarReward; }
        #endregion

        public static event Action<int> OnRankUp;
        public static event Action<int> OnRankLoaded;

        private void OnEnable()
        {
            GameManager.OnMissionEnd += StoreRankValues;
            MissionManager.OnMissionComplete += StartStarIncreaseProcess;
            SaveManager.OnSaveData += SaveValues;
            SaveManager.OnClearSaveData += ClearSaveData;
        }

        private void OnDisable()
        {
            GameManager.OnMissionEnd -= StoreRankValues;
            MissionManager.OnMissionComplete -= StartStarIncreaseProcess;
            SaveManager.OnSaveData -= SaveValues;
            SaveManager.OnClearSaveData -= ClearSaveData;
        }

        private void Start()
        {
            RestoreValues();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                RankUp();
            }
        }

        private void RestoreValues()
        {
            RestoreRank();
            RestoreStars();
        }

        private void RestoreRank()
        {
            Debug.Log(PlayerPrefs.GetInt(PLAYER_RANK));
            _currentRank = RankManagerInstance.GetRank(PlayerPrefs.GetInt(PLAYER_RANK, 0));
            _rankBeforeMissionStart = _currentRank;
            OnRankLoaded?.Invoke(_currentRank.RankID);
        }

        private void RestoreStars()
        {
            CurrentStars = PlayerPrefs.GetInt(PLAYER_STARS, 0);
            _starsBeforeMissionStart = _currentStars;
        }

        private void StoreRankValues()
        {
            _rankBeforeMissionStart = _currentRank;
            _starsBeforeMissionStart = _currentStars;
        }

        private void StartStarIncreaseProcess(int starsToGain)
        {
            Debug.Log(starsToGain + " stars gained");
            _starsToGain = starsToGain;
            _totalStarReward = starsToGain;
            IncreaseStars();
        }

        private void IncreaseStars()
        {
            CurrentStars += _starsToGain;

        }

        private void RankUp()
        {
            _starsToGain -= _currentRank.StarsToRankUp;
            _starsToGain = (int)MathF.Max(0, _starsToGain);
            CurrentStars -= _currentRank.StarsToRankUp;
            _currentRank = RankManagerInstance.RankUp(_currentRank.RankID);
            if(!_Gamemanager.InMission)
            {
                _rankBeforeMissionStart = _currentRank;
            }

            if (_starsToGain > 0)
            {
                IncreaseStars();
            }

            OnRankUp?.Invoke(_currentRank.RankID);
        }

        private void SaveValues()
        {
            Debug.Log("Saving rank of " + _currentRank.RankID);
            PlayerPrefs.SetInt(PLAYER_RANK, _currentRank.RankID);
            PlayerPrefs.SetInt(PLAYER_STARS, _currentStars);
        }

        private void ClearSaveData()
        {
            _currentRank = RankManagerInstance.GetRank(0);
            _rankBeforeMissionStart = RankManagerInstance.GetRank(0);
            _currentStars = 0;
            _starsBeforeMissionStart = 0;
        }
    }
}