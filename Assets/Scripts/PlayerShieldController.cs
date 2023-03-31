using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShieldController : ShieldControllerBase, IShield
{
    private const string PLAYER_PROJECTILE_LAYER_NAME = "PlayerProjectile";


    [SerializeField] private int _shieldActiveDuration;
    [SerializeField] private float _shieldActiveTimer;

    public int ShieldActiveDuration
    {
        get
        {
            return _shieldActiveDuration;
        }
        set
        {
            _shieldActiveDuration = value;
        }
    }

    public float ShieldActiveTimer
    {
        get
        {
            return _shieldActiveTimer;
        }

        set
        {
            _shieldActiveTimer = value;
        }
    }

    private void Update()
    {
        if (shieldsActive)
        {
            if(ShieldActiveTimer >= 0)
            {
                ShieldActiveTimer -= Time.deltaTime;
            }
            else
            {
                DeactivateShields();
            }
        }
    }

    public override void ProcessCollision(GameObject collider)
    {
        if (collider.TryGetComponent<Boss>(out var boss))
        {
            return;
        }

        base.ProcessCollision(collider);
    }

    public override void ActivateShields()
    {
        ShieldActiveTimer = ShieldActiveDuration;
        base.ActivateShields();
    }


    public override void ReduceShields(float damage)
    {
        ShieldActiveTimer -= damage;
    }

    public override void ReflectProjectile(Bullet bulletToReflect)
    {
        base.ReflectProjectile(bulletToReflect);
        bulletToReflect.gameObject.layer = LayerMask.NameToLayer(PLAYER_PROJECTILE_LAYER_NAME);
    }
}
