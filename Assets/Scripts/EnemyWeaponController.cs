using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponController : Weapon
{

    protected override void Start()
    {
        base.Start();
        EnableWeapon();
    }

    private void EnableWeapon()
    {
        _autoFire = true;
    }
    public void DisableWeapon()
    {
        _autoFire = false;
    }
}
