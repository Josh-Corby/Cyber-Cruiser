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
        foreach (GameObject enemy in _enemiesInHomingRange)
        {
            _distanceToTarget = Vector2.Distance(transform.position, enemy.transform.position);
            if (_distanceToTarget < minDistance)
            {
                minDistance = _distanceToTarget;
                _homingTarget = enemy;
            }
        }
        bullet.homingTarget = _homingTarget;
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
    }
}
