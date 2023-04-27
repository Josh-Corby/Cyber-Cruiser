using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robodactyl : Boss, IBoss
{
    [SerializeField] private Weapon[] _laserLaunchers;
    [SerializeField] private EnemyScriptableObject _mineInfo;
    [SerializeField] private GameObject _mineReleasePoint;

    //Double lasers
    public void Attack1()
    {
        for (int i = 0; i < _laserLaunchers.Length; i++)
        {
            _laserLaunchers[i].CheckFireTypes();
        }
    }

    //Release Mine
    public void Attack2()
    {
        GameObject mine = Instantiate(EM.CreateEnemyFromSO(_mineInfo), _mineReleasePoint.transform.position, _mineReleasePoint.transform.rotation);
        mine.transform.parent = null;
    }
}
