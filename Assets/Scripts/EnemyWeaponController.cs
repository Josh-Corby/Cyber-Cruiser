using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponController : Weapon
{
    private void Start()
    {
        EnableWeapon();
    }

    private void EnableWeapon()
    {
        AutoFire = true;
    }
    public void DisableWeapon()
    {
        AutoFire = false;
    }
}
