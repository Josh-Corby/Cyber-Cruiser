using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DistanceCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text distanceText;
    private float distanceFloat;
    private int distanceInt;
    private void Start()
    {
        distanceFloat = 0;
        distanceInt = 0;
        distanceText.text = distanceFloat.ToString();
    }

    private void Update()
    {
        distanceFloat += Time.deltaTime * 10;
        distanceInt = Mathf.RoundToInt(distanceFloat);
        distanceText.text = distanceInt.ToString();
    }

}
