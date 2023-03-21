using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaSpawner : GameBehaviour
{
    [SerializeField] private Vector3 spawnSize;
    [SerializeField] private GameObject plasmaPrefab;

    public int SetPlasmaDropDistance(int currentDistanceMilestone)
    {
        int plasmaDropDistance = Random.Range(currentDistanceMilestone + 15, currentDistanceMilestone + 99);

        return plasmaDropDistance;
    }

    private Vector3 GetRandomPosition()
    {
        float x = Random.Range(-spawnSize.x / 2, spawnSize.x / 2);
        float y = Random.Range(-spawnSize.y / 2, spawnSize.y / 2);
        float z = 0;
        Vector3 spawnPosition = new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z + z);
        return spawnPosition;
    }
    public void SpawnPlasma()
    {
        GameObject plasma = Instantiate(plasmaPrefab, GetRandomPosition(), Quaternion.identity);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, spawnSize);
    }
}
