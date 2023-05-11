using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class DistanceManager : GameBehaviour
{
    #region References
    [SerializeField] private TMP_Text _distanceCounterText;
    #endregion

    #region Fields
    [Header("Check Distances")]
    [SerializeField] private int _bossSpawnDistance = 500;
    private const int MILESTONE_DISTANCE = 100;

    [Header("General Distance")]
    private float _distanceFloat;
    private int _distanceInt;
    [SerializeField] private bool _isDistanceIncreasing;

    [Header("Boss distances")]
    private int _previousBossDistance, _currentBossDistance;

    [Header("Distance Milestones")]
    private int _currentDistanceMilestone;
    private bool isDistanceMilestoneIncreased;

    [Header("Pickup values")]
    private int _plasmaDropDistance, _weaponUpgradeDropDistance;
    private bool _isPlasmaSpawned, _isWeaponUpgradeSpawned = false;
    #endregion 

    #region Properties
    private int DistanceInt
    {
        get => _distanceInt;
        set
        {
            if (_distanceInt != value)
            {
                if (OnDistanceTraveled != null) OnDistanceTraveled?.Invoke();
            }
            _distanceInt = value;
            OnDistanceChanged?.Invoke(_distanceInt);
            UpdateDistanceText();
        }
    }
    #endregion

    #region Actions
    public static event Action<PickupType> OnPlasmaDistanceReached = null;
    public static event Action<int> OnDistanceChanged = null;
    public static event Action OnDistanceTraveled = null;
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
        PickupManager.OnPlasmaSpawned += () => { StartCoroutine(PlasmaSpawned()); };
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

    private void Start()
    {
        _isDistanceIncreasing = false;
    }

    private void Update()
    {
        if (GM.IsPaused) return;

        if (!_isDistanceIncreasing) return;

        IncreaseDistance();
        CheckDistance();
    }

    private void IncreaseDistance()
    {
        _distanceFloat += Time.deltaTime * 10;
        DistanceInt = Mathf.RoundToInt(_distanceFloat);
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
        if (DistanceInt > 0)
        {
            if (DistanceInt % _currentBossDistance == 0)
            {
                BossDistanceReached();
            }
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
        _previousBossDistance = DistanceInt;
        _currentBossDistance += _bossSpawnDistance;
        StopIncreasingDistance();
        GenerateNewWeaponUpgradeDropDistance();
        Debug.Log(DistanceInt);
        OnBossDistanceReached?.Invoke();
    }

    protected void GenerateNewPlasmaDropDistance()
    {
        _plasmaDropDistance = Random.Range(_currentDistanceMilestone + 15, _currentDistanceMilestone + 99);
        _isPlasmaSpawned = false;
    }

    protected void GenerateNewWeaponUpgradeDropDistance()
    {
        _weaponUpgradeDropDistance = Random.Range(_previousBossDistance + 15, _currentBossDistance);
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
        _currentDistanceMilestone += MILESTONE_DISTANCE;

        yield return new WaitForSeconds(1f);
        isDistanceMilestoneIncreased = false;
    }

    private void ResetValues()
    {
        _currentDistanceMilestone = 0;
        _previousBossDistance = 0;
        _distanceFloat = 0;
        DistanceInt = 0;
        _currentBossDistance = _bossSpawnDistance;
        isDistanceMilestoneIncreased = false;
        _isDistanceIncreasing = false;
    }

    private void StartIncreasingDistance()
    {
        _distanceFloat += 1;
        DistanceInt += 1;
        _isDistanceIncreasing = true;
    }

    private void StopIncreasingDistance()
    {
        _isDistanceIncreasing = false;
    }

    private void UpdateDistanceText()
    {
        _distanceCounterText.text = DistanceInt.ToString();
    }
}
