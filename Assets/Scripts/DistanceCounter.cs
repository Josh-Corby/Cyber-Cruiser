using UnityEngine;
using System;
using TMPro;

public class DistanceCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text distanceText;
    private float distanceFloat;
    private int distanceInt;
    private bool distanceIncreasing;

    private void OnEnable()
    {
        GameplayUIManager.OnCountdownDone += StartIncreasingDistance;

        GameManager.OnLevelCountDownStart += ResetCounter;
        GameManager.OnGamePaused += StopIncreasingDistance;
        GameManager.OnGameResumed += StartIncreasingDistance;

        PlayerManager.OnPlayerDeath += StopIncreasingDistance;
    }

    private void OnDisable()
    {
        GameplayUIManager.OnCountdownDone -= StartIncreasingDistance;

        GameManager.OnLevelCountDownStart -= ResetCounter;
        GameManager.OnGamePaused -= StopIncreasingDistance;
        GameManager.OnGameResumed -= StartIncreasingDistance;

        PlayerManager.OnPlayerDeath -= StopIncreasingDistance;
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

        distanceFloat += Time.deltaTime * 10;
        distanceInt = Mathf.RoundToInt(distanceFloat);
        distanceText.text = distanceInt.ToString();
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
