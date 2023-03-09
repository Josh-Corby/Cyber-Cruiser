using UnityEngine;

public class Bullet : MonoBehaviour
{

    [SerializeField] private float speed;
    private Vector2 direction = Vector2.right;
    [SerializeField] private float damage;
    [SerializeField] private GameObject collisionParticles;

    private float bulletLifetime = 3f;
    private void Update()
    {
        bulletLifetime -= Time.deltaTime;
        if (bulletLifetime <= 0)
        {
            Destroy(gameObject);
        }

        transform.position += (Vector3)direction * speed * Time.deltaTime;

        if(transform.rotation.z != 0)
        {
            direction = transform.right;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("bullet collision");

        ProcessCollision(collision.gameObject);
    }

    private void ProcessCollision(GameObject collider)
    {
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
}
