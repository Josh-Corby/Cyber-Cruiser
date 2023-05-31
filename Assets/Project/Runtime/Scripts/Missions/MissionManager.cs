using System;
using UnityEngine;

namespace CyberCruiser
{
    public class MissionManager : GameBehaviour<MissionManager>
    {
        #region References
        [SerializeField] private MissionScriptableObject[] _tutorialMissions;

        [SerializeField] private MissionScriptableObject[] _1StarMissions;

        [SerializeField] private MissionScriptableObject[] _2StarMissions;

        [SerializeField] private MissionScriptableObject[] _3StarMissions;
        #endregion

        private const string CURRENT_MISSION_ID = "CurrentMissionID";
        #region Fields
        [SerializeField] private MissionScriptableObject _currentMission;
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

        public static event Action<int> OnMissionComplete = null;

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

        private void Start()
        {
            _currentMissionGoal = 10000;
            SetMission();
        }

        public void SetMission()
        {
            if (_currentMission != null) return;

            UnassignMission();
            _currentMission = _tutorialMissions[_currentMissionID];
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

        private void ClearSaveData()
        {
            _currentMission = null;
            _currentMissionID = 0;
            _currentMissionProgress = 0;
        }

        private void OnApplicationQuit()
        {
            StoreMissionID();
        }
    }
}