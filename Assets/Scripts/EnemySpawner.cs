using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : GameBehaviour
{
    public static event Action<List<GameObject>, GameObject> OnEnemySpawned = null;
    public static event Action<Enemy> OnBossSpawned = null;

    [SerializeField] private Vector3 spawnArea;
    [SerializeField] private GameObject[] enemiesToSpawn;
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
        float x = Random.Range(-spawnArea.x / 2, spawnArea.x / 2);
        float y = Random.Range(-spawnArea.y / 2, spawnArea.y / 2);
        float z = 0;
        Vector3 spawnPosition = new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z + z);
        return spawnPosition;
    }

    private void SpawnRandomEnemy()
    {
        GameObject randomEnemyPrefab = enemiesToSpawn[Random.Range(0, enemiesToSpawn.Length)];
        GameObject enemy = Instantiate(randomEnemyPrefab, GetRandomSpawnPosition(), transform.rotation);
        AddSpeedModifier(enemy);
        OnEnemySpawned(ESM.enemiesAlive, enemy);
    }

    private void SpawnBoss(Vector3 spawnPosition, GameObject bossToSpawn)
    {
        GameObject boss = Instantiate(bossToSpawn, spawnPosition, transform.rotation);
        AddSpeedModifier(boss);
        OnEnemySpawned(ESM.enemiesAlive, boss);
        Enemy bossInfo = boss.GetComponent<Enemy>();
        boss.name = bossInfo.enemyName;
        OnBossSpawned(bossInfo);
    }

    private void AddSpeedModifier(GameObject enemy)
    {
        if(speedModifier > 0)
        {
            enemy.GetComponent<EnemyMovement>().speed += speedModifier;
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
        Gizmos.DrawWireCube(transform.position, spawnArea);
    }
}
