using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CyberCruiser
{
    /// <summary>
    /// Mission manager is control of assigning new missions and tracking mission progress
    /// Mission manager also stores needed fields in player prefs
    /// 
    /// If the player does not currently have a mission a new mission is assigned when the mission screen is opened
    /// 
    /// When a mission is assigned the manager checks its parameters to find what events it should subscribe to for mission progress and completion
    /// the opposite is done when a mission is completed
    /// </summary>
    public class MissionManager : GameBehaviour<MissionManager>
    {
        #region References
        [SerializeField] private string fileName;

        [SerializeField] private MissionCategory[] _missionCategories;
        [SerializeField] private MissionCategory _currentMissionCategory;
        [SerializeField] private List<MissionScriptableObject> _missionsToCompleteInCategory = new();
        [SerializeField] private MissionScriptableObject _currentMission;
        #endregion

        #region PlayerPrefs Strings
        private const string CURRENT_MISSION_ID = "CurrentMissionID";
        private const string CURRENT_MISSION_CATEGORY_ID = "CurrentMissionCategoryID";
        private const string CURRENT_MISSION_PROGRESS = "CurrentMissionProgress";
        #endregion

        #region Fields
        [SerializeField] private int _currentMissionGoal;
        [SerializeField] private int _currentMissionProgress;
        [SerializeField] private int _currentMissionID;
        [SerializeField] private bool _isMissionFailed;
        [SerializeField] private bool _isAnyMissionCompleted;
        #endregion

        #region Properties
        public MissionScriptableObject CurrentMission { get => _currentMission; }

        public int CurrentMissionProgress
        {
            get => _currentMissionProgress;
            private set
            {
                _currentMissionProgress = value;
                CheckMissionProgress();
            }
        }

        public bool IsAnyMissionCompleted { get => _isAnyMissionCompleted; private set => _isAnyMissionCompleted = value; }
        #endregion

        #region Actions
        public static event Action<int> OnMissionComplete = null;
        #endregion

        #region Unity Methods
        protected override void Awake()
        {
            RestorePlayerData();
        }

        private void OnEnable()
        {
            GameManager.OnMissionStart += () => IsAnyMissionCompleted = false;
            GameManager.OnSaveDataCleared += ClearSaveData;
        }

        private void OnDisable()
        {

            GameManager.OnMissionStart -= () => IsAnyMissionCompleted = false;
            GameManager.OnSaveDataCleared -= ClearSaveData;
        }

        private void OnApplicationQuit()
        {
            StorePlayerData();
        }
        #endregion

        #region Mission Assigning
        public void SetMission(MissionScriptableObject missionToSet)
        {
            UnassignMission();
            _currentMission = missionToSet;
            SetMissionObjective();
        }

        private void UnassignMission()
        {
            if (_currentMission == null)
            {
                return;
            }

            ClearMissionObjective(_currentMission.missionCondition);
            ResetMissionProgress();
            _currentMission = null;
        }
        #endregion

        #region Mission Condition Assigning
        private void SetMissionObjective()
        {
            _currentMissionGoal = _currentMission.missionObjectiveAmount;
            CurrentMissionProgress = 0;
            _isMissionFailed = false;

            if (_currentMission.missionPersistence == MissionPersistence.OneRun)
            {
                GameManager.OnMissionStart += ResetMissionProgress;
            }

            AssignMissionCondition(_currentMission.missionCondition);
        }

        private void AssignMissionCondition(MissionConditions currentMissionCondition)
        {
            switch (currentMissionCondition)
            {
                case MissionConditions.EndMission:
                    GameManager.OnMissionEnd += IncrementMissionProgress;
                    break;

                case MissionConditions.CollectPlasma:
                    PlayerManager.OnPlasmaPickupValue += IncreaseMissionProgress;
                    break;

                case MissionConditions.UseShield:
                    PlayerShieldController.OnPlayerShieldsActivated += (i) => IncrementMissionProgress();
                    break;

                case MissionConditions.UseWeaponPack:
                    PlayerWeaponController.OnWeaponUpgradeStart += (i) => IncrementMissionProgress();
                    break;

                case MissionConditions.FlyDistance:
                    DistanceManager.OnDistanceTraveled += IncrementMissionProgress;
                    break;

                case MissionConditions.KillEnemy:
                    AssignEnemyToKill(_currentMission.enemy);
                    break;

                case MissionConditions.DontShootForDistance:
                    DistanceManager.OnDistanceTraveled += IncrementMissionProgress;
                    PlayerWeaponController.OnShoot += FailMission;
                    break;
            }
        }

        private void AssignEnemyToKill(EnemyTypes currentMissionEnemyType)
        {
            switch (currentMissionEnemyType)
            {
                case EnemyTypes.AllBasicEnemies:
                    Enemy.OnEnemyDeath += (enemyType) => IncrementMissionProgress();
                    break;

                case EnemyTypes.Gunship:
                case EnemyTypes.Mine:
                case EnemyTypes.Missile:
                case EnemyTypes.Blimp:
                case EnemyTypes.Slicer:
                case EnemyTypes.Dragon:
                    Enemy.OnEnemyDeath += CheckEnemyType;
                    break;

                case EnemyTypes.AllBosses:
                    Boss.OnBossDeath += (bossType) => IncrementMissionProgress();
                    break;

                case EnemyTypes.Robodactyl:
                case EnemyTypes.Behemoth:
                case EnemyTypes.Battlecruiser:
                case EnemyTypes.CyberKraken:
                    Boss.OnBossDeath += CheckBossType;
                    break;
            }
        }
        #endregion

        #region Mission Condition Clearing
        private void ClearEnemyToKill(EnemyTypes currentMissionEnemyType)
        {
            switch (currentMissionEnemyType)
            {
                case EnemyTypes.AllBasicEnemies:
                    Enemy.OnEnemyDeath -= (enemyType) => IncrementMissionProgress();
                    break;

                case EnemyTypes.Gunship:
                case EnemyTypes.Mine:
                case EnemyTypes.Missile:
                case EnemyTypes.Blimp:
                case EnemyTypes.Slicer:
                case EnemyTypes.Dragon:
                    Enemy.OnEnemyDeath -= CheckEnemyType;
                    break;

                case EnemyTypes.AllBosses:
                    Boss.OnBossDeath -= (bossType) => IncrementMissionProgress();
                    break;

                case EnemyTypes.Robodactyl:
                case EnemyTypes.Behemoth:
                case EnemyTypes.Battlecruiser:
                case EnemyTypes.CyberKraken:
                    Boss.OnBossDeath -= CheckBossType;
                    break;
            }
        }

        private void ClearMissionObjective(MissionConditions currentMissionCondition)
        {
            if (_currentMission.missionPersistence == MissionPersistence.OneRun)
            {
                GameManager.OnMissionStart -= ResetMissionProgress;
            }

            switch (currentMissionCondition)
            {
                case MissionConditions.EndMission:
                    GameManager.OnMissionEnd -= IncrementMissionProgress;
                    break;

                case MissionConditions.CollectPlasma:
                    PlayerManager.OnPlasmaPickupValue -= IncreaseMissionProgress;
                    break;

                case MissionConditions.UseShield:
                    PlayerShieldController.OnPlayerShieldsActivated -= (i) => IncrementMissionProgress();
                    break;

                case MissionConditions.UseWeaponPack:
                    PlayerWeaponController.OnWeaponUpgradeStart -= (i) => IncrementMissionProgress();
                    break;

                case MissionConditions.FlyDistance:
                    DistanceManager.OnDistanceTraveled -= IncrementMissionProgress;
                    break;

                case MissionConditions.KillEnemy:
                    ClearEnemyToKill(_currentMission.enemy);
                    break;

                case MissionConditions.DontShootForDistance:
                    DistanceManager.OnDistanceTraveled -= IncrementMissionProgress;
                    PlayerWeaponController.OnShoot -= FailMission;
                    break;
            }
        }
        #endregion

        #region Mission Progress 
        private void CheckEnemyType(EnemyTypes enemyType)
        {
            if (enemyType == _currentMission.enemy)
            {
                IncrementMissionProgress();
            }
        }

        private void CheckBossType(EnemyTypes bossType)
        {
            if (bossType == _currentMission.enemy)
            {
                IncrementMissionProgress();
            }
        }

        private void IncreaseMissionProgress(int value)
        {
            CurrentMissionProgress += value;
        }

        private void IncrementMissionProgress()
        {
            CurrentMissionProgress += 1;
        }

        private void CheckMissionProgress()
        {
            if (_currentMission == null)
            {
                return;
            }

            if (CurrentMissionProgress >= _currentMissionGoal)
            {
                if (_isMissionFailed)
                {
                    return;
                }

                CompleteMission();
            }
        }

        private void ResetMissionProgress()
        {
            _isMissionFailed = false;
            CurrentMissionProgress = 0;
        }
        #endregion

        #region EndMission
        private void CompleteMission()
        {
            Debug.Log("mission complete");
            IsAnyMissionCompleted = true;
            OnMissionComplete(_currentMission.missionStarReward);

            if (_missionsToCompleteInCategory.Contains(_currentMission))
            {
                _missionsToCompleteInCategory.Remove(_currentMission);
                if(_missionsToCompleteInCategory == null)
                {
                    GetNextMissionCategory();
                }
            }

            UnassignMission();
            _currentMissionID++;
        }

        private void FailMission()
        {
            _isMissionFailed = true;
        }
        #endregion

        #region Data Management
        private void StorePlayerData()
        {
            StoreMissionCategory();
            StoreMissionID();
            StoreMissionProgress();
            StoreMissionsToCompleteInCategoryToJSON();
        }

        private void StoreMissionCategory()
        {
            PlayerPrefs.SetInt(CURRENT_MISSION_CATEGORY_ID, _currentMissionCategory.ID);
        }

        private void StoreMissionID()
        {
            if (_currentMission != null)
            {
                PlayerPrefs.SetInt(CURRENT_MISSION_ID, _currentMissionID);
            }
        }

        private void StoreMissionProgress()
        {
            PlayerPrefs.SetInt(CURRENT_MISSION_PROGRESS, _currentMissionProgress);
        }

        private void StoreMissionsToCompleteInCategoryToJSON()
        {
            FileHandler.SaveToJSON(_missionsToCompleteInCategory, fileName);
        }

        private void RestorePlayerData()
        {
            RestoreMissionCategory();
            RestoreMissionID();
            RestoreMissionProgress();
            RestoreMissionsToCompleteInCategoryFromJSON();
        }

        private void RestoreMissionCategory()
        {
            _currentMissionCategory = _missionCategories[PlayerPrefs.GetInt(CURRENT_MISSION_CATEGORY_ID, 0)];
        }

        private void RestoreMissionID()
        {
            _currentMissionID = PlayerPrefs.GetInt(CURRENT_MISSION_ID, 0);
        }

        private void RestoreMissionProgress()
        {
            _currentMissionProgress = PlayerPrefs.GetInt(CURRENT_MISSION_PROGRESS, 0);
        }

        private void RestoreMissionsToCompleteInCategoryFromJSON()
        {
            _missionsToCompleteInCategory = FileHandler.ReadFromJSON<MissionScriptableObject>(fileName);

            if(_missionsToCompleteInCategory.Count == 0)
            {
                Debug.Log("Missions left to complete is empty, repopulating list.");
                for (int i = 0; i < _currentMissionCategory.Missions.Length; i++)
                {
                    _missionsToCompleteInCategory.Add(_currentMissionCategory.Missions[i]);
                }
            }
        }

        private void ClearSaveData()
        {
            _currentMission = null;
            _currentMissionID = 0;
            _currentMissionProgress = 0;
        }
        #endregion

        private void GetNextMissionCategory()
        {
            _currentMissionCategory = _missionCategories[_currentMissionCategory.ID + 1];

            _missionsToCompleteInCategory.Clear();
            for (int i = 0; i < _currentMissionCategory.Missions.Length; i++)
            {
                _missionsToCompleteInCategory.Add(_currentMissionCategory.Missions[i]);
            }
        }

        //choose next mission when requested
        public void ChooseNextMission()
        {
            //if player is in tutorial missions assign next mission directly, otherwise select random mission
            if (_currentMissionCategory.ID == 0)
            {
                SetMission(_missionsToCompleteInCategory[0]);
            }
            else
            {
                PickRandomUncompletedMissionInCategory();
            }
        }

        //pick random mission in category
        private void PickRandomUncompletedMissionInCategory()
        {
            MissionScriptableObject randomMission = _missionsToCompleteInCategory[Random.Range(0, _missionsToCompleteInCategory.Count)];
            SetMission(randomMission);
        }

        [Serializable]
        private struct MissionCategory
        {
            public string Name;
            public int ID;
            public MissionScriptableObject[] Missions;
        }
    }
}