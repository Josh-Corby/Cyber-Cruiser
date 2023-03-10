using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class EnemySpawnerManager : MonoBehaviour
{
    [SerializeField] private EnemySpawner[] spawners;
    [SerializeField] private int spawnEnemyInterval;
    public bool spawnEnemies;

    private void OnEnable()
    {
        GameplayUIManager.OnCountdownDone += StartSpawningEnemies;
        PlayerManager.OnPlayerDeath += StopSpawningEnemies;
    }

    private void OnDisable()
    {
        GameplayUIManager.OnCountdownDone -= StartSpawningEnemies;
        PlayerManager.OnPlayerDeath -= StopSpawningEnemies;
    }

    private void StartSpawningEnemies()
    {
        StartCoroutine(SpawnEnemies());
    }

    private void StopSpawningEnemies()
    {
        StopCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (spawnEnemies)
        {
            yield return new WaitForSeconds(spawnEnemyInterval);
            ChooseRandomSpawner();
        }
    }
    private void ChooseRandomSpawner()
    {
        EnemySpawner currentspawner = spawners[Random.Range(0, spawners.Length - 1)];
        currentspawner.SpawnEnemy();
    }
}
