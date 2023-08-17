using TMPro;
using UnityEngine;

namespace CyberCruiser
{
    public class PlayerStatistics : MonoBehaviour
    {
        [SerializeField] private DistanceManager _distanceManager;

        private const string LONGEST_RUN = "LongestRun";
        private const string LAST_RUN = "LastRun";
        private const string TOTAL_RUNS = "TotalRuns";
        private const string TOTAL_DISTANCE_FLOWN = "TotalDistanceFlown";
        private const string MISSIONS_COMPLETED = "MissionsCompleted";
        private const string PLASMA_COLLECTED = "PlasmaCollected";
        private const string PLASMA_SPENT = "PlasmaSpent";
        private const string ENEMIES_KILLED = "EnemiesKilled";
        private const string BOSSES_KILLED = "BossesKilled";

        private int _longestRun;
        private int _lastRun;
        private int _totalRuns;
        private int _totalDistanceFlown;
        private int _missionsCompleted;
        private int _plasmaCollected;
        private int _plasmaSpent;
        private int _enemiesKilled;
        private int _bossesKilled;

        [SerializeField] private TMP_Text _longestRunText;
        [SerializeField] private TMP_Text _lastRunText;
        [SerializeField] private TMP_Text _totalRunsText;
        [SerializeField] private TMP_Text _totalDistanceFlownText;
        [SerializeField] private TMP_Text _missionsCompletedText;
        [SerializeField] private TMP_Text _plasmaCollectedText;
        [SerializeField] private TMP_Text _plasmaSpentText;
        [SerializeField] private TMP_Text _enemiesKilledText;
        [SerializeField] private TMP_Text _bossesKilledText;

        private void Awake()
        {
            LoadStatistics();
        }

        private void OnEnable()
        {
            Boss.OnBossTypeDied += (bossType) => {; };
            Enemy.OnEnemyDied += (enemyObject, enemyType) => { _enemiesKilled += 1; };
            GameManager.OnMissionEnd += GetEndOfMissionStats;
            MissionManager.OnMissionComplete += (i) => { _missionsCompleted++; };
            PlayerManager.OnPlasmaPickupValue += (plasmaCollected) => { _plasmaCollected += plasmaCollected; };
            PlayerManager.OnPlasmaSpent += (plasmaSpent) => { _plasmaSpent += plasmaSpent; };
            
        }

        private void OnDisable()
        {
            Boss.OnBossTypeDied -= (bossType) => {; };
            Enemy.OnEnemyDied -= (enemyObject, enemyType) => { _enemiesKilled += 1; };
            GameManager.OnMissionEnd -= GetEndOfMissionStats;
            MissionManager.OnMissionComplete -= (i) => { _missionsCompleted++; };
            PlayerManager.OnPlasmaPickupValue -= (plasmaCollected) => { _plasmaCollected += plasmaCollected; };
            PlayerManager.OnPlasmaSpent -= (plasmaSpent) => { _plasmaSpent += plasmaSpent; };
        }

        private void OnApplicationQuit()
        {
            SaveStatistics();
        }

        private void SaveStatistics()
        {
            PlayerPrefs.SetInt(LONGEST_RUN, _longestRun);
            PlayerPrefs.SetInt(LAST_RUN, _lastRun);
            PlayerPrefs.SetInt(TOTAL_RUNS, _totalRuns);
            PlayerPrefs.SetInt(TOTAL_DISTANCE_FLOWN, _totalDistanceFlown);
            PlayerPrefs.SetInt(MISSIONS_COMPLETED, _missionsCompleted);
            PlayerPrefs.SetInt(PLASMA_COLLECTED, _plasmaCollected);
            PlayerPrefs.SetInt(PLASMA_SPENT, _plasmaSpent);
            PlayerPrefs.SetInt(ENEMIES_KILLED, _enemiesKilled);
            PlayerPrefs.SetInt(BOSSES_KILLED, _bossesKilled);
        }

        private void LoadStatistics()
        {
            _longestRun = PlayerPrefs.GetInt(LONGEST_RUN, 0);
            _lastRun = PlayerPrefs.GetInt(LAST_RUN, 0);
            _totalRuns = PlayerPrefs.GetInt(TOTAL_RUNS, 0);
            _totalDistanceFlown = PlayerPrefs.GetInt(TOTAL_DISTANCE_FLOWN, 0);
            _missionsCompleted = PlayerPrefs.GetInt(MISSIONS_COMPLETED, 0);
            _plasmaCollected = PlayerPrefs.GetInt(PLASMA_COLLECTED, 0);
            _plasmaSpent = PlayerPrefs.GetInt(PLASMA_SPENT, 0);
            _enemiesKilled = PlayerPrefs.GetInt(ENEMIES_KILLED, 0);
            _bossesKilled = PlayerPrefs.GetInt(BOSSES_KILLED, 0);
            SetTexts();
        }

        public void ClearSaveData()
        {
            _longestRun = 0;
            _lastRun = 0;
            _totalRuns = 0;
            _totalDistanceFlown = 0;
            _missionsCompleted = 0;
            _plasmaCollected = 0;
            _plasmaSpent = 0;
            _enemiesKilled = 0;
            _bossesKilled = 0;
            SetTexts();
        }

        private void GetEndOfMissionStats()
        {
            int currentRunDistance = _distanceManager.DistanceInt;

            _lastRun = currentRunDistance;
            _totalDistanceFlown = _totalDistanceFlown + currentRunDistance;
            _missionsCompleted += 1;

            if (currentRunDistance > _longestRun)
            {
                _longestRun = currentRunDistance;
            }
        }

        private void SetTexts()
        {
            _longestRunText.text = _longestRun.ToString();
            _lastRunText.text = _lastRun.ToString();
            _totalRunsText.text = _totalRuns.ToString();
            _totalDistanceFlownText.text = _totalDistanceFlown.ToString();
            _missionsCompletedText.text = _missionsCompleted.ToString();
            _plasmaCollectedText.text = _plasmaCollected.ToString();
            _plasmaSpentText.text = _plasmaSpent.ToString();
            _enemiesKilledText.text = _enemiesKilled.ToString();
            _bossesKilledText.text = _bossesKilled.ToString();
        }
    }
}
