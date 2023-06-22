using CyberCruiser;
using System.Collections.Generic;
using UnityEngine;

namespace CyberCruiser
{
    public class ChainLightning : GameBehaviour
    {
        [SerializeField] private int _totalChainBounces;
        [SerializeField] private int _currentChainBounces;
        [SerializeField] private int _chainDamage;
        [SerializeField] private float _chainRange;
        [SerializeField] private List<GameObject> _objectsInRange = new();
        [SerializeField] private List<GameObject> _objectsBouncedTo = new();
        [SerializeField] private LayerMask _enemyMask;

        private float _smallestDistance;
        private float _distanceToTarget;
        [SerializeField] private GameObject _chainTarget;
        [SerializeField] private float _speed;
        [SerializeField] private float _chainSpeed;
        [SerializeField] private float _chainDelay;


        private void OnEnable()
        {
            Enemy.OnEnemyAliveStateChange += RemoveEnemyBuffer;
        }

        private void OnDisable()
        {
            Enemy.OnEnemyAliveStateChange -= RemoveEnemyBuffer;
        }

        private void RemoveEnemyBuffer(GameObject enemy, bool val)
        {
            RemoveEnemy(enemy);
        }

        private void RemoveEnemy(GameObject enemy)
        {
            if (_objectsInRange.Contains(enemy))
            {
                _objectsInRange.Remove(enemy);
            }
            if (_objectsBouncedTo.Contains(enemy))
            {
                _objectsBouncedTo.Remove(enemy);
            }
        }


        private void Start()
        {
            _chainTarget = null;
            _currentChainBounces = 0;
        }
        private void Update()
        {
            if (_chainTarget == null)
            {
                if (_currentChainBounces == 0)
                {
                    transform.position += transform.right * _speed * Time.deltaTime;
                }
            }

            if (_chainTarget != null)
            {
                Vector3 directionToTarget = (_chainTarget.transform.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(directionToTarget, transform.up);

                transform.right = directionToTarget;
                //transform.LookAt(_chainTarget.transform.position);
                transform.position = Vector2.MoveTowards(transform.position, _chainTarget.transform.position, _chainSpeed * Time.deltaTime);
            }
        }


        private void FindTargetsInRange()
        {
            _objectsInRange.Clear();
            //targets in range found
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _chainRange, _enemyMask);

            if (colliders.Length == 0)
            {
                Destroy(gameObject);
            }

            foreach (Collider2D collider in colliders)
            {
                if (!_objectsBouncedTo.Contains(collider.gameObject))
                {
                    _objectsInRange.Add(collider.gameObject);
                }
            }

            if (_objectsInRange.Count <= 0)
            {
                Destroy(gameObject);
            }

            if (_objectsInRange.Count > 0)
            {
                FindClosestTarget();
            }
        }

        private void FindClosestTarget()
        {
            _smallestDistance = _chainRange;
            foreach (GameObject enemy in _objectsInRange)
            {
                _distanceToTarget = Vector2.Distance(transform.position, enemy.transform.position);
                if (_distanceToTarget < _smallestDistance)
                {
                    _smallestDistance = _distanceToTarget;
                    _chainTarget = enemy;
                }
            }
            if (_chainTarget == null)
            {
                Destroy(gameObject);
            }
        }

        //enemy collision detection
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_chainTarget != null)
            {
                if (collision.gameObject != _chainTarget)
                {
                    return;
                }
            }

            if (collision.TryGetComponent<Shield>(out var shield))
            {
                if (collision.gameObject.GetComponentInParent<Enemy>())
                {
                    shield._shieldController.ReduceShields(_chainDamage);
                }
            }

            else if (collision.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.Damage(_chainDamage);
            }

            else
            {
                Destroy(gameObject);
            }

            _objectsBouncedTo.Add(collision.gameObject);
            _currentChainBounces += 1;
            if (_currentChainBounces < _totalChainBounces)
            {
                FindTargetsInRange();
            }

            if (_currentChainBounces >= _totalChainBounces)
            {
                Destroy(gameObject);
            }
        }
    }
}