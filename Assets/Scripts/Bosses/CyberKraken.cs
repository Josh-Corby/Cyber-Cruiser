using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyberKraken : Boss, IBoss
{
    [SerializeField] private EnemySpawner _topSpawner;
    [SerializeField] private EnemySpawner _bottomSpawner;

    [SerializeField] private Transform _topGoalPosition;
    [SerializeField] private Transform _bottomGoalPosition;

    [SerializeField] private GameObject _spawnedTentacle;

    [SerializeField] private GameObject _grappleTentacleSpawnPoint;
    [SerializeField] private GameObject _grappleTentacleGoalTransform;
    [SerializeField] private GameObject _grappleTentaclePrefab;

    private bool _isAttacking;

    protected override void Awake()
    {
        _topSpawner = ESM._topTentacleSpawner;
        _bottomSpawner = ESM._bottomTentacleSpawner;
        base.Awake();
    }

    protected override void Update()
    {
        if (!_isAttacking)
        {
            if (_attackTimer > 0)
            {
                _attackTimer -= Time.deltaTime;
            }

            if (_attackTimer <= 0)
            {
                ChooseRandomAttack();
                _attackTimer = _attackCooldown;
            }
        }
    }
    //spawn tentacle at top or bottom of screen
    public void Attack1()
    {
        int i = Random.Range(0, 2);
        GameObject tentacle = i == 0 ? _topSpawner.SpawnEnemyAtRandomPosition(_spawnedTentacle) : _bottomSpawner.SpawnEnemyAtRandomPosition(_spawnedTentacle);
        if (tentacle.TryGetComponent<CyberKrakenTentacle>(out var krakenTentacle))
        {
            krakenTentacle.goalTransform = i == 0 ? _topGoalPosition : _bottomGoalPosition;
        }
    }

    //release tentacle towards the player and grab them
    public void Attack2()
    {
        Debug.Log("grapple tentacle spawned");

        GameObject grappleTentacle = Instantiate(_grappleTentaclePrefab, _grappleTentacleSpawnPoint.transform, false);
        grappleTentacle.transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, GetDirectionToPlayer());
    }

    private float GetDirectionToPlayer()
    {
        Vector2 directionToPlayer = PM.player.transform.position - _grappleTentacleSpawnPoint.transform.position;
        float rotationZ = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        //directionToPlayer.Normalize();
        return rotationZ;
    }
}
