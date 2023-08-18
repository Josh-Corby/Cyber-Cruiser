using UnityEngine;

namespace CyberCruiser
{
    public class BorderCollider : GameBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject != PlayerManagerInstance.player)
            {
                //Debug.Log(collision.name);
                if (collision.gameObject.TryGetComponent<Shield>(out var enemyShield))
                {
                    Enemy ShieldUnit = enemyShield.GetComponentInParent<Enemy>();
                    if (ShieldUnit != null)
                    {
                        ShieldUnit.Cull();
                    }
                }

                else if (collision.gameObject.TryGetComponent<Enemy>(out var enemy))
                {
                    enemy.Cull();
                }

                else if (collision.gameObject.GetComponent<Bullet>())
                {
                    Destroy(collision.gameObject);
                }
            }
        }
    }
}