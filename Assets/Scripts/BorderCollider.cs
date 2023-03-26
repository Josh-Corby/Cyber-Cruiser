using UnityEngine;
using System;

public class BorderCollider : GameBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != PM.player)
        {
            if (collision.gameObject.TryGetComponent<IDamageable>(out var damageable))
            {
                //Debug.Log("enemy culled");
                damageable.Destroy();
            }

            if (collision.gameObject.GetComponent<Bullet>())
            {
                Destroy(collision.gameObject);
            }
        }
    }
}
