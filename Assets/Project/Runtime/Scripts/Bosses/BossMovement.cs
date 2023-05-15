using System;
using UnityEngine;

public enum BossMovementType
{
    Static, UpDown, Free
}

public class BossMovement : EnemyMovement
{
    public static event Action<BossMovement, BossMovementType> OnMovePositionRequested = null;

    private GameObject _goalPoint;
    private bool _goalPositionReached;
    [SerializeField] private BossMovementType _moveType;
    #region Properties
    private Vector2 _movePosition;
    public Vector2 SetMovePosition
    {
        set
        {
            _movePosition = value;
        }
    }
    #endregion



    protected override void Awake()
    {
        _goalPoint = EnemySpawnerManagerInstance.bossGoalPosition;
    }

    protected override void Start()
    {
        _goalPositionReached = false;
    }

    protected override void Update()
    {
        if (IsEnemyDead)
        {
            DeathMovement();
            return;
        }

        if (!_goalPositionReached)
        {
            MoveTowardGoalPosition();
            return;
        }

        MoveTypeCheck();
    }

    private void MoveTypeCheck()
    {
        if (_moveType == BossMovementType.Static)
        {
            return;
        }

        if (_movePosition != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, _movePosition, _speed * Time.deltaTime);
        }

        if (Vector2.Distance(transform.position, _movePosition) <= 0.1f)
        {
            RequestNewMovePosition();
        }
    }

    private void RequestNewMovePosition()
    {
        OnMovePositionRequested(this, _moveType);
    }

    private void MoveTowardGoalPosition()
    {
        transform.position = Vector2.MoveTowards(transform.position, _goalPoint.transform.position, _speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, _goalPoint.transform.position) <= 0.1)
        {
            _goalPositionReached = true;
            RequestNewMovePosition();
        }
        return;
    }

}
