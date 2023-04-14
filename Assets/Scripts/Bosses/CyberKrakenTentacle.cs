using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyberKrakenTentacle : GameBehaviour
{
    [SerializeField] protected float speed;
    [SerializeField] protected GameObject _positionOffset;

    [SerializeField] protected Vector2 spawnPosition;
    [SerializeField] protected bool _moveForward;

    [SerializeField] protected bool _isWaiting;
    [SerializeField] protected float _waitTime;
    [SerializeField] protected float _waitTimer;

    protected void Awake()
    {
        _positionOffset = transform.parent.gameObject;
        spawnPosition = _positionOffset.transform.position;
        _moveForward = true;
        _isWaiting = false;
    }

    private void Update()
    {
        TentacleMovement();
    }

    protected virtual void TentacleMovement()
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

    protected void MoveForward()
    {
        _positionOffset.transform.position += transform.right * speed * Time.deltaTime;
        if (Vector2.Distance(_positionOffset.transform.position, spawnPosition) > transform.localScale.x)
        {
            StartWaiting();
            _moveForward = false;
        }
    }

    private void MoveBackward()
    {
        _positionOffset.transform.position -= transform.right * speed * Time.deltaTime;

        if (Vector2.Distance(_positionOffset.transform.position, spawnPosition) <0.5f)
        {
            Destroy(gameObject);
        }
    }

    protected void StartWaiting()
    {
        Debug.Log("Start waiting");
        _isWaiting = true;
        _waitTimer = _waitTime;
    }

    protected void WaitTimer()
    {
        _waitTimer -= Time.deltaTime;
        if (_waitTimer <= 0)
        {
            _isWaiting = false;
        }
    } 
}
