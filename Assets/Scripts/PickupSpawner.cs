using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;

public class PickupSpawner : GameBehaviour
{
    [SerializeField] private Vector3 spawnSize;
    //local offset for indicator spawning of this spawner
    public Vector3 _pickupIndicatorPosition;

    public Vector3 GetRandomPosition()
    {
        float x = Random.Range(-spawnSize.x / 2, spawnSize.x / 2);
        float y = Random.Range(-spawnSize.y / 2, spawnSize.y / 2);
        float z = 0;
        Vector3 spawnPosition = new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z + z);
        return spawnPosition;
    }

    public void SpawnPickupAtPosition(GameObject pickup, Vector3 position)
    {
        GameObject _pickup = Instantiate(pickup, position, transform.rotation);
    }

    public void SpawnPickupAtRandomPosition(GameObject pickup)
    {
        GameObject _pickup = Instantiate(pickup, GetRandomPosition(), transform.rotation);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, spawnSize);
    }
}
