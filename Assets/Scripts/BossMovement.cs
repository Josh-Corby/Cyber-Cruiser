using UnityEngine;
using System;

public class BossMovement : GameBehaviour
{
    public static event Action<BossMovement> OnMovePositionRequested = null;

    [SerializeField] private bool staticMovement;
    public float speed;

    private GameObject goalPoint;
    private bool goalPositionReached;

    private Vector2 _movePosition;
    public Vector2 SetMovePosition
    {
        set
        {
            _movePosition = value;
        }
    }

    private void Awake()
    {
        goalPoint = ESM.bossGoalPosition;
    }
    private void Start()
    {
        goalPositionReached = false;
    }

    private void Update()
    {
        if (!goalPositionReached)
        {
            MoveTowardGoalPosition();
            return;
        }

        if (!staticMovement)
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
        transform.position = Vector2.MoveTowards(transform.position, goalPoint.transform.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, goalPoint.transform.position) <= 0.1)
        {
            goalPositionReached = true;
            RequestNewMovePosition();
        }
        return;
    }

}
