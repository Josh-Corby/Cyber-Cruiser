using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Vector3 spawnSize;
    [SerializeField] private GameObject[] enemyPrefabs;

    private void Start()
    {
        SpawnEnemy();
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(-spawnSize.x / 2, spawnSize.x / 2);
        float y = Random.Range(-spawnSize.y / 2, spawnSize.y / 2);
        float z = Random.Range(-spawnSize.z / 2, spawnSize.z / 2);
        Vector3 spawnPosition = new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z + z);
        return spawnPosition;
    }
    public void SpawnEnemy()
    {
        GameObject randomEnemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length - 1)];
        GameObject enemy = Instantiate(randomEnemyPrefab, GetRandomSpawnPosition(),Quaternion.identity);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(gameObject.transform.position, spawnSize);
    }
}
