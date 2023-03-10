using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class EnemySpawnerManager : GameBehaviour<EnemySpawnerManager>
{
    [SerializeField] private EnemySpawner[] spawners;
    [SerializeField] private int spawnEnemyInterval;

    private Coroutine spawnEnemiesCoroutine;

    public List<GameObject> enemiesAlive = new List<GameObject>();
    public bool spawnEnemies;

    private void OnEnable()
    {
        GameManager.OnLevelCountDownStart += RestartLevel;
        GameplayUIManager.OnCountdownDone += StartSpawningEnemies;
        PlayerManager.OnPlayerDeath += StopSpawningEnemies;
    }

    private void OnDisable()
    {
        GameManager.OnLevelCountDownStart -= RestartLevel;
        GameplayUIManager.OnCountdownDone -= StartSpawningEnemies;
        PlayerManager.OnPlayerDeath -= StopSpawningEnemies;
    }


    private void StartSpawningEnemies()
    {
        spawnEnemiesCoroutine = StartCoroutine(SpawnEnemies());
    }

    private void StopSpawningEnemies()
    {
        if (spawnEnemiesCoroutine != null)
        {
            StopCoroutine(spawnEnemiesCoroutine);
        }
    }

    private void RestartLevel()
    {
        StopSpawningEnemies();
        ClearEnemiesAlive();
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
        enemiesAlive.Add(currentspawner.SpawnEnemy());
    }

    private void ClearEnemiesAlive()
    {
        for (int i = 0; i < enemiesAlive.Count; i++)
        {
            GameObject enemy = enemiesAlive[i];
            enemiesAlive.Remove(enemy);
            Destroy(enemy);
        }
    }
}
