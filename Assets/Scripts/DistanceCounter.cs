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

        GameManager.OnGamePaused += StopIncreasingDistance;
        GameManager.OnGameResumed += StartIncreasingDistance;
    }

    private void OnDisable()
    {
        GameplayUIManager.OnCountdownDone -= StartIncreasingDistance;

        GameManager.OnGamePaused -= StopIncreasingDistance;
        GameManager.OnGameResumed -= StartIncreasingDistance;
    }

    private void Start()
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
