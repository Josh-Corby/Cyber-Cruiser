using System.Collections.Generic;
using UnityEngine;
using System;

public class MissionManager : GameBehaviour<MissionManager>
{
    [SerializeField] private MissionScriptableObject _currentMission;
    [SerializeField] private List<MissionScriptableObject> _availableMissions = new();
    [SerializeField] private MissionScriptableObject[] _missions;

    [SerializeField] private int _currentMissionGoal;
    [SerializeField] private int _currentMissionProgress;
    [SerializeField] private bool _isMissionFailed;
    [SerializeField] private bool _isAnyMissionCompleted;

    private int CurrentMissionProgress
    {
        get => _currentMissionProgress;
        set
        {
            _currentMissionProgress = value;
            CheckMissionProgress();
        }
    }

    public bool IsAnyMissionCompleted { get => _isAnyMissionCompleted; private set => _isAnyMissionCompleted = value; }

    public static event Action<int> OnMissionComplete = null;

    private void OnEnable()
    {
        GameManager.OnMissionStart += () => { IsAnyMissionCompleted = false; };
    }

    private void OnDisable()
    {
        GameManager.OnMissionStart -= () => { IsAnyMissionCompleted = false; };
    }

    public void SetMission(MissionScriptableObject mission)
    {
        UnassignMission();
        _currentMission = mission;
        _currentMissionProgress = 0;
        SetMissionObjective();
    }

    private void SetMissionObjective()
    {
        _isMissionFailed = false;
        switch (_currentMission.missionCondition)
        {
            #region General
            case MissionConditions.EndMission:
                GameManager.OnMissionEnd += IncrementMissionProgress;
                break;
            #endregion

            #region Travel Distance
            case MissionConditions.FlyDistanceOnce:
                DistanceManager.OnDistanceChanged += SetMissionProgress;
                break;
            case MissionConditions.FlyDistanceTotal:
                DistanceManager.OnDistanceTraveled += IncrementMissionProgress;
                break;
            #endregion

            #region Pickups
            case MissionConditions.CollectPlasma:
                PlayerManager.OnPlasmaPickupValue += IncreaseMissionProgress;
                break;
            case MissionConditions.UseWeaponPack:
                PlayerWeaponController.OnWeaponUpgradeStart += (U, F) => { IncrementMissionProgress(); };
                break;
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
                        Boss.OnBossDied += (V, P) => { IncrementMissionProgress(); };
                        break;
                    case BossTypes.Battlecruiser:
                        break;
                    case BossTypes.Robodactyl:
                        Robodactyl.OnDied += IncrementMissionProgress;
                        break;
                    case BossTypes.Behemoth:
                        Behemoth.OnDied += IncrementMissionProgress;
                        break;

                }
                break;
            #endregion
            #endregion

            #region Pacifist
            case MissionConditions.DontShootForDistance:
                GameManager.OnMissionEnd += ResetMissionProgress;
                DistanceManager.OnDistanceTraveled += IncrementMissionProgress;
                PlayerWeaponController.OnShoot += FailMission;
                break;
            #endregion

            #region Shield
            case MissionConditions.UseShield:
                PlayerShieldController.OnPlayerShieldsActivated += (U, F) => { IncrementMissionProgress(); };
                break;
                #endregion
        }
        _currentMissionGoal = _currentMission.missionObjectiveAmount;
    }

    private void UnassignMission()
    {
        if (_currentMission == null) return;

        ResetMissionProgress();
        Debug.Log("Mission Complete");
        UnassignMissionObjective();
    }

    private void UnassignMissionObjective()
    {
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
                PlayerShieldController.OnPlayerShieldsActivated -= (U, F) => { IncrementMissionProgress(); };
                break;
            case MissionConditions.UseWeaponPack:
                PlayerWeaponController.OnWeaponUpgradeStart -= (U, F) => { IncrementMissionProgress(); };
                break;
            case MissionConditions.DontShootForDistance:
                GameManager.OnMissionEnd += ResetMissionProgress;
                DistanceManager.OnDistanceTraveled += IncrementMissionProgress;
                PlayerWeaponController.OnShoot += FailMission;
                break;
        }
    }

    private void SetMissionProgress(int value)
    {
        CurrentMissionProgress = value;
    }

    private void IncreaseMissionProgress(int value)
    {
        CurrentMissionProgress += value;
    }

    private void IncrementMissionProgress()
    {
        Debug.Log("Mission Progress");
        CurrentMissionProgress += 1;
    }

    private void CheckMissionProgress()
    {
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
        IsAnyMissionCompleted = true;
        OnMissionComplete(_currentMission.missionStarReward);
        UnassignMission();
    }

    private void FailMission()
    {
        _isMissionFailed = true;
    }
}
