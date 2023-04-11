using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyberKrakenGrappleTentacle : GameBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject _positionOffset;
    private bool _isPlayerGrappled;
    private Vector2 spawnPosition;
    private bool _moveForward;

    private void Awake()
    {
        _positionOffset = transform.parent.gameObject;
        spawnPosition = transform.position;
        _moveForward = true;
    }

    void Update()
    {
        if (!_isPlayerGrappled)
        {
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

    private void MoveForward()
    {
        _positionOffset.transform.position += transform.right * speed * Time.deltaTime;
        if (Vector2.Distance(transform.position, spawnPosition) > transform.localScale.x - 2)
        {
            _moveForward = false;
        }
    }

    private void MoveBackward()
    {
        _positionOffset.transform.position -= transform.right * speed * Time.deltaTime;

        if(_positionOffset.transform.localPosition.x < -1)
        {
            if (_isPlayerGrappled)
            {
                ResetPlayerMovement();
            }
            Destroy(gameObject);
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
