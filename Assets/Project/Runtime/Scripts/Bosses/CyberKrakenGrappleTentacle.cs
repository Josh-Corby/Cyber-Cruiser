using System;
using UnityEngine;

namespace CyberCruiser
{
    public class CyberKrakenGrappleTentacle : CyberKrakenTentacle
    {
        private bool _isPlayerGrappled;

        public static event Action OnGrappleEnd = null;

        private void Update()
        {
            TentacleMovement();
        }

        private void OnDisable()
        {
            if (_isPlayerGrappled)
            {
                //if (!PlayerManagerInstance.isDead)
                //{
                //    ResetPlayerMovement();
                //}
            }
        }

        protected override void TentacleMovement()
        {
            if (!_isPlayerGrappled)
            {
                if (_isWaiting)
                {
                    WaitTimer();
                    return;
                }
                if (_moveForward)
                {
                    MoveForward();
                }
                else
                {
                    MoveBackward();

                }
            }
            else
            {
                MoveBackward();
                PullPlayer();
            }
        }

        private void PullPlayer()
        {
            PlayerManagerInstance.player.transform.position = transform.parent.position;
        }

        private void ResetPlayerMovement()
        {
            OnGrappleEnd?.Invoke();
        }

        private void MoveBackward()
        {
            transform.parent.position -= transform.right * speed * Time.deltaTime;

            if (Vector2.Distance(transform.parent.position, spawnPosition) < 0.5f)
            {
                if (_isPlayerGrappled)
                {
                    ResetPlayerMovement();
                }
                Destroy(transform.parent.gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.TryGetComponent<PlayerShipController>(out var player))
            {
                Debug.Log("Player grappled");

                player.ControlsEnabled = false;
                _isPlayerGrappled = true;

            }
        }
    }
}