using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerManager : MonoBehaviour
{
    [SerializeField] private EnemySpawner[] spawners;

    private void ChooseRandomSpawner()
    {
        EnemySpawner currentspawner = spawners[ Random.Range(0, spawners.Length-1)];


    }
}
