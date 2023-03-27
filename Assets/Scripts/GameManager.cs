using UnityEngine;
using System.Collections;
using System;
using TMPro;

public class GameManager : GameBehaviour<GameManager>
{
    private const int BOSS_SPAWN_DISTANCE = 500;
    private const int MILESTONE_DISTANCE = 100;

    public bool isPaused = false;

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

    [SerializeField] private GameObject gameplayObjects;

    public static event Action OnLevelCountDownStart = null;
    public static event Action OnGamePaused = null;
    public static event Action OnGameResumed = null;
    public static event Action OnBossDistanceReached;
    public static event Action OnPlasmaDistanceReached = null;
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

    private void OnEnable()
    {
        InputManager.OnPause += TogglePause;
        UIManager.OnLevelEntry += StartLevel;
        GameplayUIManager.OnCountdownDone += StartIncreasingDistance;
        GameplayUIManager.OnCountdownDone += RequestFirstPickupDistances;
        PlayerManager.OnPlayerDeath += StopIncreasingDistance;
        EnemySpawnerManager.OnBossDied += OnBossDied;
    }

    private void OnDisable()
    {
        InputManager.OnPause -= TogglePause;
        UIManager.OnLevelEntry -= StartLevel;
        GameplayUIManager.OnCountdownDone -= StartIncreasingDistance;
        GameplayUIManager.OnCountdownDone -= RequestFirstPickupDistances;
        PlayerManager.OnPlayerDeath -= StopIncreasingDistance;
        EnemySpawnerManager.OnBossDied -= OnBossDied;
    }

    private void Start()
    {
        ResetDistances();
    }

    private void ResetDistances()
    {
        isDistanceMilestoneIncreased = false;
        CurrentDistanceMilestone = 0;
        PreviousBossDistance = 0;
        CurrentBossDistance = BOSS_SPAWN_DISTANCE;
    }

    private void Update()
    {
        if (!_isDistanceIncreasing) return;

        if (_isDistanceIncreasing)
        {
            _distanceFloat += Time.deltaTime * 10;
            DistanceInt = Mathf.RoundToInt(_distanceFloat);

            //increment distance milestone every 100 units
            if (_distanceInt % MILESTONE_DISTANCE == 0 && !isDistanceMilestoneIncreased)
            {   
                StartCoroutine(IncreaseDistanceMilestone());
                RequestNewPlasmaDropDistance();
            }

            //start boss fight at boss distance
            if (_distanceInt > 0 && _distanceInt % _currentBossDistance == 0)
            {
                PreviousBossDistance = _distanceInt;
                CurrentBossDistance += BOSS_SPAWN_DISTANCE;
                Debug.Log("boss distance reached");
                StopIncreasingDistance();
                OnBossDistanceReached?.Invoke();
                RequestNewWeaponUpgradeDropDistance();
            }

            //spawn plasma at seeded distance
            if (_distanceInt == _plasmaDropDistance && !_isPlasmaSpawned)
            {
                OnPlasmaDistanceReached?.Invoke();
                _isPlasmaSpawned = true;
            }

            if(_distanceInt == _weaponUpgradeDropDistance && !_isWeaponUpgradeSpawned)
            {
                OnWeaponUpgradeDistanceReached?.Invoke();
                _isWeaponUpgradeSpawned = true;
            }
        }
    }
    private void RequestFirstPickupDistances()
    {
        RequestNewPlasmaDropDistance();
        RequestNewWeaponUpgradeDropDistance();
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
    
    private IEnumerator IncreaseDistanceMilestone()
    {
        isDistanceMilestoneIncreased = true;
        CurrentDistanceMilestone += MILESTONE_DISTANCE;

        yield return new WaitForSeconds(1f);
        isDistanceMilestoneIncreased = false;
    }

    private void OnBossDied()
    {
        StartIncreasingDistance();
    }

    private void ResetCounter()
    {
        _isDistanceIncreasing = false;
        _distanceFloat = 0;
        DistanceInt = 0;
    }

    public void StartLevel()
    {
        isPaused = false;
        Time.timeScale = 1f;
        OnLevelCountDownStart?.Invoke();
        ResetCounter();
        gameplayObjects.SetActive(true);
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

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            OnGamePaused?.Invoke();
            PauseGame();
        }
        else if (!isPaused)
        {
            ResumeGame();
            OnGameResumed?.Invoke();
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}
