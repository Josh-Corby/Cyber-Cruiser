using System;
using TMPro;
using UnityEngine;

namespace CyberCruiser
{
    public class PlayerStatistics : MonoBehaviour
    {
        [SerializeField] private DistanceManager _distanceManager;

        #region Player Prefs Strings
        private const string LONGEST_RUN = "LongestRun";
        private const string LAST_RUN = "LastRun";
        private const string TOTAL_RUNS = "TotalRuns";
        private const string TOTAL_DISTANCE_FLOWN = "TotalDistanceFlown";
        private const string MISSIONS_COMPLETED = "MissionsCompleted";
        private const string PLASMA_COLLECTED = "PlasmaCollected";
        private const string PLASMA_SPENT = "PlasmaSpent";

        #region Enemy Kills
        private const string ENEMIES_KILLED = "EnemiesKilled";
        private const string BOSSES_KILLED = "BossesKilled";
        private const string BLIMPS_KILLED = "BlimpsKilled";
        private const string GUNSHIPS_KILLED = "GunshipsKilled";
        private const string MINES_KILLED = "MinesKilled";
        private const string SLICERS_KILLED = "SlicersKilled";
        private const string DRAGONS_KILLED = "DragonsKilled";
        private const string MISSILES_KILLED = "MissilesKilled";
        private const string BATTLECRUISERS_KILLED = "BattlecruisersKilled";
        private const string BEHEMOTHS_KILLED = "BehemothsKilled";
        private const string ROBODACTYLS_KILLED = "RobodactylsKilled";
        private const string CYBER_KRAKENS_KILLED = "CyberKrakensKilled";
        #endregion

        #region Player Kills Per Enemy
        private const string BLIMP_PLAYER_KILLS = "BlimpKills";
        private const string GUNSHIP_PLAYER_KILLS = "GunshipKills";
        private const string MINE_PLAYER_KILLS = "MineKills";
        private const string SLICER_PLAYER_KILLS = "SlicerKills";
        private const string DRAGON_PLAYER_KILLS = "DragonKills";
        private const string MISSILE_PLAYER_KILLS = "MissileKills";
        private const string BATTLECRUISER_PLAYER_KILLS = "BattlecruiserKills";
        private const string BEHEMOTH_PLAYER_KILLS = "BehemothKills";
        private const string ROBODACTYL_PLAYER_KILLS = "RobodactylKills";
        private const string CYBER_KRAKEN_PLAYER_KILLS = "CyberKrakenKills";
        #endregion

        #region Weapon Equips
        private const string SCATTER_FIXED_EQUIPS = "ScatterFixedEquips";
        private const string SCATTER_RANDOM_EQUIPS = "ScatterRandomEquips";
        private const string SMART_GUN_EQUIPS = "SmartGunEquips";
        private const string CHAIN_LIGHTNING_EQUIPS = "ChainLightingEquips";
        private const string PULVERIZER_EQUIPS = "PulverizerEquips";
        private const string BFG_EQUIPS = "BFGEquips";
        #endregion
        #endregion







        #region Ints
        private int _longestRun;
        private int _lastRun;
        private int _totalRuns;
        private int _totalDistanceFlown;
        private int _missionsCompleted;
        private int _plasmaCollected;
        private int _plasmaSpent;
        private int _enemiesKilled;
        private int _bossesKilled;
        #endregion

        #region Enemy Data References
        [Header("Enemy Data Rederences")]
        [SerializeField] private EnemyScriptableObject _blimpData;
        [SerializeField] private EnemyScriptableObject _gunshipData;
        [SerializeField] private EnemyScriptableObject _mineData;
        [SerializeField] private EnemyScriptableObject _slicerData;
        [SerializeField] private EnemyScriptableObject _dragonData;
        [SerializeField] private EnemyScriptableObject _missileData;
        [SerializeField] private EnemyScriptableObject _battlecruiserData;
        [SerializeField] private EnemyScriptableObject _behemothData;
        [SerializeField] private EnemyScriptableObject _robodactylData;
        [SerializeField] private EnemyScriptableObject _cyberKrakenData;
        #endregion

        #region Weapon Data References
        [SerializeField] private WeaponSO _scatterFixedData;
        [SerializeField] private WeaponSO _scatterRandomData;
        [SerializeField] private WeaponSO _smartGunData;
        [SerializeField] private WeaponSO _chainLightningData;
        [SerializeField] private WeaponSO _pulverizerData;
        [SerializeField] private WeaponSO _bfgData;
        #endregion

        #region Text References
        [Header("Text References")]
        [SerializeField] private TMP_Text _longestRunText;
        [SerializeField] private TMP_Text _lastRunText;
        [SerializeField] private TMP_Text _totalRunsText;
        [SerializeField] private TMP_Text _totalDistanceFlownText;
        [SerializeField] private TMP_Text _missionsCompletedText;
        [SerializeField] private TMP_Text _plasmaCollectedText;
        [SerializeField] private TMP_Text _plasmaSpentText;
        [SerializeField] private TMP_Text _enemiesKilledText;
        [SerializeField] private TMP_Text _bossesKilledText;
        #endregion

        private void Awake()
        {
            LoadStatistics();
        }

        private void OnEnable()
        {
            Enemy.OnEnemyDied += (enemyObject, enemyType) => { _enemiesKilled++; };
            GameManager.OnMissionEnd += GetEndOfMissionStats;
            MissionManager.OnMissionComplete += (i) => { _missionsCompleted++; };
            PlayerManager.OnPlasmaPickupValue += (plasmaCollected) => { _plasmaCollected += plasmaCollected; };
            PlayerManager.OnPlasmaSpent += (plasmaSpent) => { _plasmaSpent += plasmaSpent; };
        }

        private void OnDisable()
        {
            Enemy.OnEnemyDied -= (enemyObject, enemyType) => { _enemiesKilled++; };
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

            PlayerPrefs.SetInt(BLIMPS_KILLED, _blimpData.KillData.TimesKilled);
            PlayerPrefs.SetInt(GUNSHIPS_KILLED, _gunshipData.KillData.TimesKilled);
            PlayerPrefs.SetInt(MINES_KILLED, _mineData.KillData.TimesKilled);
            PlayerPrefs.SetInt(SLICERS_KILLED, _slicerData.KillData.TimesKilled);
            PlayerPrefs.SetInt(DRAGONS_KILLED, _dragonData.KillData.TimesKilled);
            PlayerPrefs.SetInt(MISSILES_KILLED, _missileData.KillData.TimesKilled);
            PlayerPrefs.SetInt(BATTLECRUISERS_KILLED, _battlecruiserData.KillData.TimesKilled);
            PlayerPrefs.SetInt(BEHEMOTHS_KILLED, _behemothData.KillData.TimesKilled);
            PlayerPrefs.SetInt(ROBODACTYLS_KILLED, _robodactylData.KillData.TimesKilled);
            PlayerPrefs.SetInt(CYBER_KRAKENS_KILLED, _cyberKrakenData.KillData.TimesKilled);

            PlayerPrefs.SetInt(BLIMP_PLAYER_KILLS, _blimpData.KillData.PlayerKills);
            PlayerPrefs.SetInt(GUNSHIP_PLAYER_KILLS, _gunshipData.KillData.PlayerKills);
            PlayerPrefs.SetInt(MINE_PLAYER_KILLS, _mineData.KillData.PlayerKills);
            PlayerPrefs.SetInt(SLICER_PLAYER_KILLS, _slicerData.KillData.PlayerKills);
            PlayerPrefs.SetInt(DRAGON_PLAYER_KILLS, _dragonData.KillData.PlayerKills);
            PlayerPrefs.SetInt(MISSILE_PLAYER_KILLS, _missileData.KillData.PlayerKills);
            PlayerPrefs.SetInt(BATTLECRUISER_PLAYER_KILLS, _battlecruiserData.KillData.PlayerKills);
            PlayerPrefs.SetInt(BEHEMOTH_PLAYER_KILLS, _behemothData.KillData.PlayerKills);
            PlayerPrefs.SetInt(ROBODACTYL_PLAYER_KILLS, _robodactylData.KillData.PlayerKills);
            PlayerPrefs.SetInt(CYBER_KRAKEN_PLAYER_KILLS, _cyberKrakenData.KillData.PlayerKills);

            PlayerPrefs.SetInt(SCATTER_FIXED_EQUIPS, _scatterFixedData.Equips);
            PlayerPrefs.SetInt(SCATTER_RANDOM_EQUIPS, _scatterRandomData.Equips);
            PlayerPrefs.SetInt(SMART_GUN_EQUIPS, _smartGunData.Equips);
            PlayerPrefs.SetInt(CHAIN_LIGHTNING_EQUIPS, _chainLightningData.Equips);
            PlayerPrefs.SetInt(PULVERIZER_EQUIPS, _pulverizerData.Equips);
            PlayerPrefs.SetInt(BFG_EQUIPS, _bfgData.Equips);
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

            _blimpData.KillData.TimesKilled = PlayerPrefs.GetInt(BLIMPS_KILLED, 0);
            _gunshipData.KillData.TimesKilled = PlayerPrefs.GetInt(GUNSHIPS_KILLED, 0);
            _mineData.KillData.TimesKilled = PlayerPrefs.GetInt(MINES_KILLED, 0);
            _slicerData.KillData.TimesKilled = PlayerPrefs.GetInt(SLICERS_KILLED, 0);
            _dragonData.KillData.TimesKilled = PlayerPrefs.GetInt(DRAGONS_KILLED, 0);
            _missileData.KillData.TimesKilled = PlayerPrefs.GetInt(MISSILES_KILLED, 0);
            _battlecruiserData.KillData.TimesKilled = PlayerPrefs.GetInt(BATTLECRUISERS_KILLED, 0);
            _behemothData.KillData.TimesKilled = PlayerPrefs.GetInt(BEHEMOTHS_KILLED, 0);
            _robodactylData.KillData.TimesKilled = PlayerPrefs.GetInt(ROBODACTYLS_KILLED, 0);
            _cyberKrakenData.KillData.TimesKilled = PlayerPrefs.GetInt(CYBER_KRAKENS_KILLED, 0);

            _blimpData.KillData.PlayerKills = PlayerPrefs.GetInt(BLIMP_PLAYER_KILLS, 0);
            _gunshipData.KillData.PlayerKills = PlayerPrefs.GetInt(GUNSHIP_PLAYER_KILLS, 0);
            _mineData.KillData.PlayerKills = PlayerPrefs.GetInt(MINE_PLAYER_KILLS, 0);
            _slicerData.KillData.PlayerKills = PlayerPrefs.GetInt(SLICER_PLAYER_KILLS, 0);
            _dragonData.KillData.PlayerKills = PlayerPrefs.GetInt(DRAGON_PLAYER_KILLS, 0);
            _missileData.KillData.PlayerKills = PlayerPrefs.GetInt(MISSILE_PLAYER_KILLS, 0);
            _battlecruiserData.KillData.PlayerKills = PlayerPrefs.GetInt(BATTLECRUISER_PLAYER_KILLS, 0);
            _behemothData.KillData.PlayerKills = PlayerPrefs.GetInt(BEHEMOTH_PLAYER_KILLS, 0);
            _robodactylData.KillData.PlayerKills = PlayerPrefs.GetInt(ROBODACTYL_PLAYER_KILLS, 0);
            _cyberKrakenData.KillData.PlayerKills = PlayerPrefs.GetInt(CYBER_KRAKEN_PLAYER_KILLS, 0);

            _scatterFixedData.Equips = PlayerPrefs.GetInt(SCATTER_FIXED_EQUIPS, 0);
            _scatterRandomData.Equips = PlayerPrefs.GetInt(SCATTER_RANDOM_EQUIPS, 0);
            _smartGunData.Equips = PlayerPrefs.GetInt(SMART_GUN_EQUIPS, 0);
            _chainLightningData.Equips = PlayerPrefs.GetInt(CHAIN_LIGHTNING_EQUIPS, 0);
            _pulverizerData.Equips = PlayerPrefs.GetInt(PULVERIZER_EQUIPS, 0);
            _bfgData.Equips = PlayerPrefs.GetInt(BFG_EQUIPS, 0);
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
            _totalDistanceFlown += currentRunDistance;
            _missionsCompleted += 1;

            if (currentRunDistance > _longestRun)
            {
                _longestRun = currentRunDistance;
            }

            SetTexts();
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
