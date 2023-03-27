using UnityEngine;

public class Bullet : GameBehaviour
{
    public float speed;
    [SerializeField] private float damage;
    [SerializeField] private GameObject collisionParticles;
    [HideInInspector] public SpriteRenderer spriteRenderer;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnEnable()
    {
        GameManager.OnLevelCountDownStart += DestroyBullet;
    }

    private void OnDisable()
    {
        GameManager.OnLevelCountDownStart -= DestroyBullet;
    }

    private void Update()
    {
        transform.position += transform.right * speed * Time.deltaTime;
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
