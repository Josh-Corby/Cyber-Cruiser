using System.Collections.Generic;
using UnityEngine;

namespace CyberCruiser
{
    public class BulletHoming : MonoBehaviour
    {
        private Bullet bullet;
        [SerializeField] private List<GameObject> _enemiesInHomingRange = new();
        private GameObject _homingTarget;
        private float _distanceToTarget;

        private void Awake()
        {
            bullet = GetComponentInParent<Bullet>();
        }

        private void Update()
        {
            if (bullet.HomingTarget == null)
            {
                FindClosestEnemy();

            }
        }

        private void FindClosestEnemy()
        {
            _homingTarget = null;
            float minDistance = float.MaxValue;

            for (int i = 0; i < _enemiesInHomingRange.Count; i++)
            {
                if (_enemiesInHomingRange[i] == null)
                {
                    _enemiesInHomingRange.Remove(_enemiesInHomingRange[i]);
                    continue;
                }

                _distanceToTarget = Vector2.Distance(transform.position, _enemiesInHomingRange[i].transform.position);

                if (_distanceToTarget < minDistance)
                {
                    minDistance = _distanceToTarget;
                    _homingTarget = _enemiesInHomingRange[i];
                }
            }

            bullet.AssignHomingRotation(transform.parent.rotation * Quaternion.Euler(0, 0, 45));
            bullet.HomingTarget = _homingTarget;
        }

        public void ClearEnemiesInRange()
        {
            _enemiesInHomingRange.Clear();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent<Shield>(out var shield))
            {
                if (!collision.GetComponentInParent<Enemy>())
                {
                    return;
                }
            }

            else if (!collision.TryGetComponent<Enemy>(out var enemy))
            {
                return;
            }

            _enemiesInHomingRange.Add(collision.gameObject);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.TryGetComponent<Enemy>(out var enemy))
            {
                if (bullet.HomingTarget = enemy.gameObject)
                {
                    bullet.HomingTarget = null;
                }
                _enemiesInHomingRange.Remove(enemy.gameObject);
            }

            if (collision.TryGetComponent<Shield>(out var shield))
            {
                if (bullet.HomingTarget = shield.gameObject)
                {
                    bullet.HomingTarget = null;
                }
                _enemiesInHomingRange.Remove(shield.gameObject);
            }
        }
    }
}