using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behemoth : Boss, IBoss
{
    [SerializeField] private Weapon missileLauncher;
    [SerializeField] private Weapon homingMissileLauncher;

    //missiles in random directions
    public void Attack1()
    {
        missileLauncher.CheckFireTypes();
    }

    //shoot homing missiles at different directions
    public void Attack2()
    {
        homingMissileLauncher.CheckFireTypes();
    }
}
