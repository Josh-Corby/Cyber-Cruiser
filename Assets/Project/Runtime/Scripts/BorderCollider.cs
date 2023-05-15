using UnityEngine;

public class BorderCollider : GameBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != PlayerManagerInstance.player)
        {
            //Debug.Log(collision.name);
            if (collision.gameObject.TryGetComponent<Shield>(out var enemyShield))
            {
                IDamageable ShieldUnit = enemyShield.GetComponentInParent<IDamageable>();
                if (ShieldUnit != null)
                {
                    ShieldUnit.Destroy();
                }
            }

            else if (collision.gameObject.TryGetComponent<IDamageable>(out var damageable))
            {
                //Debug.Log("enemy culled");
                damageable.Destroy();
            }

            else if (collision.gameObject.GetComponent<Bullet>())
            {
                Destroy(collision.gameObject);
            }
        }
    }
}
