using UnityEngine;
using System;

public class BorderCollider : GameBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject != PM.player)
        {
            if (collision.gameObject.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.Destroy();
            }

            if (collision.gameObject.GetComponent<Bullet>())
            {
                Destroy(collision.gameObject);
            }
        }
    }
}
