using UnityEngine;

public class EnemyMovement : GameBehaviour
{
    #region References
    private Transform _player;
    #endregion

    #region Fields
    private bool _isEnemyDead;

    #region MovementInformation
    protected float _speed;
    protected float _crashSpeed;

    [Header("Up down movement")]
    protected bool _upDownMovement;
    protected float _upDownSpeed;
    private bool _isBackForthMovingUp;
    protected float _upDownMoveDistance;

    [Header("Seek movement")]

    protected bool _seekPlayer;
    protected bool _seekPlayerY;
    protected bool _seekPlayerX;
    protected float _seekSpeed;

    [Header("Sin up down movement")]
    private bool _sinUpDownMovement;
    private float _sinFrequency;
    private float _sinMagnitude;
    private float _sinRandomSeed;

    [Header("Homing movement")]
    protected bool _homeOnPlayer;
    protected float _homeTurnSpeed;
    protected float _homeTime;
    private float homeCounter;
    protected bool _homeDelay;
    protected float _homeDelayTime;
    private float _homeDelayCounter;
    #endregion
    #endregion

    #region Properties
    public bool IsEnemyDead { get => _isEnemyDead; set => _isEnemyDead = value; }
    #endregion

    protected virtual void Awake()
    {
        _player = PM.player.transform;
    }

    public void AssignEnemyMovementInfo(EnemyScriptableObject enemyInfo)
    {
        _speed = enemyInfo.speed;
        _crashSpeed = enemyInfo.crashSpeed;
        _upDownMovement = enemyInfo.upDownMovement;
        _upDownSpeed = enemyInfo.upDownSpeed;
        _upDownMoveDistance = enemyInfo.upDownDistance;

        _seekPlayer = enemyInfo.seekPlayer;
        _seekPlayerY = enemyInfo.seekPlayerY;
        _seekPlayerX = enemyInfo.seekPlayerX;
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

    public void ApplySpeedModifier(float speedModifier)
    {
        _speed += speedModifier;
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
        if (GM.IsPaused)
        {
            return;
        }

        if (_isEnemyDead)
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

        if (_seekPlayer)
        {
            if (_seekPlayerY)
            {
                SeekY();
            }
            if (_seekPlayerX)
            {
                SeekX();
            }
        }

        if (_sinUpDownMovement)
        {
            SinUpDown();
        }
    }

    private void MoveForward()
    {
        transform.position += _speed * Time.deltaTime * transform.right;
    }

    private void UpDownMovement()
    {
        if (_isBackForthMovingUp)
        {
            if (transform.position.y < _upDownMoveDistance)
            {
                transform.position += new Vector3(0, _upDownSpeed * Time.deltaTime, 0);
            }
            else
            {
                _isBackForthMovingUp = false;
            }
        }

        if (!_isBackForthMovingUp)
        {
            if (transform.position.y > -_upDownMoveDistance)
            {
                transform.position -= new Vector3(0, _upDownSpeed * Time.deltaTime, 0);
            }
            else
            {
                _isBackForthMovingUp = true;
            }
        }
    }

    private void SeekY()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, _player.position.y), _seekSpeed * Time.deltaTime);
    }

    protected void SeekX()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(_player.position.x, transform.position.y), _seekSpeed * Time.deltaTime);
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
            _homeOnPlayer = false;
            return;
        }


        Vector3 direction = _player.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        //Quaternion targetRotation = Quaternion.Euler(0f, _startRotation.y, angle);

        //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _homeTurnSpeed * Time.deltaTime);
    }

    protected virtual void DeathMovement()
    {
        transform.position += _crashSpeed * Time.deltaTime * Vector3.down;
    }
}
