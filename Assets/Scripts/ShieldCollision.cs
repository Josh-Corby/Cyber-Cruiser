using UnityEngine;
using System;

public class ShieldCollision : MonoBehaviour
{
    private ShieldControllerBase shieldcontroller;

    private void Awake()
    {
        shieldcontroller = GetComponentInParent<ShieldControllerBase>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        ProcessCollision(collision.gameObject);
    }

    private void ProcessCollision(GameObject collider)
    {
        if(collider.TryGetComponent<Boss>(out var boss))
        {
            return;
        }

        else if (collider.TryGetComponent<IDamageable>(out var damageable))
        {
            //Debug.Log(collider.name);
            damageable.Destroy();
            shieldcontroller.DeactivateShields();
        }

        else if (collider.TryGetComponent<IShield>(out var shield))
        {
            shield.DeactivateShields();
            shieldcontroller.DeactivateShields();
        }

        else if (collider.TryGetComponent<Bullet>(out var bullet))
        {
            if (shieldcontroller.reflectorShield)
            {
                shieldcontroller.ReflectProjectile(collider);
            }
            else
            {
                shieldcontroller.ReduceShields();
                Destroy(bullet.gameObject);
            }
        }

    }
}
