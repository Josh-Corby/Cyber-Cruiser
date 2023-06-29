using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberCruiser
{
    public class UpDownBorder : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            EnemyMovement movement;
            if (collision.gameObject.TryGetComponent<EnemyMovement>(out movement))
            {
                if (movement.IsEnemyMovingUpDown)
                {
                    movement.FlipUpDownDirection();
                }
            }

            else
            {
                movement = collision.gameObject.GetComponentInParent<EnemyMovement>();
                if(movement != null)
                {
                    if (movement.IsEnemyMovingUpDown)
                    {
                        movement.FlipUpDownDirection();
                    }
                }
            }
        }
    }
}
