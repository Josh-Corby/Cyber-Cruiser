using UnityEngine;
using System;

public class BossMovement : EnemyMovement
{
    public static event Action<BossMovement> OnMovePositionRequested = null;

    [SerializeField] private bool _staticMovement;
    public float speed;

    private GameObject _goalPoint;
    private bool _goalPositionReached;

    private Vector2 _movePosition;
    public Vector2 SetMovePosition
    {
        set
        {
            _movePosition = value;
        }
    }

    protected override void Awake()
    {
        _goalPoint = ESM.bossGoalPosition;
    }

    protected override void Start()
    {
        _goalPositionReached = false;
    }

    protected override void Update()
    {
        if (GM.isPaused)
        {
            return;
        }

        if (isEnemyDead)
        {
            DeathMovement();
            return;
        }

        if (!_goalPositionReached)
        {
            MoveTowardGoalPosition();
            return;
        }

        if (!_staticMovement)
        {
            if (_movePosition != null)
            {
                transform.position = Vector2.MoveTowards(transform.position, _movePosition, speed * Time.deltaTime);
            }
            
            if (Vector2.Distance(transform.position, _movePosition) <= 0.1f)
            {
                RequestNewMovePosition();
            }
        }
    }

    private void RequestNewMovePosition()
    {
        //new destination requested;
        OnMovePositionRequested(this);
    }

    private void MoveTowardGoalPosition()
    {
        transform.position = Vector2.MoveTowards(transform.position, _goalPoint.transform.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, _goalPoint.transform.position) <= 0.1)
        {
            _goalPositionReached = true;
            RequestNewMovePosition();
        }
        return;
    }

}
