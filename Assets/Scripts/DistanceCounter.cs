using UnityEngine;
using System;
using TMPro;

public class DistanceCounter : MonoBehaviour
{
    public static event Action OnBossDistanceReached;

    [SerializeField] private TMP_Text distanceText;
    private float distanceFloat;
    private int distanceInt;
    private bool distanceIncreasing;
    private readonly int bossDistance = 500;

    private void OnEnable()
    {
        GameplayUIManager.OnCountdownDone += StartIncreasingDistance;

        GameManager.OnLevelCountDownStart += ResetCounter;
        GameManager.OnGamePaused += StopIncreasingDistance;
        GameManager.OnGameResumed += StartIncreasingDistance;

        PlayerManager.OnPlayerDeath += StopIncreasingDistance;

        EnemySpawnerManager.OnBossDied += StartIncreasingDistance;
    }

    private void OnDisable()
    {
        GameplayUIManager.OnCountdownDone -= StartIncreasingDistance;

        GameManager.OnLevelCountDownStart -= ResetCounter;
        GameManager.OnGamePaused -= StopIncreasingDistance;
        GameManager.OnGameResumed -= StartIncreasingDistance;

        PlayerManager.OnPlayerDeath -= StopIncreasingDistance;

        EnemySpawnerManager.OnBossDied -= StartIncreasingDistance;
    }


    private void ResetCounter()
    {
        distanceIncreasing = false;
        distanceFloat = 0;
        distanceInt = 0;
        distanceText.text = distanceFloat.ToString();
    }

    private void Update()
    {
        if (!distanceIncreasing) return;

        if(distanceIncreasing)
        {
            Debug.Log("Distance is increasing");

            distanceFloat += Time.deltaTime * 10;
            distanceInt = Mathf.RoundToInt(distanceFloat);
            distanceText.text = distanceInt.ToString();

            if(distanceInt > 0 && distanceInt % bossDistance == 0)
            {
                Debug.Log("boss distance reached");
                StopIncreasingDistance();
                OnBossDistanceReached?.Invoke();
            }
        }  
    }

    private void StartIncreasingDistance()
    {
        distanceIncreasing = true;
    }

    private void StopIncreasingDistance()
    {
        distanceIncreasing = false;
    }
}
