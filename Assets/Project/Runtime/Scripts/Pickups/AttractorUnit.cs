using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberCruiser
{
    public class AttractorUnit : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject.TryGetComponent<Pickup>(out var pickup))
            {
                pickup.SetAttractorTarget(gameObject);
            }
        }
    }
}
