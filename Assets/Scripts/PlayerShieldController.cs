using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShieldController : ShieldBase, IShield
{
    [SerializeField] private int shieldActiveDuration;
    [SerializeField] private float shieldActiveTimer;

    private void Update()
    {
        if (shieldsActive)
        {
            if(shieldActiveTimer >= 0)
            {
                shieldActiveTimer -= Time.deltaTime;
            }
            else
            {
                DeactivateShields();
            }
        }
    }

    public override void ActivateShields()
    {
        shieldActiveTimer = shieldActiveDuration;
        base.ActivateShields();
    }

    public override void DeactivateShields()
    {
        base.DeactivateShields();
    } 

    public void ReduceShields(float shieldDamage)
    {
        shieldActiveTimer -= shieldDamage;
    }
}
