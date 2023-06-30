using System;
using System.Collections.Generic;
using System.Linq;
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
    public class MissionManager : GameBehaviour
    {
        #region References
        [SerializeField] private string fileName;

        [SerializeField] private MissionCategory[] _missionCategories;
        [SerializeField] private MissionCategory _currentMissionCategory;
        [SerializeField] private List<MissionScriptableObject> _missionsToCompleteInCategory = new();
        [SerializeField] private MissionScriptableObject _currentMission;
        [SerializeField] private MissionScriptableObject _nextMissionToStart;
        #endregion

        #region PlayerPrefs Strings
        private const string CATEGORY_ID_OF_CURRENT_MISSION = "CurrentMissionID";
        private const string CURRENT_MISSION_CATEGORY_ID = "CurrentMissionCategoryID";
        private const string CURRENT_MISSION_PROGRESS = "CurrentMissionProgress";
        #endregion

        #region Fields
        [SerializeField] private int _currentMissionGoal;
        [SerializeField] private int _currentMissionProgress;
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

        public int MissionProgressLeft
        {
            get
            {
                return _currentMissionGoal - _currentMissionProgress;
            }
        }

        public bool IsAnyMissionCompleted { get => _isAnyMissionCompleted; private set => _isAnyMissionCompleted = value; }
        #endregion

        #region Actions
        public static event Action<int> OnMissionComplete = null;
        #endregion

        #region Unity Methods
        //protected void Awake()
        //{
        //    RestorePlayerData();
        //}

        private void OnEnable()
        {
            GameManager.OnMissionStart += () => IsAnyMissionCompleted = false;
            GameManager.OnMissionEnd += StartNextMissionIfReady;
        }

        private void OnDisable()
        {

            GameManager.OnMissionStart -= () => IsAnyMissionCompleted = false;
            GameManager.OnMissionEnd += StartNextMissionIfReady;
        }

        private void Start()
        {
            RestorePlayerData();
            StartNextMissionIfReady();
        }

        private void OnApplicationQuit()
        {
            StorePlayerData();
        }
        #endregion

        #region Mission Assigning
        public void StartNextMissionIfReady()
        {
            if (_currentMission != null)
            {
                return;
            }

            _currentMission = _nextMissionToStart;
            _nextMissionToStart = null;
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

        private void GetNextMissionCategory()
        {
            _currentMissionCategory = _missionCategories[_currentMissionCategory.ID + 1];

            _missionsToCompleteInCategory.Clear();
            FillMissionsToComplete();
        }

        //choose next mission when requested
        //public void ChooseNextMission()
        //{
        //    //if player is in tutorial missions assign next mission directly, otherwise select random mission
        //    if (_currentMissionCategory.ID == 0)
        //    {
        //        SetMission(_missionsToCompleteInCategory[0]);
        //    }
        //    else
        //    {
        //        PickRandomUncompletedMissionInCategory();
        //    }
        //}

        private void ChooseNextMission()
        {
            if (_currentMissionCategory.ID == 0)
            {
                _nextMissionToStart = _missionsToCompleteInCategory[0];
            }
            else
            {
                PickRandomUncompletedMissionInCategory();
            }
        }

        //pick random mission in category
        private void PickRandomUncompletedMissionInCategory()
        {
            _nextMissionToStart = _missionsToCompleteInCategory[Random.Range(0, _missionsToCompleteInCategory.Count)];
        }
        #endregion

        

        #region Mission Condition Assigning
        private void SetMissionObjective()
        {
            _currentMissionGoal = _currentMission.missionObjectiveAmount;
            //CurrentMissionProgress = 0;
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
                    PlayerShieldController.OnPlayerShieldsActivated += IncrementMissionProgress;
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
                    Boss.OnBossTypeDied += (bossType) => IncrementMissionProgress();
                    break;

                case EnemyTypes.Robodactyl:
                case EnemyTypes.Behemoth:
                case EnemyTypes.Battlecruiser:
                case EnemyTypes.CyberKraken:
                    Boss.OnBossTypeDied += CheckBossType;
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
                    Boss.OnBossTypeDied -= (bossType) => IncrementMissionProgress();
                    break;

                case EnemyTypes.Robodactyl:
                case EnemyTypes.Behemoth:
                case EnemyTypes.Battlecruiser:
                case EnemyTypes.CyberKraken:
                    Boss.OnBossTypeDied -= CheckBossType;
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
                    PlayerShieldController.OnPlayerShieldsActivated -= IncrementMissionProgress;
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
            CurrentMissionProgress = 0;
            IsAnyMissionCompleted = true;
            OnMissionComplete(_currentMission.missionStarReward);

            if (_missionsToCompleteInCategory.Contains(_currentMission))
            {
                _missionsToCompleteInCategory.Remove(_currentMission);
                if (_missionsToCompleteInCategory == null)
                {
                    GetNextMissionCategory();
                }
            }

            UnassignMission();

            ChooseNextMission();
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
            StoreCategoryIDOfCurrentMission();
            StoreMissionProgress();
            StoreMissionsToCompleteInCategoryToJSON();
        }

        private void StoreMissionCategory()
        {
            PlayerPrefs.SetInt(CURRENT_MISSION_CATEGORY_ID, _currentMissionCategory.ID);
        }

        private void StoreCategoryIDOfCurrentMission()
        {
            int missionIndexInMissionsToComplete;

            if (_currentMission != null)
            {
                missionIndexInMissionsToComplete = _missionsToCompleteInCategory.ToList().FindIndex(obj => obj == _currentMission);
            }

            else if (_nextMissionToStart != null)
            {
                missionIndexInMissionsToComplete = _missionsToCompleteInCategory.ToList().FindIndex(obj => obj == _nextMissionToStart);
            }

            else
            {
                missionIndexInMissionsToComplete = 0;
            }
            
            PlayerPrefs.SetInt(CATEGORY_ID_OF_CURRENT_MISSION, missionIndexInMissionsToComplete);
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
            RestoreMissionsToCompleteInCategoryFromJSON();
            RestoreCategoryIDOfCurrentMission();
            RestoreMissionProgress();
        }

        private void RestoreMissionCategory()
        {
            _currentMissionCategory = _missionCategories[PlayerPrefs.GetInt(CURRENT_MISSION_CATEGORY_ID, 0)];
        }

        private void RestoreCategoryIDOfCurrentMission()
        {
            int categoryIDOfCurrentMission = PlayerPrefs.GetInt(CATEGORY_ID_OF_CURRENT_MISSION, 0);
            Debug.Log(categoryIDOfCurrentMission);
            _nextMissionToStart = _missionsToCompleteInCategory[categoryIDOfCurrentMission];
        }

        private void RestoreMissionProgress()
        {
            _currentMissionProgress = PlayerPrefs.GetInt(CURRENT_MISSION_PROGRESS, 0);
        }

        private void RestoreMissionsToCompleteInCategoryFromJSON()
        {
            _missionsToCompleteInCategory = FileHandler.ReadFromJSON<MissionScriptableObject>(fileName);

            if (_missionsToCompleteInCategory.Count == 0)
            {
                Debug.Log("Missions left to complete is empty, repopulating list.");
                FillMissionsToComplete();
            }
        }

        private void FillMissionsToComplete()
        {
            for (int i = 0; i < _currentMissionCategory.Missions.Length; i++)
            {
                _missionsToCompleteInCategory.Add(_currentMissionCategory.Missions[i]);
            }
        }

        public void ClearSaveData()
        {
            _currentMission = null;
            _currentMissionProgress = 0;
            _currentMissionCategory = _missionCategories[0];
            _missionsToCompleteInCategory.Clear();
            FillMissionsToComplete();
            _nextMissionToStart = _currentMissionCategory.Missions[0];
            StartNextMissionIfReady();
        }
        #endregion  

        [Serializable]
        private struct MissionCategory
        {
            public string Name;
            public int ID;
            public MissionScriptableObject[] Missions;
        }
    }
}