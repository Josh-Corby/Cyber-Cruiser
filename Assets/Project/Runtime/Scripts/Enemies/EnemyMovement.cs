using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CyberCruiser
{
    public class EnemyMovement : GameBehaviour
    {
        private Transform _player;
        private float _upDownTimer;
        private float _homeCounter;
        private float _homeDelayCounter;
        private float _randomSinSeed;
        private bool _isBackForthDirectionUp;
        private const string UPDOWNCHECKLAYER = "UpDown";

        #region Local Movement Stats
        private EnemyMovementType _movementType;
        protected float _speed;
        private float _crashSpeed;

        protected bool _isEnemyMovingUpDown;
        protected float _upDownSpeed;
        protected float _upDownDistance;

        private bool _isEnemyMovingInSinPattern;
        private float _sinWaveFrequency;
        private float _sinWaveMagnitude;

        protected bool _isEnemySeekingPlayer;
        protected bool _isEnemySeekingPlayerOnYAxis;
        protected bool _isEnemySeekingPlayerOnXAxis;
        protected float _seekSpeed;

        protected bool _isEnemyHomingOnPlayer;
        private float _homeTurnSpeed;
        private float _homeTimeInSeconds;
        private bool _isHomingDelayed;
        private float _homeDelayTimeInSeconds;
        #endregion

        private float _startingYRotation;
        public bool IsEnemyDead { get; set; }

        protected virtual void Awake()
        {
            _player = PlayerManagerInstance.player.transform;
            _startingYRotation = transform.eulerAngles.y;
        }

        //Could definately reference local movement stats directly but at the moment I prefer this approach as it takes away a step in referencing
        public void AssignEnemyMovementInfo(EnemyMovementStats movementStats)
        {
            _movementType = movementStats.MoveType;
            _speed = movementStats.Speed;
            _crashSpeed = movementStats.SpeedWhenCrashing;

            _isEnemyMovingUpDown = _movementType == EnemyMovementType.UpDown;
            _upDownSpeed = movementStats.UpDownSpeed;
            _upDownDistance = movementStats.UpDownDistance;

            _isEnemyMovingInSinPattern = _movementType == EnemyMovementType.SinUpDown;
            _sinWaveFrequency = movementStats.SinWaveFrequency;
            _sinWaveMagnitude = movementStats.SinWaveMagnitude;

            _isEnemySeekingPlayer = _movementType == EnemyMovementType.SeekPlayer;
            _isEnemySeekingPlayerOnYAxis = movementStats.IsEnemySeekingPlayerOnYAxis;
            _isEnemySeekingPlayerOnXAxis = movementStats.IsEnemySeekingPlayerOnXAxis;
            _seekSpeed = movementStats.SeekSpeed;

            _isEnemyHomingOnPlayer = _movementType == EnemyMovementType.HomeOnPlayer;
            _homeTurnSpeed = movementStats.HomeTurnSpeed;
            _homeTimeInSeconds = movementStats.HomeTimeInSeconds;
            _isHomingDelayed = movementStats.IsHomingDelayed;
            _homeDelayTimeInSeconds = movementStats.HomeDelayTimeInSeconds;
        }

        public void ApplySpeedModifier(float speedModifier)
        {
            _speed += speedModifier;
        }

        protected virtual void Start()
        {
            CheckMovementBools();
        }

        private void CheckMovementBools()
        {
            HomingCheck();
            SinCheck();
        }

        #region Movement Checks
        private void HomingCheck()
        {
            if (_isEnemyHomingOnPlayer)
            {
                if (_isHomingDelayed)
                {
                    _homeDelayCounter = _homeDelayTimeInSeconds;
                }

                _homeCounter = _homeTimeInSeconds;
            }
        }

        private void SinCheck()
        {
            if (_isEnemyMovingInSinPattern)
            {
                _randomSinSeed = Random.Range(0f, Mathf.PI * 2f);
            }
        }
        #endregion

        protected virtual void Update()
        {
            UnitMovement();
        }

        private void UnitMovement()
        {
            if (IsEnemyDead)
            {
                MoveForward();
                DeathMovement();
                return;
            }

            if (_isEnemyHomingOnPlayer)
            {
                HomingMovement();
            }

            MoveForward();

            //Functions that need to be after general movement
            if (_isEnemyMovingUpDown)
            {
                UpDownMovement();
            }

            if (_isEnemySeekingPlayer)
            {
                SeekMovement();
            }

            if (_isEnemyMovingInSinPattern)
            {
                SinMovement();
            }
        }

        private void MoveForward()
        {
            transform.position += _speed * Time.deltaTime * transform.right;
        }

        #region Homing Movement
        private void HomingMovement()
        {
            if (_isHomingDelayed)
            {
                HomeDelay();
            }

            else
            {
                RotateTowardsPlayer();
            }
        }

        private void HomeDelay()
        {
            _homeDelayCounter -= Time.deltaTime;

            if (_homeDelayCounter <= 0)
            {
                _isHomingDelayed = false;
            }
        }

        private void RotateTowardsPlayer()
        {

            _homeCounter -= Time.deltaTime;

            if (_homeCounter <= 0)
            {
                _isEnemyHomingOnPlayer = false;
                return;
            }

            Vector2 direction = _player.transform.position - transform.position;

            float angle;
            if (Mathf.Abs(_startingYRotation) == 180)
            {
                angle = Vector2.SignedAngle(transform.right, direction);
                angle = -angle;
            }

            else
            {
                angle = MathF.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            }

            Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, angle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _homeTurnSpeed * Time.deltaTime);
        }
        #endregion

        #region Up Down Movement
        private void UpDownMovement()
        {
            _upDownTimer -= Time.deltaTime;

            if (_upDownTimer <= 0)
            {
                FlipUpDownDirection();
            }

            if (_isBackForthDirectionUp)
            {
                MoveUp();
            }

            else
            {
                MoveDown();
            }
        }


        private void MoveUp()
        {
            transform.position += new Vector3(0, _upDownSpeed * Time.deltaTime, 0);
        }

        private void MoveDown()
        {

            transform.position -= new Vector3(0, _upDownSpeed * Time.deltaTime, 0);
        }

        private void FlipUpDownDirection()
        {
            _isBackForthDirectionUp = !_isBackForthDirectionUp;
            _upDownTimer = Random.Range(2, 5);
        }
        #endregion

        #region Seek Movement
        private void SeekMovement()
        {
            if (_isEnemySeekingPlayerOnYAxis)
            {
                SeekY();
            }
            if (_isEnemySeekingPlayerOnXAxis)
            {
                SeekX();
            }
        }

        private void SeekY()
        {
            transform.position = Vector2.MoveTowards(transform.position,
                new Vector2(transform.position.x, _player.position.y), _seekSpeed * Time.deltaTime);
        }

        protected void SeekX()
        {
            transform.position = Vector2.MoveTowards(transform.position,
                new Vector2(_player.position.x, transform.position.y), _seekSpeed * Time.deltaTime);
        }
        #endregion

        #region Sin Movement
        private void SinMovement()
        {
            if (Time.timeScale == 0f)
            {
                return;
            }
            float yPos = Mathf.Sin((Time.time - _randomSinSeed) * _sinWaveFrequency) * _sinWaveMagnitude;
            transform.position = new Vector3(transform.position.x, transform.position.y + yPos, transform.position.z);
        }
        #endregion

        protected virtual void DeathMovement()
        {
            transform.position += _crashSpeed * Time.deltaTime * Vector3.down;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {

            if (collision.gameObject.CompareTag(UPDOWNCHECKLAYER))
            {
                FlipUpDownDirection();
            }

        }
    }

    [Serializable]
    public struct EnemyMovementStats
    {
        [Header("Movement Info")]
        public EnemyMovementType MoveType;
        public float Speed;
        public float SpeedWhenCrashing;

        [Header("Up/Down Settings")]
        public bool UpDownMovementPattern;
        public float UpDownSpeed;
        public float UpDownDistance;

        [Header("Sin Movement Settings")]
        public bool IsEnemyMovingInSinPattern;
        public float SinWaveFrequency;
        public float SinWaveMagnitude;

        [Header("Seek Settings")]
        public bool IsEnemySeekingPlayer;
        public bool IsEnemySeekingPlayerOnYAxis;
        public bool IsEnemySeekingPlayerOnXAxis;
        public float SeekSpeed;

        [Header("Homing Settings")]
        public bool IsEnemyHomingOnPlayer;
        public float HomeTurnSpeed;
        public float HomeTimeInSeconds;
        public bool IsHomingDelayed;
        public float HomeDelayTimeInSeconds;
    }

    public enum EnemyMovementType
    {
        Default, UpDown, SeekPlayer, SinUpDown, HomeOnPlayer
    }
}