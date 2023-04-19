using UnityEngine;

public class Bullet : GameBehaviour
{
    private const string ENEMY_LAYER = "Enemy";

    #region References
    public GameObject homingTarget = null;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject collisionParticles;
    private BulletHoming _homingTrigger;
    #endregion

    #region Fields
    public float speed;
    public float damage;
    [SerializeField] private bool _isHoming;
    [SerializeField] protected float _homeTurnSpeed;
    [SerializeField] protected float _homeTime;
    [SerializeField] protected bool _homeDelay;
    [SerializeField] protected float _homeDelayTime;
    [SerializeField] private float homeCounter;
    [SerializeField] private float _homeDelayCounter;
    [HideInInspector] public Vector2 direction;
    #endregion

    public bool IsHoming
    {
        get
        {
            return _isHoming;
        }
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
        spriteRenderer = GetComponent<SpriteRenderer>();

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
                homeCounter = _homeTime;
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
        transform.position += transform.right * speed * Time.deltaTime;
    }

    private void RotateTowardsTarget()
    {
        homeCounter -= Time.deltaTime;

        if (homeCounter <= 0)
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
        transform.position = Vector2.MoveTowards(transform.position, homingTarget.transform.position, speed * Time.deltaTime);
    }

    public void CheckBulletLayer()
    {
        if (gameObject.layer == LayerMask.NameToLayer(ENEMY_LAYER))
        {
            homingTarget = PM.player;
        }
        else return;
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

        GameObject particles = Instantiate(collisionParticles, transform);
        particles.transform.parent = null;

        if (!collider.TryGetComponent<IDamageable>(out var interactable))
        {

            Destroy(gameObject);
            return;
        }

        interactable.Damage(damage);
        Destroy(gameObject);
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
