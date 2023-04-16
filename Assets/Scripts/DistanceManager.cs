using UnityEngine;
using System;
using System.Collections;
using TMPro;
using Random = UnityEngine.Random;

public class DistanceManager : GameBehaviour
{
    [SerializeField] private int _bossSpawnDistance = 500;
    private const int MILESTONE_DISTANCE = 100;

    private float _distanceFloat;
    private int _distanceInt;
    private bool _isDistanceIncreasing;

    private int _previousBossDistance, _currentBossDistance;

    private int _currentDistanceMilestone;
    private bool isDistanceMilestoneIncreased;
    [SerializeField] private TMP_Text _distanceCounterText;

    private int _plasmaDropDistance, _weaponUpgradeDropDistance;
    private bool _isPlasmaSpawned, _isWeaponUpgradeSpawned = false;

    #region Properties
    public int DistanceInt
    {
        get
        {
            return _distanceInt;
        }
        set
        {
            _distanceInt = value;
            UpdateDistanceText();
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
    #endregion

    #region Actions
    public static event Action<PickupType> OnPlasmaDistanceReached = null;
    public static event Action OnWeaponUpgradeDistanceReached = null;
    public static event Action OnBossDistanceReached = null;
    #endregion

    private void OnEnable()
    {
        GameManager.OnMissionStart += ResetValues;
        WaveCountdownManager.OnCountdownDone += StartIncreasingDistance;
        WaveCountdownManager.OnCountdownDone += GenerateFirstPickupDistances;
        PlayerManager.OnPlayerDeath += StopIncreasingDistance;
        Boss.OnBossDied += (p, v) => { StartIncreasingDistance(); };
        PickupManager.OnPlasmaSpawned += () => { StartCoroutine(PlasmaSpawned()); } ;
        PickupManager.OnWeaponUpgradeSpawned += () => { StartCoroutine(WeaponUpgradeSpawned()); };
    }

    private void OnDisable()
    {
        GameManager.OnMissionStart -= ResetValues;
        WaveCountdownManager.OnCountdownDone -= StartIncreasingDistance;
        WaveCountdownManager.OnCountdownDone -= GenerateFirstPickupDistances;
        PlayerManager.OnPlayerDeath -= StopIncreasingDistance;
        Boss.OnBossDied -= (p, v) => { StartIncreasingDistance(); };
        PickupManager.OnPlasmaSpawned -= () => { StartCoroutine(PlasmaSpawned()); };
        PickupManager.OnWeaponUpgradeSpawned -= () => { StartCoroutine(WeaponUpgradeSpawned()); };
    }

    private void Update()
    {
        if (!IsDistanceIncreasing) return;

        IncreaseDistance();
        CheckDistance();
    }

    private void IncreaseDistance()
    {
        DistanceFloat += Time.deltaTime * 10;
        DistanceInt = Mathf.RoundToInt(DistanceFloat);
    }

    private void CheckDistance()
    {
        //increment distance milestone every 100 units
        if (DistanceInt % MILESTONE_DISTANCE == 0 && !isDistanceMilestoneIncreased)
        {
            StartCoroutine(IncreaseDistanceMilestone());
            GenerateNewPlasmaDropDistance();
        }

        //start boss fight at boss distance
        if (DistanceInt > 0 && DistanceInt % CurrentBossDistance == 0)
        {
            BossDistanceReached();
        }

        //spawn plasma at seeded distance
        if (DistanceInt == _plasmaDropDistance && !_isPlasmaSpawned)
        {
            PlasmaDistanceReached();
        }

        //spawn weapon pack at seeded distance
        if (DistanceInt == _weaponUpgradeDropDistance && !_isWeaponUpgradeSpawned)
        {
            WeaponUpgradeDistanceReached();
        }
    }

    private void PlasmaDistanceReached()
    {
        _isPlasmaSpawned = true;
        OnPlasmaDistanceReached(PickupType.Plasma);
    }

    private void WeaponUpgradeDistanceReached()
    {
        _isWeaponUpgradeSpawned = true;
        OnWeaponUpgradeDistanceReached?.Invoke();
    }

    private IEnumerator PlasmaSpawned()
    {
        yield return new WaitForSeconds(0.1f);
        _isPlasmaSpawned = false;
    }

    private IEnumerator WeaponUpgradeSpawned()
    {
        yield return new WaitForSeconds(0.1f);
        _isWeaponUpgradeSpawned = false;
    }

    private void BossDistanceReached()
    {
        PreviousBossDistance = DistanceInt;
        CurrentBossDistance += _bossSpawnDistance;
        StopIncreasingDistance();
        GenerateNewWeaponUpgradeDropDistance();
        OnBossDistanceReached?.Invoke();
    }

    protected void GenerateNewPlasmaDropDistance()
    {
        _plasmaDropDistance = Random.Range(CurrentDistanceMilestone + 15, CurrentDistanceMilestone + 99);
        _isPlasmaSpawned = false;
    }

    protected void GenerateNewWeaponUpgradeDropDistance()
    {
        _weaponUpgradeDropDistance = Random.Range(PreviousBossDistance + 15, CurrentBossDistance);
        Debug.Log(_weaponUpgradeDropDistance);
        _isWeaponUpgradeSpawned = false;
    }

    private void GenerateFirstPickupDistances()
    {
        GenerateNewPlasmaDropDistance();
        GenerateNewWeaponUpgradeDropDistance();
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

    private void UpdateDistanceText()
    {
        _distanceCounterText.text = DistanceInt.ToString();
    }
}
