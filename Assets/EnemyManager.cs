using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : GameBehaviour<EnemyManager>
{
    [HideInInspector] public List<GameObject> gunshipsAlive = new();
    [SerializeField] private List<GameObject> _enemiesAlive = new();
    [SerializeField] private List<GameObject> _crashingEnemies = new();
   
    private void OnEnable()
    {
        Enemy.OnEnemyCrash += AddCrashingEnemy;
        Enemy.OnEnemyAliveStateChange += IsEnemyAlive;
        Gunship.OnGunshipAliveStateChange += IsGunshipAlive;
        GameManager.OnMissionEnd += ClearLists;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyCrash -= AddCrashingEnemy;
        Enemy.OnEnemyAliveStateChange -= IsEnemyAlive;
        Gunship.OnGunshipAliveStateChange -= IsGunshipAlive;
        GameManager.OnMissionEnd -= ClearLists;
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

    private void AddCrashingEnemy(GameObject enemy)
    {
        _crashingEnemies.Add(enemy);
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
                if (AreAllEnemiesDead())
                {
                    ESM.StartBossSpawn();
                }
            }
        }
    }

    private void ClearLists()
    {
        ClearList(_enemiesAlive);
        ClearList(_crashingEnemies);
        gunshipsAlive.Clear();
    }

    public bool AreAllEnemiesDead()
    {
        return _enemiesAlive.Count == 0;
    }
}
