using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class PickupSpawner : GameBehaviour
{
    public static event Action<GameObject> OnPlasmaSpawned = null;

    [SerializeField] private Vector3 spawnSize;
    [SerializeField] private GameObject plasmaPrefab;
    [SerializeField] private GameObject[] weaponUpgradePrefabs;

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
        GameObject plasma = Instantiate(plasmaPrefab, GetRandomPosition(), transform.rotation);
        OnPlasmaSpawned(plasma);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, spawnSize);
    }
}
