using System;
using UnityEngine;

public class MissionManager : GameBehaviour<MissionManager>
{
    #region References
    [SerializeField] private MissionScriptableObject[] _missions;
    #endregion

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
        GameManager.OnMissionStart += () => IsAnyMissionCompleted = false;
        UIManager.OnMissionsPanelLoaded += SetMission;
    }

    private void OnDisable()
    {
        GameManager.OnMissionStart -= () => IsAnyMissionCompleted = false;
        UIManager.OnMissionsPanelLoaded -= SetMission;
    }

    private void Start()
    {
        _currentMissionID = 0;
        _currentMissionGoal = 10000;
        SetMission();
    }

    public void SetMission()
    {
        if (_currentMission != null) return;

        UnassignMission();
        _currentMission = _missions[_currentMissionID];
        SetMissionObjective();
    }

    private void SetMissionObjective()
    {
        _currentMissionGoal = _currentMission.missionObjectiveAmount;
        CurrentMissionProgress = 0;
        _isMissionFailed = false;

        switch (_currentMission.missionCondition)
        {
            #region General
            case MissionConditions.EndMission:
                GameManager.OnMissionEnd += IncrementMissionProgress;
                return;
            #endregion

            #region Travel Distance
            case MissionConditions.FlyDistanceOnce:
                DistanceManager.OnDistanceChanged += SetMissionProgress;
                return;
            case MissionConditions.FlyDistanceTotal:
                DistanceManager.OnDistanceTraveled += IncrementMissionProgress;
                return;
            #endregion

            #region Pickups
            case MissionConditions.CollectPlasma:
                PlayerManager.OnPlasmaPickupValue += IncreaseMissionProgress;
                return;
            case MissionConditions.UseWeaponPack:
                PlayerWeaponController.OnWeaponUpgradeStart += (i) => IncrementMissionProgress();
                return;
            #endregion

            #region Kill Enemies

            #region GeneralEnemies
            #endregion

            #region Enemy Specific
            #endregion

            #region GeneralBosses
            case MissionConditions.KillBoss:
                switch (_currentMission.boss)
                {
                    case BossTypes.All:
                        Boss.OnBossDied += (pickupType, position) => IncrementMissionProgress();
                        return;
                    case BossTypes.Battlecruiser:
                        return;
                    case BossTypes.Robodactyl:
                        Robodactyl.OnDied += IncrementMissionProgress;
                        return;
                    case BossTypes.Behemoth:
                        Behemoth.OnDied += IncrementMissionProgress;
                        return;
                }
                break;
            #endregion
            #endregion

            #region Pacifist
            case MissionConditions.DontShootForDistance:
                GameManager.OnMissionEnd += ResetMissionProgress;
                DistanceManager.OnDistanceTraveled += IncrementMissionProgress;
                PlayerWeaponController.OnShoot += FailMission;
                return;
            #endregion

            #region Shield
            case MissionConditions.UseShield:
                PlayerShieldController.OnPlayerShieldsActivated += (i) => IncrementMissionProgress();
                return;
                #endregion
        }
    }

    private void UnassignMission()
    {
        if (_currentMission == null) return;
        UnassignMissionObjective();
        ResetMissionProgress();
    }

    private void UnassignMissionObjective()
    {
        if (_currentMission == null) return;
        switch (_currentMission.missionCondition)
        {
            case MissionConditions.EndMission:
                GameManager.OnMissionEnd -= IncrementMissionProgress;
                break;
            case MissionConditions.CollectPlasma:
                PlayerManager.OnPlasmaPickupValue -= IncreaseMissionProgress;
                break;
            case MissionConditions.FlyDistanceOnce:
                DistanceManager.OnDistanceChanged -= SetMissionProgress;
                break;
            case MissionConditions.FlyDistanceTotal:
                DistanceManager.OnDistanceTraveled -= IncrementMissionProgress;
                break;
            case MissionConditions.KillBoss:
                Boss.OnBossDied -= (V, P) => { IncrementMissionProgress(); };
                break;
            case MissionConditions.UseShield:
                PlayerShieldController.OnPlayerShieldsActivated -= (i) => IncrementMissionProgress();
                break;
            case MissionConditions.UseWeaponPack:
                PlayerWeaponController.OnWeaponUpgradeStart -= (i) => IncrementMissionProgress();
                break;
            case MissionConditions.DontShootForDistance:
                GameManager.OnMissionEnd += ResetMissionProgress;
                DistanceManager.OnDistanceTraveled += IncrementMissionProgress;
                PlayerWeaponController.OnShoot += FailMission;
                break;
        }
        _currentMission = null;
    }

    private void SetMissionProgress(int value)
    {
        Debug.Log(value);
        CurrentMissionProgress = value;
    }

    private void IncreaseMissionProgress(int value)
    {
        CurrentMissionProgress += value;
    }

    private void IncrementMissionProgress()
    {
        //Debug.Log("Mission Progress");
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
}
