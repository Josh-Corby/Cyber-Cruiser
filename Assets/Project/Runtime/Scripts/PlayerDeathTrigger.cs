using System;
using UnityEngine;

namespace CyberCruiser
{
    public class PlayerDeathTrigger : MonoBehaviour
    {
        public static event Action OnPlayerDeadOffScreen = null;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<PlayerManager>())
            {
                Debug.Log("Player off screen");
                OnPlayerDeadOffScreen?.Invoke();
            }
        }
    }
}
