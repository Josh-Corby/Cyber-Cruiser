using UnityEngine;

public class Bullet : GameBehaviour
{
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

    private void Update()
    {
        bulletLifetime -= Time.deltaTime;
        if (bulletLifetime <= 0)
        {
            Destroy(gameObject);
        }

        transform.position += transform.right * speed * Time.deltaTime;
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
