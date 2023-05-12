using UnityEngine;

public class CyberKrakenTentacle : GameBehaviour
{
    [SerializeField] protected float speed;

    [SerializeField] protected Vector2 spawnPosition;
    [SerializeField] protected bool _moveForward;

    [SerializeField] protected bool _isWaiting;
    [SerializeField] protected float _waitTime;
    [SerializeField] protected float _waitTimer;

    private BoxCollider2D _col;

    protected void Awake()
    {
        spawnPosition = transform.parent.position;
        _moveForward = true;
        _isWaiting = false;
        _col = GetComponent<BoxCollider2D>();
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
        transform.parent.position += transform.right * speed * Time.deltaTime;
        if (Vector2.Distance(transform.parent.position, spawnPosition) > _col.size.x)
        {
            StartWaiting();
            _moveForward = false;
        }
    }

    private void MoveBackward()
    {
        transform.parent.position -= transform.right * speed * Time.deltaTime;

        if (Vector2.Distance(transform.parent.position, spawnPosition) < 0.5f)
        {
            Destroy(transform.parent.gameObject);
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
