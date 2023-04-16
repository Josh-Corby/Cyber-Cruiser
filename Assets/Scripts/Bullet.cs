using UnityEngine;

public class Bullet : GameBehaviour
{
    private const string PLAYER_LAYER = "Player";
    private const string ENEMY_LAYER = "Enemy";

    public float speed;
    public float damage;
    public bool isHoming;
    private float _minHomeRange;
    [SerializeField] protected float _homeTurnSpeed;
    [SerializeField] protected float _homeTime;
    [SerializeField] private float homeCounter;
    [SerializeField] protected bool _homeDelay;
    [SerializeField] protected float _homeDelayTime;
    [SerializeField] private float _homeDelayCounter;
    [HideInInspector] public Vector2 direction;
    [SerializeField] private GameObject collisionParticles;
    [HideInInspector] public SpriteRenderer spriteRenderer;

    public GameObject homingTarget = null;
    private BulletHoming _homingTrigger;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        _homingTrigger = GetComponentInChildren<BulletHoming>();
    }

    private void OnEnable()
    {
        GameManager.OnMissionStart += DestroyBullet;
    }

    private void OnDisable()
    {
        GameManager.OnMissionStart -= DestroyBullet;
    }

    private void Start()
    {
        if (isHoming)
        {
            _homingTrigger.gameObject.SetActive(true);
            homeCounter = _homeTime;
            CheckBulletLayer();
        }
        if (!isHoming)
        {
            _homingTrigger.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (isHoming)
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
            isHoming = false;
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
