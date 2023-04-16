using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(GunshipMovement))]
public class Gunship : Enemy, IDamageable
{
    public static event Action<GameObject, bool> OnGunshipAliveStateChange = null;

    protected override void Start()
    {
        OnGunshipAliveStateChange(gameObject, true);
        base.Start();   
    }

    public override void Destroy()
    {
        OnGunshipAliveStateChange(gameObject, false);
        base.Destroy();
    }
}
