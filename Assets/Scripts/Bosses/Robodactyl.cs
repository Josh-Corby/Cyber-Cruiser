using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robodactyl : Boss, IBoss
{
    [SerializeField] private Weapon[] laserLaunchers;
    [SerializeField] private GameObject minePrefab;

    //Double lasers
    public void Attack1()
    {
        for (int i = 0; i < laserLaunchers.Length; i++)
        {
            laserLaunchers[i].Fire();
        }
    }

    //Release Mine
    public void Attack2()
    {
        Debug.Log("Mine spawned");
        GameObject mine = Instantiate(minePrefab, transform);
        mine.transform.parent = null;
    }
}
