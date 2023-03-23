using UnityEngine;
using System.Collections;
using System;
using TMPro;

public class GameManager : GameBehaviour
{
    public static event Action OnLevelCountDownStart = null;
    public static event Action OnGamePaused = null;
    public static event Action OnGameResumed = null;
    public static event Action OnBossDistanceReached;

    private TMP_Text distanceText;
    private float distanceFloat;
    private int distanceInt;
    private bool distanceIncreasing;
    [SerializeField] private int bossDistance;

    private int currentDistanceMilestone;
    private bool milestoneIncreased;

    [SerializeField] private PlasmaSpawner plasmaSpawner;
    private int plasmaDropDistance;
    private bool plasmaSpawned = false;

    [SerializeField] private GameObject gameplayObjects;
    private bool isPaused = false;

    private void OnEnable()
    {
        InputManager.OnPause += TogglePause;
        UIManager.OnLevelEntry += StartLevel;

        GameplayUIManager.OnCountdownDone += StartIncreasingDistance;
        GameplayUIManager.OnCountdownDone += GetNewPlasmaDropDistance;
        PlayerManager.OnPlayerDeath += StopIncreasingDistance;
        EnemySpawnerManager.OnBossDied += StartIncreasingDistance;
    }

    private void OnDisable()
    {
        InputManager.OnPause -= TogglePause;
        UIManager.OnLevelEntry -= StartLevel;

        GameplayUIManager.OnCountdownDone -= StartIncreasingDistance;
        GameplayUIManager.OnCountdownDone -= GetNewPlasmaDropDistance;
        PlayerManager.OnPlayerDeath -= StopIncreasingDistance;
        EnemySpawnerManager.OnBossDied -= StartIncreasingDistance;
    }

    private void Awake()
    {
        distanceText = GUIM.distanceCounterText;
        plasmaSpawner = GetComponentInChildren<PlasmaSpawner>();
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        currentDistanceMilestone = 0;
        milestoneIncreased = false;
    }
    private void Update()
    {
        if (!distanceIncreasing) return;

        if (distanceIncreasing)
        {
            distanceFloat += Time.deltaTime * 10;
            distanceInt = Mathf.RoundToInt(distanceFloat);
            distanceText.text = distanceInt.ToString();


            //increment distance milestone every 100 units
            if (distanceInt % bossDistance == 0 && !milestoneIncreased)
            {
                StartCoroutine(IncreaseDistanceMilestone());
                GetNewPlasmaDropDistance();
            }

            //start boss fight at boss distance
            if (distanceInt > 0 && distanceInt % bossDistance == 0)
            {
                Debug.Log("boss distance reached");
                StopIncreasingDistance();
                OnBossDistanceReached?.Invoke();
            }

            //spawn plasma at seeded distance
            if (distanceInt == plasmaDropDistance && !plasmaSpawned)
            {
                SpawnPlasma();
                plasmaSpawned = true;
            }
        }
    }

    
    private IEnumerator IncreaseDistanceMilestone()
    {
        milestoneIncreased = true;
        currentDistanceMilestone += bossDistance;

        yield return new WaitForSeconds(1f);
        milestoneIncreased = false;

    }

    private void GetNewPlasmaDropDistance()
    {
        plasmaDropDistance = plasmaSpawner.SetPlasmaDropDistance(currentDistanceMilestone);
        Debug.Log("Plasma spawn distance is: " + plasmaDropDistance);
        plasmaSpawned = false;
    }

    private void SpawnPlasma()
    {
        plasmaSpawner.SpawnPlasma();
    }

    private void ResetCounter()
    {
        distanceIncreasing = false;
        distanceFloat = 0;
        distanceInt = 0;
        distanceText.text = distanceFloat.ToString();
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
        distanceFloat += 1;
        distanceInt += 1;
        distanceIncreasing = true;
    }

    private void StopIncreasingDistance()
    {
        distanceIncreasing = false;
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
