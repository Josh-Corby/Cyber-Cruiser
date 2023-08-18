using System;
using UnityEngine;

namespace CyberCruiser
{
    public class Bullet : GameBehaviour
    {
        private const string ENEMY_PROJECTILE_LAYER_NAME = "EnemyProjectile";
        private const string PLAYER_PROJECTILE_LAYER_NAME = "PlayerProjectile";

        private BulletHoming _homingTrigger;
        private GameObject _homingTarget = null;
        private SpriteRenderer _spriteRenderer;
        [SerializeField] private bool _isPlayerBullet = false;
        [SerializeField] private BoolReference _isTimeStopped;
        private bool _rotateOut;
        private Quaternion _homeOutRotation;
        private const float _homeTurnSpeedBase = 100;
        #region Art
        [Header("Art")]
        [SerializeField] private Sprite _playerProjectileSprite;
        [SerializeField] private Sprite _enemyProjectileSprite;
        [SerializeField] private GameObject _collisionParticles;
        #endregion

        #region Bullet Info
        [SerializeField] private float _speed;
        [SerializeField] private float _damage;
        [SerializeField] private bool _doesBulletExplode;
        [SerializeField] private GameObject _explosion;
        #endregion

        #region Homing 
        [Header("Homing Stats")]
        [SerializeField] private bool _isHoming;
        [SerializeField] private float _homeTurnSpeed;
        [SerializeField] private float _homeTime;
        [SerializeField] private bool _homeDelay;
        [SerializeField] private float _homeDelayTime;
        [SerializeField] private float _homeCounter;
        #endregion

        public float Damage { get => _damage; }

        public GameObject HomingTarget { get => _homingTarget; set => _homingTarget = value; }

        public EnemyScriptableObject Owner { get; set; }

        public bool IsHoming
        {
            get => _isHoming;
            set
            {
                _isHoming = value;
                if (_isHoming == false)
                {
                    _homingTrigger.ClearEnemiesInRange();
                    _homingTrigger.enabled = false;
                }
            }
        }

        private void Awake()
        {
            GetComponents();
        }

        private void OnEnable()
        {
            GameManager.OnMissionEnd += DestroyBullet;
        }

        private void OnDisable()
        {
            GameManager.OnMissionEnd -= DestroyBullet;
        }

        private void Start()
        {
            AssignHoming();
        }

        private void Update()
        {
            if (_isTimeStopped.Value && !_isPlayerBullet)
            {
                return;
            }

            if (IsHoming)
            {
                if (_homingTarget != null)
                {
                    RotateTowardsTarget();
                }
            }

            MoveRight();
        }

        private void GetComponents()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            if (transform.childCount > 0)
            {
                _homingTrigger = GetComponentInChildren<BulletHoming>();
            }
        }

        private void AssignHoming()
        {
            if (_homingTrigger != null)
            {
                if (IsHoming)
                {
                    _homingTrigger.gameObject.SetActive(true);
                    _homeCounter = _homeTime;
                    CheckBulletLayer();
                }

                if (!IsHoming)
                {
                    _homingTrigger.gameObject.SetActive(false);
                }

                _homeTurnSpeed = _homeTurnSpeedBase; ;
                _rotateOut = true;
            }
        }

        private void MoveRight()
        {
            transform.position += transform.right * _speed * Time.deltaTime;
        }

        public void AssignHomingRotation(Quaternion rotation)
        {
            _homeOutRotation = rotation;
        }

        private void RotateTowardsTarget()
        {
            float t = Time.deltaTime;
            _homeCounter -= t;

            if (_homeCounter <= 0)
            {
                IsHoming = false;
                return;
            }

            Vector2 direction = _homingTarget.transform.position - transform.position;
            float angle = MathF.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, angle);

            if (_rotateOut)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, _homeOutRotation, _homeTurnSpeed * Time.deltaTime);

                if(transform.rotation == _homeOutRotation)
                {
                    _rotateOut = false;
                    _homeTurnSpeed = _homeTurnSpeedBase;          
                }
            }

            if(!_rotateOut)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _homeTurnSpeed * Time.deltaTime);
            }

            _homeTurnSpeed += (2 * t * t) + (_homeTurnSpeed / 3);
        }

        public void CheckBulletLayer()
        {
            if (gameObject.layer == LayerMask.NameToLayer(ENEMY_PROJECTILE_LAYER_NAME))
            {
                _homingTarget = PlayerManagerInstance.player;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            ProcessCollision(collision.gameObject);
        }

        private void ProcessCollision(GameObject collider)
        {
            if (_collisionParticles != null)
            {
                GameObject particles = Instantiate(_collisionParticles, transform);
                particles.transform.parent = null;
            }

            if (_doesBulletExplode)
            {
                Explode();
            }

            if (collider.GetComponent<Shield>())
            {
                //DestroyBullet();
                return;
            }

            else if (collider.TryGetComponent<IDamageable>(out var interactable))
            {
                interactable.Damage(Damage, Owner);
            }

            DestroyBullet();
        }

        private void Explode()
        {
            GameObject explosion = Instantiate(_explosion, transform.position, Quaternion.identity);
            explosion.transform.parent = null;
        }

        public void Reflect(GameObject objectReflectedFrom)
        {
            transform.right = objectReflectedFrom.transform.right;
            _speed /= 2;
            SwitchBulletTeam();
            _spriteRenderer.flipX = !_spriteRenderer.flipX;
        }

        public void SwitchBulletTeam()
        {
            if (gameObject.layer == LayerMask.NameToLayer(PLAYER_PROJECTILE_LAYER_NAME))
            {
                gameObject.layer = ChangeLayerFromString(ENEMY_PROJECTILE_LAYER_NAME);
                //_spriteRenderer.sprite = _enemyProjectileSprite;
                _isPlayerBullet = false;
            }

            else if (gameObject.layer == LayerMask.NameToLayer(ENEMY_PROJECTILE_LAYER_NAME))
            {
                gameObject.layer = ChangeLayerFromString(PLAYER_PROJECTILE_LAYER_NAME);
                //_spriteRenderer.sprite = _playerProjectileSprite;
                _isPlayerBullet = true;
            }
        }

        private void DestroyBullet()
        {
            Destroy(gameObject);
        }
    }
}