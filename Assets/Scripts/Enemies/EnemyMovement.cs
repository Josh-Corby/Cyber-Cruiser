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
    private bool isEnemyDead;
    [HideInInspector] public Vector2 direction;

    [Header("UpDown Variables")]
    public bool upDown;
    //if true backforth will move up, otherwise move down
    [SerializeField] private float upDownSpeed;
    private bool isBackForthMovingUp;
    [SerializeField] private int upDownMoveDistance;
   

    [Header("Seek Variables")]
    public bool seekPlayerY;
    public float seekSpeed;

    [Header("Sin Variables")]
    [SerializeField] private bool sinUpDown;
    [SerializeField] private float frequency;
    [SerializeField] private float magnitude;

    [Header("Homing variables")]
    [SerializeField] protected bool homeOnPlayer;
    [SerializeField] protected float turnSpeed;
    [SerializeField] protected float homeTime;
    private float homeCounter;

    [SerializeField] protected bool homeDelay;
    [SerializeField] protected float homeDelayTime;
    private float homeDelayCounter;

    //offset for random starting sin value
    private float startTime;

    protected virtual void Awake()
    {
        player = PM.player.transform;
    }

    protected virtual void Start()
    {
        if (homeOnPlayer)
        {
            if (homeDelay)
            {
                homeDelayCounter = homeDelayTime;
            }

            homeCounter = homeTime;
        }
        startTime = Random.Range(0f, Mathf.PI * 2f);  
    }

    protected virtual void Update()
    {
        if (GM.isPaused)
        {
            return;
        }



        if (homeOnPlayer)
        {
            if (homeDelay)
            {
                HomeDelay();
            }

            if (!homeDelay)
            {
                RotateTowardsPlayer();
            }
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

   

    private void MoveForward()
    {
        transform.position += speed * Time.deltaTime * transform.up;
    }

    private void UpDownMovement()
    {
        if (isBackForthMovingUp)
        {
            if (transform.position.y < upDownMoveDistance)
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
            if (transform.position.y > -upDownMoveDistance)
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

    private void HomeDelay()
    {
        homeDelayCounter -= Time.deltaTime;

        if (homeDelayCounter <= 0)
        {
            homeDelay = false;
        }
    }

    private void RotateTowardsPlayer()
    {

        homeCounter -= Time.deltaTime;

        if (homeCounter <= 0)
        {
            direction = transform.up;
            homeOnPlayer = false;
            //Debug.Log("No longer rotating towards player");
            return;
        }

        Vector3 vectorToPlayer = player.position - transform.position;
        float angle = Mathf.Atan2(vectorToPlayer.y, vectorToPlayer.x) * Mathf.Rad2Deg - 90;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, turnSpeed * Time.deltaTime);
        direction = transform.up;
    }
}
