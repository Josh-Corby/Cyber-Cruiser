using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (bullet.homingTarget == null)
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
        bullet.homingTarget = _homingTarget;
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
            if (bullet.homingTarget = enemy.gameObject)
            {
                bullet.homingTarget = null;
            }
            _enemiesInHomingRange.Remove(enemy.gameObject);
        }

        if(collision.TryGetComponent<Shield>(out var shield))
        {
            if (bullet.homingTarget = shield.gameObject)
            {
                bullet.homingTarget = null;
            }
            _enemiesInHomingRange.Remove(shield.gameObject);
        }
    }
}
