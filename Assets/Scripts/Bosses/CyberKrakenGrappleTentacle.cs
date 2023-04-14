using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyberKrakenGrappleTentacle : CyberKrakenTentacle
{
    private bool _isPlayerGrappled;

    private void Update()
    {
        TentacleMovement();
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
        PM.player.transform.position = _positionOffset.transform.position;
    }

    private void ResetPlayerMovement()
    {
        PM.playerShipController.ControlsEnabled = true;
    }

    private void MoveBackward()
    {
        _positionOffset.transform.position -= transform.right * speed * Time.deltaTime;

        if (_positionOffset.transform.localPosition.x < -1)
        {
            if (_isPlayerGrappled)
            {
                ResetPlayerMovement();
            }
            Destroy(gameObject);
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
