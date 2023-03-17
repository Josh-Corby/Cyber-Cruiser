using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gunship : Enemy, IDamageable
{

    protected override void Start()
    {
        base.Start();
        ESM.enemiesAlive.Add(gameObject);



    }

    public override void Destroy()
    {
        ESM.gunshipsAlive.Remove(gameObject);
        base.Destroy();
    }
}
