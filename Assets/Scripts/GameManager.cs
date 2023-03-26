using UnityEngine;
using System.Collections;
using System;
using TMPro;

public class GameManager : GameBehaviour<GameManager>
{
    public static event Action OnLevelCountDownStart = null;
    public static event Action OnGamePaused = null;
    public static event Action OnGameResumed = null;
    public static event Action OnBossDistanceReached;

    private TMP_Text _distanceText;
    private float _distanceFloat;
    private int _distanceInt;
    private bool _distanceIncreasing;
    [SerializeField] private int _bossDistance;

    private readonly int _milestoneDistance = 100;
    //number used for plasma drop distance generation
    private int _currentDistanceMilestone; 
    private bool _milestoneIncreased;

    [SerializeField] private PickupSpawner _pickupSpawner;
    private int _plasmaDropDistance;
    private bool _plasmaSpawned = false;

    [SerializeField] private GameObject gameplayObjects;
    public bool isPaused = false;

    private void OnEnable()
    {
        InputManager.OnPause += TogglePause;
        UIManager.OnLevelEntry += StartLevel;

        GameplayUIManager.OnCountdownDone += StartIncreasingDistance;
        GameplayUIManager.OnCountdownDone += GetNewPlasmaDistance;
        PlayerManager.OnPlayerDeath += StopIncreasingDistance;
        EnemySpawnerManager.OnBossDied += StartIncreasingDistance;
    }

    private void OnDisable()
    {
        InputManager.OnPause -= TogglePause;
        UIManager.OnLevelEntry -= StartLevel;

        GameplayUIManager.OnCountdownDone -= StartIncreasingDistance;
        GameplayUIManager.OnCountdownDone -= GetNewPlasmaDistance;
        PlayerManager.OnPlayerDeath -= StopIncreasingDistance;
        EnemySpawnerManager.OnBossDied -= StartIncreasingDistance;
    }

    private void Awake()
    {
        _distanceText = GUIM.distanceCounterText;
        _pickupSpawner = GetComponentInChildren<PickupSpawner>();
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        _currentDistanceMilestone = 0;
        _milestoneIncreased = false;
    }
    private void Update()
    {
        if (!_distanceIncreasing) return;

        if (_distanceIncreasing)
        {
            _distanceFloat += Time.deltaTime * 10;
            _distanceInt = Mathf.RoundToInt(_distanceFloat);
            _distanceText.text = _distanceInt.ToString();


            //increment distance milestone every 100 units
            if (_distanceInt % _milestoneDistance == 0 && !_milestoneIncreased)
            {   
                StartCoroutine(IncreaseDistanceMilestone());
                GetNewPlasmaDistance();
            }

            //start boss fight at boss distance
            if (_distanceInt > 0 && _distanceInt % _bossDistance == 0)
            {
                Debug.Log("boss distance reached");
                StopIncreasingDistance();
                OnBossDistanceReached?.Invoke();
            }

            //spawn plasma at seeded distance
            if (_distanceInt == _plasmaDropDistance && !_plasmaSpawned)
            {
                _pickupSpawner.SpawnPlasma();
                _plasmaSpawned = true;
            }
        }
    }

    
    private IEnumerator IncreaseDistanceMilestone()
    {
        _milestoneIncreased = true;
        _currentDistanceMilestone += _milestoneDistance;

        yield return new WaitForSeconds(1f);
        _milestoneIncreased = false;

    }

    private void GetNewPlasmaDistance()
    {
        _plasmaDropDistance = _pickupSpawner.SetPlasmaDropDistance(_currentDistanceMilestone);
        Debug.Log("Plasma spawn distance is: " + _plasmaDropDistance);
        _plasmaSpawned = false;
    }

    private void ResetCounter()
    {
        _distanceIncreasing = false;
        _distanceFloat = 0;
        _distanceInt = 0;
        _distanceText.text = _distanceFloat.ToString();
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
        _distanceInt += 1;
        _distanceIncreasing = true;
    }

    private void StopIncreasingDistance()
    {
        _distanceIncreasing = false;
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
