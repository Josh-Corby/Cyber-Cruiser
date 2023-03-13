using System.Collections;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public static event Action<GameObject> OnEnemySpawned = null;

    [SerializeField] private Vector3 spawnSize;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private GameObject EnemyIndicator;
    [SerializeField] private Vector3 EnemyIndicatorPosition;
    [SerializeField] private float IndicatorAngle;
    [SerializeField] private float spawnDelay;

    public void StartSpawnProcess()
    {
        Vector3 randomposition = GetRandomSpawnPosition();
        CreateIndicator(randomposition);
        StartCoroutine(SpawnRandomEnemy(randomposition));
    }

    public void StartBossSpawn(GameObject bossToSpawn)
    {
        CreateIndicator(transform.position);
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

    private void CreateIndicator(Vector2 position)
    {
        GameObject Indicator = Instantiate(EnemyIndicator, transform.position, Quaternion.identity);

        Indicator.transform.position += EnemyIndicatorPosition;

        if(EnemyIndicatorPosition.x == 0)
        {
            Indicator.transform.position += new Vector3(position.x, 0);
        }


        if(EnemyIndicatorPosition.y == 0)
        {
            Indicator.transform.position += new Vector3(0, position.y);
        }

        Indicator.transform.rotation = Quaternion.Euler(0, 0, IndicatorAngle);
        StartCoroutine(Indicator.GetComponent<EnemyIndicator>().IndicatorTimer(spawnDelay));
    }

    private IEnumerator SpawnRandomEnemy(Vector3 spawnPosition)
    {
        yield return new WaitForSeconds(spawnDelay);
        GameObject randomEnemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        GameObject enemy = Instantiate(randomEnemyPrefab, spawnPosition, Quaternion.identity);
        OnEnemySpawned(enemy);
    }

    private void SpawnBoss(Vector3 spawnPosition, GameObject bossToSpawn)
    {
        GameObject boss = Instantiate(bossToSpawn, spawnPosition, Quaternion.identity);
        OnEnemySpawned(boss);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(gameObject.transform.position, spawnSize);
    }
}
