using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : GameBehaviour<EnemyManager>
{
    [HideInInspector] public List<GameObject> _enemiesAlive = new();
    [HideInInspector] public List<GameObject> gunshipsAlive = new();

    private void OnEnable()
    {
        Enemy.OnEnemyAliveStateChange += IsEnemyAlive;
        Gunship.OnGunshipAliveStateChange += IsGunshipAlive;
        GameManager.OnMissionStart += ClearEnemiesAlive;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyAliveStateChange -= IsEnemyAlive;
        Gunship.OnGunshipAliveStateChange -= IsGunshipAlive;
        GameManager.OnMissionStart -= ClearEnemiesAlive;
    }

    private void IsEnemyAlive(GameObject enemy, bool aliveState)
    {
        if (aliveState)
        {
            AddToList(_enemiesAlive, enemy);
        }
        if (!aliveState)
        {
            RemoveFromList(_enemiesAlive, enemy);
        }
    }

    private void IsGunshipAlive(GameObject gunship, bool aliveState)
    {
        if (aliveState)
        {
            AddToList(gunshipsAlive, gunship);
        }
        if (!aliveState)
        {
            RemoveFromList(gunshipsAlive, gunship);
        }
    }


    /// <summary>
    /// Add given object to given list
    /// </summary>
    /// <param name="listToAddTo"></param>
    /// <param name="unit"></param>
    private void AddToList(List<GameObject> listToAddTo, GameObject unit)
    {
        if (!listToAddTo.Contains(unit))
        {
            listToAddTo.Add(unit);
        }
    }

    /// <summary>
    /// remove given object from given list
    /// </summary>
    /// <param name="listToRemoveFrom"></param>
    /// <param name="unit"></param>
    private void RemoveFromList(List<GameObject> listToRemoveFrom, GameObject unit)
    {
        if (listToRemoveFrom.Contains(unit))
        {
            listToRemoveFrom.Remove(unit);
        }

        //If an enemy was removed from enemies alive, and if the boss is ready to be spawned, check how many enemies are alive
        if (listToRemoveFrom == _enemiesAlive)
        {
            if (ESM.bossReadyToSpawn)
            {
                CheckEnemiesAliveForBossSpawn();
            }
        }
    }

    private void ClearEnemiesAlive()
    {
        if (_enemiesAlive.Count > 0)
        {
            for (int i = _enemiesAlive.Count - 1; i >= 0; i--)
            {
                GameObject enemyToRemove = _enemiesAlive[i];
                _enemiesAlive.RemoveAt(i);

                Destroy(enemyToRemove);
            }
        }
        gunshipsAlive.Clear();
    }

    public bool CheckEnemiesAliveForBossSpawn()
    {
        return _enemiesAlive.Count == 0;
    }
}
