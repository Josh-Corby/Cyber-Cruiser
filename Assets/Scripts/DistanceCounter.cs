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
        LevelUIManager.OnCountdownDone += StartIncreasingDistance;
    }

    private void OnDisable()
    {
        LevelUIManager.OnCountdownDone -= StartIncreasingDistance;
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
}
