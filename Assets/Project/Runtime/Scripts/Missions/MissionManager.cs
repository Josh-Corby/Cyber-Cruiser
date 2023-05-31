using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CyberCruiser
{
    public class MissionManager : GameBehaviour<MissionManager>
    {
        #region References
        [SerializeField] private MissionCategory[] _missionCategories;
        [SerializeField] private List<MissionScriptableObject> _uncompletedMissionsInCategory = new();
        [SerializeField] private MissionScriptableObject _currentMission;
        private MissionCategory _currentMissionCategory;
        #endregion

        #region PlayerPrefs Strings
        private const string CURRENT_MISSION_ID = "CurrentMissionID";
        private const string CURRENT_MISSION_CATEGORY_ID = "CurrentMissionCategoryID";
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
        private void OnEnable()
        {
            RestoreMissionID();
            GameManager.OnMissionStart += () => IsAnyMissionCompleted = false;
            GameManager.OnSaveDataCleared += ClearSaveData;
        }

        private void OnDisable()
        {
            StoreMissionID();
            GameManager.OnMissionStart -= () => IsAnyMissionCompleted = false;
            GameManager.OnSaveDataCleared -= ClearSaveData;
        }

        private void Start()
        {
            _currentMissionGoal = 10000;
            SetMission();
        }

        private void OnApplicationQuit()
        {
            StoreMissionID();
        }
        #endregion

        #region Mission Assigning
        public void SetMission()
        {
            if (_currentMission != null) return;

            UnassignMission();
            //_currentMission = _tutorialMissions[_currentMissionID];
            SetMissionObjective();
        }

        private void SetMissionObjective()
        {
            _currentMissionGoal = _currentMission.missionObjectiveAmount;
            CurrentMissionProgress = 0;
            _isMissionFailed = false;

            if(_currentMission.missionPersistence == MissionPersistence.OneRun)
            {
                GameManager.OnMissionStart += ResetMissionProgress;
            }

            CheckMissionCondition(_currentMission.missionCondition);
        }

        private void CheckMissionCondition(MissionConditions currentMissionCondition)
        {
            switch(currentMissionCondition)
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
                     CheckEnemyToKill(_currentMission.enemy);
                    break;

                case MissionConditions.DontShootForDistance:
                    DistanceManager.OnDistanceTraveled += IncrementMissionProgress;
                    PlayerWeaponController.OnShoot += FailMission;
                    break;
            }
        }

        private void CheckEnemyToKill(EnemyTypes currentMissionEnemyType)
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

        private void UncheckEnemyToKill(EnemyTypes currentMissionEnemyType)
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
       
        private void UnassignMission()
        {
            if (_currentMission == null) return;
            UnassignMissionObjective(_currentMission.missionCondition);
            ResetMissionProgress();
            _currentMission = null;
        }
        
        private void UnassignMissionObjective(MissionConditions currentMissionCondition)
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
                    UncheckEnemyToKill(_currentMission.enemy);
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
            if(enemyType == _currentMission.enemy)
            {
                IncrementMissionProgress();
            }
        }

        private void CheckBossType(EnemyTypes bossType)
        {
            if(bossType == _currentMission.enemy)
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
            if (_currentMission == null) return;

            if (CurrentMissionProgress >= _currentMissionGoal)
            {
                if (_isMissionFailed) return;
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
            UnassignMission();
            _currentMissionID++;
        }

        private void FailMission()
        {
            _isMissionFailed = true;
        }
        #endregion

        #region Data Management
        private void RestoreMissionCategory()
        {
            _currentMissionCategory = _missionCategories[PlayerPrefs.GetInt(CURRENT_MISSION_CATEGORY_ID, 0)];
        }

        private void StoreMissionCategory()
        {
            PlayerPrefs.SetInt(CURRENT_MISSION_CATEGORY_ID, _currentMissionCategory.ID);
        }

        private void RestoreMissionID()
        {
            _currentMissionID = PlayerPrefs.GetInt(CURRENT_MISSION_ID, 0);
        }

        private void StoreMissionID()
        {
            if(_currentMission != null)
            {
                PlayerPrefs.SetInt(CURRENT_MISSION_ID, _currentMissionID);
            }
        }

        private void ClearSaveData()
        {
            _currentMission = null;
            _currentMissionID = 0;
            _currentMissionProgress = 0;
        }
        #endregion


        //pick random mission in category
        private void PickRandomUncompletedMissionInCategory()
        {
            MissionScriptableObject randomMission = _uncompletedMissionsInCategory[Random.Range(0, _uncompletedMissionsInCategory.Count)];
            _currentMission = randomMission;
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