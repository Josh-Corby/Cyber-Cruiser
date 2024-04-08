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
        #endregion

        #region Properties
        public Rank CurrentRank { get => _currentRank; }
        public Rank RankBeforeMissionStart { get => _rankBeforeMissionStart; }
        public int CurrentStars
        {
            get => _currentStars;

            set
            {
                _currentStars = value;
                if(_currentStars >= _currentRank.StarsToRankUp)
                {
                    RankUp();
                }
            }
        }
        public int StarsBeforeMissionStart { get => _starsBeforeMissionStart; }
        public int TotalStarReward { get => _totalStarReward; }
        #endregion

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
            _currentRank = RankManagerInstance.GetRank(PlayerPrefs.GetInt(PLAYER_RANK, 0));
            _rankBeforeMissionStart = _currentRank;
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
            CurrentStars -= _currentRank.StarsToRankUp;
            _currentRank = RankManagerInstance.RankUp(_currentRank.RankID);
            if (_starsToGain > 0)
            {
                IncreaseStars();
            }
        }

        private void SaveValues()
        {
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