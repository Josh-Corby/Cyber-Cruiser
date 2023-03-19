using UnityEngine;

public class EnemyMovement : GameBehaviour
{
    public enum MovementTypes
    {
        Forward, None
    }

    public MovementTypes moveType;
    private Transform player;
    public float speed;
    [SerializeField] private bool bossEnemy;
    private bool goalPositionReached;
    [HideInInspector] public Vector2 direction;  

    [Header("UpDown Variables")]
    public bool upDown;
    //if true backforth will move up, otherwise move down
    [SerializeField] private float upDownSpeed;
    private bool isBackForthMovingUp;
    [SerializeField] private int upDownMoveDistance;
    private readonly float yUp = 4.5f;
    private readonly float yDown = -4.5f;

    [Header("Seek Variables")]
    public bool seekPlayerY;
    public float seekSpeed;

    [Header("Sin Variables")]
    [SerializeField] private bool sinUpDown;
    [SerializeField] private float frequency;
    [SerializeField] private float magnitude;

    [Header("Homing variables")]
    [SerializeField] private bool homeOnPlayer;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float homeTime;
    private float homeCounter;

    //offset for random starting sin value
    private float startTime;


    private GameObject goalPoint;

    protected void Awake()
    {
        player = PM.player.transform;
        goalPoint = ESM.bossGoalPosition;
    }

    protected virtual void Start()
    {
        if (homeOnPlayer)
        {
            homeCounter = homeTime;
        }

        startTime = Random.Range(0f, Mathf.PI * 2f);

        goalPositionReached = false;
    }

    protected virtual void Update()
    {
        if (bossEnemy)
        {
            if (!goalPositionReached)
            {
                MoveTowardGoalPosition();
            }
        }

        if (homeOnPlayer)
        {
            RotateTowardsPlayer();
        }

        if (moveType == MovementTypes.Forward)
        {
            MoveForward();
        }

        if (upDown)
        {
            UpDownMovement();
            return;
        }

        if (seekPlayerY)
        {
            SeekPlayerY();
            return;
        }

        if (sinUpDown)
        {
            SinUpDown();
            return;
        }
    }

    private void MoveTowardGoalPosition()
    {
        transform.position = Vector2.MoveTowards(transform.position, goalPoint.transform.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, goalPoint.transform.position) <= 0.1)
        {
            goalPositionReached = true;
        }
        return;
    }

    private void MoveForward()
    {
        transform.position += speed * Time.deltaTime * transform.up;
    }

    private void UpDownMovement()
    {
        if (isBackForthMovingUp)
        {
            if (transform.position.y < yUp)
            {
                transform.position += new Vector3(0, upDownSpeed * Time.deltaTime, 0);
            }
            else
            {
                isBackForthMovingUp = false;
            }
        }

        if (!isBackForthMovingUp)
        {
            if (transform.position.y > yDown)
            {
                transform.position -= new Vector3(0, upDownSpeed * Time.deltaTime, 0);
            }
            else
            {
                isBackForthMovingUp = true;
            }
        }
    }

    private void SeekPlayerY()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, player.position.y), seekSpeed * Time.deltaTime);
    }

    private void SinUpDown()
    {
        float yPos = Mathf.Sin((Time.time - startTime) * frequency) * magnitude;
        transform.position = new Vector3(transform.position.x, transform.position.y + yPos, transform.position.z);
    }

    private void RotateTowardsPlayer()
    {
        homeCounter -= Time.deltaTime;

        if (homeCounter <= 0)
        {
            direction = transform.up;
            homeOnPlayer = false;
            Debug.Log("No longer rotating towards player");
            return;
        }

        Vector3 vectorToPlayer = player.position - transform.position;
        float angle = Mathf.Atan2(vectorToPlayer.y, vectorToPlayer.x) * Mathf.Rad2Deg - 90;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, turnSpeed * Time.deltaTime);
        direction = transform.up;
    }
}
