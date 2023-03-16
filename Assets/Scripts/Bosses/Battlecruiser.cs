using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battlecruiser : Boss, IBoss
{
    [SerializeField] GameObject seekerMinePrefab;
    [SerializeField] private int minesToFire;
    private int mineDelay = 1;

    [SerializeField] private Weapon pulverizerBeam;

    //release seeker mines
    public void Attack1()
    {
        StartCoroutine(ReleaseMines());
    }

    private IEnumerator ReleaseMines()
    {
        for (int i = 0; i < minesToFire; i++)
        {
            GameObject seekerMine = Instantiate(seekerMinePrefab);
            yield return new WaitForSeconds(mineDelay);
        }
        StopCoroutine(ReleaseMines());
    }

    //pulverizer peam
    public void Attack2()
    {
        throw new System.NotImplementedException();
    }
}
