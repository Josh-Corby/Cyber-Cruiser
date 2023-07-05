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

        [Header("Art")]
        [SerializeField] private Sprite _playerProjectileSprite;
        [SerializeField] private Sprite _enemyProjectileSprite;
        [SerializeField] private GameObject _collisionParticles;

        [SerializeField] private float _speed;
        [SerializeField] private float _damage;
        [SerializeField] private bool _doesBulletExplode;
        [SerializeField] private GameObject _explosion;

        [Header("Homing Stats")]
        [SerializeField] private bool _isHoming;
        [SerializeField] private float _homeTurnSpeed;
        [SerializeField] private float _homeTime;
        [SerializeField] private bool _homeDelay;
        [SerializeField] private float _homeDelayTime;
        [SerializeField] private float _homeCounter;

        public float Damage { get => _damage; }

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

        public GameObject HomingTarget { get => _homingTarget; set => _homingTarget = value; }

        private void Awake()
        {
            GetComponents();
        }

        private void GetComponents()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            if (transform.childCount > 0)
            {
                _homingTrigger = GetComponentInChildren<BulletHoming>();
            }
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
            if (IsHoming)
            {
                if (_homingTarget != null)
                {
                    HomingMovement();
                }
            }

            MoveRight();
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
            }
        }

        private void MoveRight()
        {
            transform.position += transform.right * _speed * Time.deltaTime;
        }

        private void HomingMovement()
        {
            RotateTowardsTarget();
            MoveTowardsTarget();
        }

        private void RotateTowardsTarget()
        {
            _homeCounter -= Time.deltaTime;

            if (_homeCounter <= 0)
            {
                IsHoming = false;
                return;
            }

            Vector3 vectorToPlayer = _homingTarget.transform.position - transform.position;
            float angle = Mathf.Atan2(vectorToPlayer.y, vectorToPlayer.x) * Mathf.Rad2Deg - 90;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, q, _homeTurnSpeed * Time.deltaTime);
        }

        private void MoveTowardsTarget()
        {
            transform.position = Vector2.MoveTowards(transform.position, _homingTarget.transform.position, _speed * Time.deltaTime);
        }

        public void CheckBulletLayer()
        {
            if (gameObject.layer == LayerMask.NameToLayer(ENEMY_PROJECTILE_LAYER_NAME))
            {
                _homingTarget = PlayerManagerInstance.player;
            }
        }

        public void SwitchBulletTeam()
        {
            if (gameObject.layer == LayerMask.NameToLayer(PLAYER_PROJECTILE_LAYER_NAME))
            {
                gameObject.layer = ChangeLayerFromString(ENEMY_PROJECTILE_LAYER_NAME);
                _spriteRenderer.sprite = _enemyProjectileSprite;
            }

            else if (gameObject.layer == LayerMask.NameToLayer(ENEMY_PROJECTILE_LAYER_NAME))
            {
                gameObject.layer = ChangeLayerFromString(PLAYER_PROJECTILE_LAYER_NAME);
                _spriteRenderer.sprite = _playerProjectileSprite;
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
                DestroyBullet();
                return;
            }

            else if (collider.TryGetComponent<IDamageable>(out var interactable))
            {
                interactable.Damage(Damage);
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
            //Debug.Log("Bullet reflected");
            transform.right = objectReflectedFrom.transform.right;
            _speed /= 2;
            SwitchBulletTeam();
            _spriteRenderer.flipX = !_spriteRenderer.flipX;
        }

        private void DestroyBullet()
        {
            Destroy(gameObject);
        }
    }
}