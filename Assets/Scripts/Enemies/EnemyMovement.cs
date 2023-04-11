using UnityEngine;

public class EnemyMovement : GameBehaviour
{
    private Transform player;

    [HideInInspector] public bool isEnemyDead;
    [HideInInspector] public Vector2 direction;

    #region MovementInformation
    [HideInInspector] public float _speed;

    [Header("Up down movement")]
    [SerializeField] protected bool _upDownMovement;
    [SerializeField] protected float _upDownSpeed;
    [SerializeField] private bool isBackForthMovingUp;
    [SerializeField] protected float _upDownMoveDistance;

    [Header("Seek movement")]
    [SerializeField] protected bool _seekPlayerY;
    [SerializeField] protected float _seekSpeed;

    [Header("Sin up down movement")]
    [SerializeField] private bool _sinUpDownMovement;
    [SerializeField] private float _sinFrequency;
    [SerializeField] private float _sinMagnitude;
    [SerializeField] private float _sinRandomSeed;

    [Header("Homing movement")]
    [SerializeField] protected bool _homeOnPlayer;
    [SerializeField] protected float _homeTurnSpeed;
    [SerializeField] protected float _homeTime;
    [SerializeField] private float homeCounter;
    [SerializeField] protected bool _homeDelay;
    [SerializeField] protected float _homeDelayTime;
    [SerializeField] private float _homeDelayCounter;
    #endregion

    protected virtual void Awake()
    {
        player = PM.player.transform;
    }

    public void AssignEnemyMovementInfo(EnemyScriptableObject enemyInfo)
    {
        _speed = enemyInfo.speed;
        _upDownMovement = enemyInfo.upDownMovement;
        _upDownSpeed = enemyInfo.upDownSpeed;
        _upDownMoveDistance = enemyInfo.upDownDistance;

        _seekPlayerY = enemyInfo.seekPlayerY;
        _seekSpeed = enemyInfo.seekSpeed;

        _sinUpDownMovement = enemyInfo.sinUpDownMovement;
        _sinFrequency = enemyInfo.sinFrequency;
        _sinMagnitude = enemyInfo.sinMagnitude;

        _homeOnPlayer = enemyInfo.homeOnPlayer;
        _homeTurnSpeed = enemyInfo.homeTurnSpeed;
        _homeTime = enemyInfo.homeTime;
        _homeDelay = enemyInfo.homeDelay;
        _homeDelayTime = enemyInfo.homeDelayTime;
    }

    protected virtual void Start()
    {
        if (_homeOnPlayer)
        {
            if (_homeDelay)
            {
                _homeDelayCounter = _homeDelayTime;
            }

            homeCounter = _homeTime;
        }
        if (_sinUpDownMovement)
        {
            _sinRandomSeed = Random.Range(0f, Mathf.PI * 2f);
        }
    }

    protected virtual void Update()
    {
        UnitMovement();
    } 

    private void UnitMovement()
    {
        if (GM.isPaused)
        {
            return;
        }

        if (isEnemyDead)
        {
            MoveForward();
            DeathMovement();
            return;
        }

        if (_homeOnPlayer)
        {
            if (_homeDelay)
            {
                HomeDelay();
            }

            if (!_homeDelay)
            {
                RotateTowardsPlayer();
            }
        }

        MoveForward();

        if (_upDownMovement)
        {
            UpDownMovement();
        }

        if (_seekPlayerY)
        {
            SeekPlayerY();
        }

        if (_sinUpDownMovement)
        {
            SinUpDown();
        }
    }

    private void MoveForward()
    {
        transform.position += _speed * Time.deltaTime * transform.up;
    }

    private void UpDownMovement()
    {
        if (isBackForthMovingUp)
        {
            if (transform.position.y < _upDownMoveDistance)
            {
                transform.position += new Vector3(0, _upDownSpeed * Time.deltaTime, 0);
            }
            else
            {
                isBackForthMovingUp = false;
            }
        }

        if (!isBackForthMovingUp)
        {
            if (transform.position.y > -_upDownMoveDistance)
            {
                transform.position -= new Vector3(0, _upDownSpeed * Time.deltaTime, 0);
            }
            else
            {
                isBackForthMovingUp = true;
            }
        }
    }

    private void SeekPlayerY()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, player.position.y), _seekSpeed * Time.deltaTime);
    }

    private void SinUpDown()
    {
        float yPos = Mathf.Sin((Time.time - _sinRandomSeed) * _sinFrequency) * _sinMagnitude;
        transform.position = new Vector3(transform.position.x, transform.position.y + yPos, transform.position.z);
    }

    private void HomeDelay()
    {
        _homeDelayCounter -= Time.deltaTime;

        if (_homeDelayCounter <= 0)
        {
            _homeDelay = false;
        }
    }

    private void RotateTowardsPlayer()
    {

        homeCounter -= Time.deltaTime;

        if (homeCounter <= 0)
        {
            direction = transform.up;
            _homeOnPlayer = false;
            //Debug.Log("No longer rotating towards player");
            return;
        }

        Vector3 vectorToPlayer = player.position - transform.position;
        float angle = Mathf.Atan2(vectorToPlayer.y, vectorToPlayer.x) * Mathf.Rad2Deg - 90;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, _homeTurnSpeed * Time.deltaTime);
        direction = transform.up;
    }

    protected virtual void DeathMovement()
    {
        transform.position += _speed * Time.deltaTime * Vector3.down;
    }
}
