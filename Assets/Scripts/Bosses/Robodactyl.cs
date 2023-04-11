using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robodactyl : Boss, IBoss
{
    [SerializeField] private Weapon[] laserLaunchers;
    [SerializeField] private GameObject minePrefab;
    [SerializeField] private GameObject _mineReleasePoint;

    //Double lasers
    public void Attack1()
    {
        for (int i = 0; i < laserLaunchers.Length; i++)
        {
            laserLaunchers[i].CheckFireTypes();
        }
    }

    //Release Mine
    public void Attack2()
    {
        Debug.Log("Mine spawned");
        GameObject mine = Instantiate(minePrefab, _mineReleasePoint.transform.position, _mineReleasePoint.transform.rotation);
        mine.transform.parent = null;
        ESM.enemiesAlive.Add(mine);
    }
}
