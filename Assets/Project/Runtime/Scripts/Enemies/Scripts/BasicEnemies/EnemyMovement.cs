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

        [SerializeField] private BoolReference _isPlayerInvisible;
        [SerializeField] private BoolReference _isTimeStopped;

        #region Local Movement Stats
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

        public bool IsEnemyMovingUpDown { get => _isEnemyMovingUpDown; }

        public bool IsPlayerInvisible { get => _isPlayerInvisible.Value; }

        public bool IsTimeStopped { get => _isTimeStopped.Value; }

        protected virtual void Awake()
        {
            _player = PlayerManagerInstance.player.transform;
            _startingYRotation = transform.eulerAngles.y;
        }

        protected virtual void Start()
        {
            CheckMovementBools();
        }

        protected virtual void Update()
        {
            if (_isTimeStopped.Value)
            {
                return;
            }

            UnitMovement();
        }

        //Could definitely reference local movement stats directly but at the moment I prefer this approach as it takes away a step in referencing
        public void AssignEnemyMovementInfo(EnemyMovementStats movementStats)
        {
            _speed = movementStats.Speed;
            _crashSpeed = movementStats.SpeedWhenCrashing;

            _isEnemyMovingUpDown = movementStats.UpDownMovementPattern;
            if(_isEnemyMovingUpDown)
            {
                _upDownSpeed = movementStats.UpDownSpeed;
                _upDownDistance = movementStats.UpDownDistance;
            }          

            _isEnemyMovingInSinPattern = movementStats.IsEnemyMovingInSinPattern;
            if(_isEnemyMovingInSinPattern)
            {
                _sinWaveFrequency = movementStats.SinWaveFrequency;
                _sinWaveMagnitude = movementStats.SinWaveMagnitude;
            }
           
            _isEnemySeekingPlayer = movementStats.IsEnemySeekingPlayer;
            if (_isEnemySeekingPlayer)
            {
                _isEnemySeekingPlayerOnYAxis = movementStats.IsEnemySeekingPlayerOnYAxis;
                _isEnemySeekingPlayerOnXAxis = movementStats.IsEnemySeekingPlayerOnXAxis;
                _seekSpeed = movementStats.SeekSpeed;
            }         

            _isEnemyHomingOnPlayer = movementStats.IsEnemyHomingOnPlayer;
            if (_isEnemyHomingOnPlayer)
            {
                _homeTurnSpeed = movementStats.HomeTurnSpeed;
                _homeTimeInSeconds = movementStats.HomeTimeInSeconds;
                _isHomingDelayed = movementStats.IsHomingDelayed;
                _homeDelayTimeInSeconds = movementStats.HomeDelayTimeInSeconds;
            }          
        }

        public void ApplySpeedModifier(float speedModifier)
        {
            _speed += speedModifier;
        }

        private void CheckMovementBools()
        {
            InitializeHoming();
            InitializeSeeking();
        }

        #region Movement Checks
        private void InitializeHoming()
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

        private void InitializeSeeking()
        {
            if (_isEnemyMovingInSinPattern)
            {
                _randomSinSeed = Random.Range(0f, Mathf.PI * 2f);
            }
        }
        #endregion

        private void UnitMovement()
        {
            if (IsEnemyDead)
            {
                MoveForward();
                DeathMovement();
                return;
            }

            #region Horizontal Movement
            //if enemy is homing rotate it before moving so it moves in the correct direction
            if (_isEnemyHomingOnPlayer)
            {
                HomingMovement();
            }

            MoveForward();
            #endregion

            #region Vertical Movement Patterns
            if (_isEnemyMovingUpDown)
            {
                UpDownMovement();
            }

            if (_isEnemyMovingInSinPattern)
            {
                SinMovement();
            }

            if (!_isPlayerInvisible.Value)
            {
                if (_isEnemySeekingPlayer)
                {
                    SeekMovement();
                }
            }
            #endregion
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

            else if (!_isPlayerInvisible.Value)
            {
                HomingCounter();
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

        private void HomingCounter()
        {
            _homeCounter -= Time.deltaTime;

            if (_homeCounter <= 0)
            {
                _isEnemyHomingOnPlayer = false;
                return;
            }

            RotateTowardsPlayer();
        }

        private void RotateTowardsPlayer()
        {
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

            Vector3 moveVector = Vector3.zero;

            if (_isBackForthDirectionUp)
            {
                moveVector += new Vector3(0, _upDownSpeed * Time.deltaTime, 0);
            }

            else
            {
                moveVector -= new Vector3(0, _upDownSpeed * Time.deltaTime, 0);
            }

            transform.position += moveVector;
        }

        public void FlipUpDownDirection()
        {
            if(IsEnemyDead)
            {
                return;
            }

            _isBackForthDirectionUp = !_isBackForthDirectionUp;
            _upDownTimer = Random.Range(1f, 5f);
        }
        #endregion

        #region Seek Movement
        protected void SeekMovement()
        {
            Vector2 moveVector = Vector2.zero;

            if (_isEnemySeekingPlayerOnYAxis)
            {
                moveVector += new Vector2(transform.position.x, _player.position.y);
            }

            if (_isEnemySeekingPlayerOnXAxis)
            {
                moveVector += new Vector2(_player.position.x, transform.position.y);
            }

            transform.position = Vector2.MoveTowards(transform.position, moveVector, _seekSpeed * Time.deltaTime);
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
    }

    [Serializable]
    public struct EnemyMovementStats
    {
        [Header("Movement Info")]
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
}