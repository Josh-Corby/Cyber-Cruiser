using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(GunshipMovement))]
public class Gunship : Enemy, IDamageable
{
    public static event Action<List<GameObject>, GameObject> OnGunshipSpawn = null;
    public static event Action<List<GameObject>, GameObject> OnGunshipDied = null;

    protected override void Start()
    {
        OnGunshipSpawn(ESM.gunshipsAlive, gameObject);
        base.Start();   
    }

    public override void Destroy()
    {
        OnGunshipDied(ESM.gunshipsAlive, gameObject);
        base.Destroy();
    }
}
