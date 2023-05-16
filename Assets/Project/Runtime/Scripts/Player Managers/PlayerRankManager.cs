using UnityEngine;

namespace CyberCruiser
{
    public class PlayerRankManager : GameBehaviour
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
            _currentRank = RankManagerInstance.GetRank(0);
            //_currentRank = RankManagerInstance.GetRank(PlayerPrefs.GetInt(PLAYER_RANK, 0));
            _rankBeforeRankUp = _currentRank;

        }

        private void RestoreStars()
        {
            _currentStars = PlayerPrefs.GetInt(PLAYER_STARS, 0);
            _starsBeforeMissionStart = _currentStars;
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
            _currentRank = RankManagerInstance.RankUp(_currentRank.RankID);

            if (_starsToGain > 0)
            {
                IncreaseStars();
            }
        }

        private void SaveValues()
        {
            PlayerPrefs.SetInt(PLAYER_RANK, 0);
            //PlayerPrefs.SetInt(nameof(PLAYER_RANK), _currentRank.RankID);
            PlayerPrefs.SetInt(PLAYER_STARS, _currentStars);
        }

        private void OnApplicationQuit()
        {
            SaveValues();
        }
    }
}