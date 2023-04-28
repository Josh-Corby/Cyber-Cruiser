using UnityEngine;

public class Bullet : GameBehaviour
{
    private const string ENEMY_PROJECTILE_LAYER_NAME = "EnemyProjectile";
    private const string PLAYER_PROJECTILE_LAYER_NAME = "PlayerProjectile";

    #region References
    public GameObject homingTarget = null;

    [Header("Art")]
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _playerProjectileSprite;
    [SerializeField] private Sprite _enemyProjectileSprite;
    [SerializeField] private GameObject _collisionParticles;

    private BulletHoming _homingTrigger;
    #endregion

    #region Fields
    [SerializeField] private float _speed;
    private readonly float _damage;

    [SerializeField] private bool _isHoming;
    [SerializeField] protected float _homeTurnSpeed;
    [SerializeField] protected float _homeTime;
    [SerializeField] protected bool _homeDelay;
    [SerializeField] protected float _homeDelayTime;
    [SerializeField] private float _homeCounter;
    [SerializeField] private float _homeDelayCounter;
    private Vector2 direction;
    #endregion

    public float Speed
    {
        get => _speed;
        set => _speed = value;
    }

    public float Damage
    {
        get => _damage;
    }

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

    public Sprite ProjectileImage
    {
        set => _spriteRenderer.sprite = value;
    }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

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

    private void Update()
    {
        if (IsHoming)
        {
            if (homingTarget != null)
            {
                RotateTowardsTarget();
                MoveTowardsTarget();
                return;
            }
        }
        transform.position += transform.right * _speed * Time.deltaTime;
    }

    private void RotateTowardsTarget()
    {
        _homeCounter -= Time.deltaTime;

        if (_homeCounter <= 0)
        {
            direction = transform.up;
            IsHoming = false;
            //Debug.Log("No longer rotating towards player");
            return;
        }

        Vector3 vectorToPlayer = homingTarget.transform.position - transform.position;
        float angle = Mathf.Atan2(vectorToPlayer.y, vectorToPlayer.x) * Mathf.Rad2Deg - 90;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, _homeTurnSpeed * Time.deltaTime);
        direction = transform.up;
    }

    private void MoveTowardsTarget()
    {
        transform.position = Vector2.MoveTowards(transform.position, homingTarget.transform.position, _speed * Time.deltaTime);
    }

    public void CheckBulletLayer()
    {
        if (gameObject.layer == LayerMask.NameToLayer(ENEMY_PROJECTILE_LAYER_NAME))
        {
            homingTarget = PM.player;
        }
        else return;
    }

    public void SwitchBulletTeam()
    {
        //switch from player team
        if(gameObject.layer == LayerMask.NameToLayer(PLAYER_PROJECTILE_LAYER_NAME))
        {
            gameObject.layer = ChangeLayerFromString(ENEMY_PROJECTILE_LAYER_NAME);
            _spriteRenderer.sprite = _enemyProjectileSprite;
        }

        //switch to player team
        else if(gameObject.layer == LayerMask.NameToLayer(ENEMY_PROJECTILE_LAYER_NAME))
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
        if (collider.TryGetComponent<Shield>(out var shield))
        {
            return;
        }

        GameObject particles = Instantiate(_collisionParticles, transform);
        particles.transform.parent = null;

        if (!collider.TryGetComponent<IDamageable>(out var interactable))
        {
            Destroy(gameObject);
            return;
        }

        interactable.Damage(Damage);
        Destroy(gameObject);
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
