using UnityEngine;
using System;
using System.Collections;

public class DistanceManager : GameBehaviour
{
    [SerializeField] private int _bossSpawnDistance = 500;
    private const int MILESTONE_DISTANCE = 100;

    private float _distanceFloat;
    private int _distanceInt;
    private bool _isDistanceIncreasing;

    private int _previousBossDistance = 0;
    private int _currentBossDistance;

    private int _currentDistanceMilestone;
    private bool isDistanceMilestoneIncreased;

    private int _plasmaDropDistance;
    private int _weaponUpgradeDropDistance;
    private bool _isPlasmaSpawned = false;
    private bool _isWeaponUpgradeSpawned = false;

    public static event Action<PickupType> OnPlasmaDistanceReached = null;
    public static event Action OnWeaponUpgradeDistanceReached = null;
    public static event Action<int> OnDistanceChanged = null;
    public static event Action<int, Action<int>> OnPlasmaDropDistanceRequested = null;
    public static event Action<int, int, Action<int>> OnWeaponUpgradeDropDistanceRequested = null;

    public int DistanceInt
    {
        get
        {
            return _distanceInt;
        }
        set
        {
            _distanceInt = value;
            OnDistanceChanged(_distanceInt);
        }
    }

    public float DistanceFloat
    {
        get
        {
            return _distanceFloat;
        }

        set
        {
            _distanceFloat = value;
        }
    }

    public int CurrentDistanceMilestone
    {
        get
        {
            return _currentDistanceMilestone;
        }
        set
        {
            _currentDistanceMilestone = value;
        }
    }

    public int PreviousBossDistance
    {
        get
        {
            return _previousBossDistance;
        }
        set
        {
            _previousBossDistance = value;
        }
    }

    public int CurrentBossDistance
    {
        get
        {
            return _currentBossDistance;
        }
        set
        {
            _currentBossDistance = value;
        }
    }

    public bool IsDistanceIncreasing
    {
        get
        {
            return _isDistanceIncreasing;
        }
        set
        {
            _isDistanceIncreasing = value;
        }
    }

    private void OnEnable()
    {
        GameManager.OnLevelCountDownStart += ResetValues;
        GameplayUIManager.OnCountdownDone += StartIncreasingDistance;
        GameplayUIManager.OnCountdownDone += RequestFirstPickupDistances;
        PlayerManager.OnPlayerDeath += StopIncreasingDistance;
        Boss.OnBossDied += (v) => { StartIncreasingDistance(); };
    }

    private void OnDisable()
    {
        GameManager.OnLevelCountDownStart -= ResetValues;
        GameplayUIManager.OnCountdownDone -= StartIncreasingDistance;
        GameplayUIManager.OnCountdownDone -= RequestFirstPickupDistances;
        PlayerManager.OnPlayerDeath -= StopIncreasingDistance;
        Boss.OnBossDied -= (v) => { StartIncreasingDistance(); };
    }

    private void Update()
    {
        if (!IsDistanceIncreasing) return;

        if (IsDistanceIncreasing)
        {
            DistanceFloat += Time.deltaTime * 10;
            DistanceInt = Mathf.RoundToInt(DistanceFloat);

            //increment distance milestone every 100 units
            if (DistanceInt % MILESTONE_DISTANCE == 0 && !isDistanceMilestoneIncreased)
            {
                StartCoroutine(IncreaseDistanceMilestone());
                RequestNewPlasmaDropDistance();
            }

            //start boss fight at boss distance
            if (DistanceInt > 0 && DistanceInt % CurrentBossDistance == 0)
            {
                PreviousBossDistance = DistanceInt;
                CurrentBossDistance += _bossSpawnDistance;
                Debug.Log("boss distance reached");
                StopIncreasingDistance();
                ESM.SetupForBossSpawn();
                RequestNewWeaponUpgradeDropDistance();
            }

            //spawn plasma at seeded distance
            if (DistanceInt == _plasmaDropDistance && !_isPlasmaSpawned)
            {
                OnPlasmaDistanceReached(PickupType.Plasma);
                _isPlasmaSpawned = true;
            }

            if (_distanceInt == _weaponUpgradeDropDistance && !_isWeaponUpgradeSpawned)
            {
                OnWeaponUpgradeDistanceReached?.Invoke();
                _isWeaponUpgradeSpawned = true;
            }
        }
    }

    private void RequestNewPlasmaDropDistance()
    {
        OnPlasmaDropDistanceRequested(_currentDistanceMilestone, SetPlasmaDropDistance);
        _isPlasmaSpawned = false;
    }

    private void SetPlasmaDropDistance(int value)
    {
        _plasmaDropDistance = value;
    }

    private void RequestNewWeaponUpgradeDropDistance()
    {
        OnWeaponUpgradeDropDistanceRequested(_previousBossDistance, _currentBossDistance, SetWeaponUpgradeDropDistance);
        _isWeaponUpgradeSpawned = false;
    }

    private void SetWeaponUpgradeDropDistance(int value)
    {
        _weaponUpgradeDropDistance = value;
    }

    private void RequestFirstPickupDistances()
    {
        RequestNewPlasmaDropDistance();
        RequestNewWeaponUpgradeDropDistance();
    }

    private IEnumerator IncreaseDistanceMilestone()
    {
        isDistanceMilestoneIncreased = true;
        CurrentDistanceMilestone += MILESTONE_DISTANCE;

        yield return new WaitForSeconds(1f);
        isDistanceMilestoneIncreased = false;
    }

    private void ResetValues()
    {
        CurrentDistanceMilestone = 0;
        PreviousBossDistance = 0;
        DistanceFloat = 0;
        DistanceInt = 0;
        CurrentBossDistance = _bossSpawnDistance;
        isDistanceMilestoneIncreased = false;
        IsDistanceIncreasing = false;
    }

    private void StartIncreasingDistance()
    {
        DistanceFloat += 1;
        DistanceInt += 1;
        IsDistanceIncreasing = true;
    }

    private void StopIncreasingDistance()
    {
        IsDistanceIncreasing = false;
    }
}
