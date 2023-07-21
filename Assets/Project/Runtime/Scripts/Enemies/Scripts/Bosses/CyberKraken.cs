using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CyberCruiser
{
    namespace Enemies
    {
        public class CyberKraken : Boss, IBoss
        {
            [SerializeField] private EnemySpawner _topSpawner;
            [SerializeField] private EnemySpawner _bottomSpawner;

            [SerializeField] private GameObject _spawnedTentaclePrefab;

            [SerializeField] private GameObject _grappleTentaclePrefab;
            [SerializeField] private GameObject _grappleTentacleSpawnPoint;

            private List<GameObject> _tentacles = new();

            protected override void Awake()
            {
                _topSpawner = EnemySpawnerManagerInstance._topSpawner;
                _bottomSpawner = EnemySpawnerManagerInstance._bottomSpawner;
                base.Awake();
            }

            //spawn tentacle at top or bottom of screen
            public void Attack1()
            {
                int i = Random.Range(0, 2);
                if (i == 0)
                {
                    GameObject topTentacle = Instantiate(_spawnedTentaclePrefab, _topSpawner.GetRandomSpawnPosition(), _topSpawner.transform.rotation);
                    topTentacle.transform.parent = gameObject.transform;
                    _tentacles.Add(topTentacle);
                }
                if (i == 1)
                {
                    GameObject bottomTentacle = Instantiate(_spawnedTentaclePrefab, _bottomSpawner.GetRandomSpawnPosition(), _bottomSpawner.transform.rotation);
                    bottomTentacle.transform.parent = gameObject.transform;
                    _tentacles.Add(bottomTentacle);
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
                Vector2 directionToPlayer = PlayerManagerInstance.player.transform.position - _grappleTentacleSpawnPoint.transform.position;
                float rotationZ = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
                //directionToPlayer.Normalize();
                return rotationZ;
            }

            protected override void Crash()
            {
                for (int i = _tentacles.Count; i < 0; i--)
                {
                    Destroy(_tentacles[i]);
                }

                _tentacles.Clear();
                base.Crash();
            }
        } 
    }
}