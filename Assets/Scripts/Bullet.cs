using UnityEngine;

public class Bullet : GameBehaviour
{
    public enum moveDirection
    {
        Left, Right
    }

    [SerializeField] private moveDirection direction;
    [SerializeField] private float speed;
    private Vector2 directionVector;
    [SerializeField] private float damage;
    [SerializeField] private GameObject collisionParticles;

    private float bulletLifetime = 3f;

    private void OnEnable()
    {
        GameManager.OnLevelCountDownStart += DestroyBullet;
    }

    private void OnDisable()
    {
        GameManager.OnLevelCountDownStart -= DestroyBullet;
    }

    private void Start()
    {
        switch (direction)
        {
            case moveDirection.Left:
                directionVector = Vector2.left;
                break;
            case moveDirection.Right:
                directionVector = Vector2.right;
                break;
        }
    }
    private void Update()
    {
        bulletLifetime -= Time.deltaTime;
        if (bulletLifetime <= 0)
        {
            Destroy(gameObject);
        }

        transform.position += (Vector3)directionVector * speed * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("bullet collision");

        ProcessCollision(collision.gameObject);
    }

    private void ProcessCollision(GameObject collider)
    {
        Debug.Log(collider.name);
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
