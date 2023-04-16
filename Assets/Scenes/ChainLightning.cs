using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightning : MonoBehaviour
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
            if(_currentChainBounces == 0)
            {
            transform.position += transform.right * _speed * Time.deltaTime;

            }
        }
    }


    private void FindTargetsInRange()
    {
        _objectsInRange.Clear();
        //targets in range found
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _chainRange, _enemyMask);

        if(colliders.Length == 0)
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

        if(_objectsInRange.Count <= 0)
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
        if(_chainTarget == null)
        {
            Destroy(gameObject);
        }
        StartCoroutine(ChainToTarget());
    }

    private IEnumerator ChainToTarget()
    {
        Debug.Log("Chaining to " + _chainTarget.name);
        yield return new WaitForSeconds(_chainDelay);
        if (_chainTarget != null)
        {
            transform.position = _chainTarget.transform.position;
        }
        _chainTarget = null;
    }

    //enemy collision detection
    private void OnTriggerEnter2D(Collider2D collision)
    {
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
        _objectsBouncedTo.Add(collision.gameObject);
        _currentChainBounces += 1;
        if (_currentChainBounces < _totalChainBounces)
        {
            FindTargetsInRange();
        }
        if(_currentChainBounces >= _totalChainBounces)
        {
            Destroy(gameObject);
        }
    }
}
