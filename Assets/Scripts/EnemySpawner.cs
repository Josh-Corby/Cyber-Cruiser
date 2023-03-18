using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : GameBehaviour
{
    public static event Action<List<GameObject>, GameObject> OnEnemySpawned = null;

    [SerializeField] private Vector3 spawnSize;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private float speedModifier;

    //private GameObject EnemyIndicator;
    //private Vector3 EnemyIndicatorPosition;
    //private float IndicatorAngle;

    private void Start()
    {
        speedModifier = 0;
    }

    public void IncrementSpeedModifier(float value)
    {
        speedModifier += value;
    }

    public void StartSpawnProcess()
    {
        //CreateIndicator(randomposition);
        SpawnRandomEnemy();
    }

    public void StartBossSpawn(GameObject bossToSpawn)
    {
        //CreateIndicator(transform.position);
        SpawnBoss(transform.position, bossToSpawn);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(-spawnSize.x / 2, spawnSize.x / 2);
        float y = Random.Range(-spawnSize.y / 2, spawnSize.y / 2);
        float z = 0;
        Vector3 spawnPosition = new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z + z);
        return spawnPosition;
    }

    private void SpawnRandomEnemy()
    {
        GameObject randomEnemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        GameObject enemy = Instantiate(randomEnemyPrefab, GetRandomSpawnPosition(), Quaternion.identity);
        AddSpeedModifier(enemy);
        OnEnemySpawned(ESM.enemiesAlive, enemy);
    }

    private void SpawnBoss(Vector3 spawnPosition, GameObject bossToSpawn)
    {
        GameObject boss = Instantiate(bossToSpawn, spawnPosition, Quaternion.identity);
        AddSpeedModifier(boss);
        OnEnemySpawned(ESM.enemiesAlive, boss);
    }

    private void AddSpeedModifier(GameObject enemy)
    {
        if(speedModifier > 0)
        {
            enemy.GetComponent<Enemy>().speed += speedModifier;
        } 
    }

    //private void CreateIndicator(Vector2 position)
    //{
    //    GameObject Indicator = Instantiate(EnemyIndicator, transform.position, Quaternion.identity);

    //    Indicator.transform.position += EnemyIndicatorPosition;

    //    if (EnemyIndicatorPosition.x == 0)
    //    {
    //        Indicator.transform.position += new Vector3(position.x, 0);
    //    }


    //    if (EnemyIndicatorPosition.y == 0)
    //    {
    //        Indicator.transform.position += new Vector3(0, position.y);
    //    }

    //    Indicator.transform.rotation = Quaternion.Euler(0, 0, IndicatorAngle);
    //    Indicator.GetComponent<EnemyIndicator>().IndicatorTimer(spawnDelay));
    //}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, spawnSize);
    }
}
