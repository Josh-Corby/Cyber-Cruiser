using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShieldController : ShieldControllerBase, IShield
{
    [SerializeField] private int shieldActiveDuration;
    [SerializeField] private float shieldActiveTimer;
    [SerializeField] private float shieldTimerReductionOnCollision;

    private const string PLAYER_PROJECTILE_LAYER_NAME = "PlayerProjectile";
    private void Awake()
    {
        unitCollider = GetComponentInParent<Collider2D>();
    }

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

    public override void ReduceShields()
    {
        shieldActiveTimer -= shieldTimerReductionOnCollision;
    }

    public override void ReflectProjectile(Bullet bulletToReflect)
    {
        base.ReflectProjectile(bulletToReflect);
        bulletToReflect.gameObject.layer = LayerMask.NameToLayer(PLAYER_PROJECTILE_LAYER_NAME);
    }
}
