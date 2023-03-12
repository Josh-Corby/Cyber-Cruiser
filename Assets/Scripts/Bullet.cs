using UnityEngine;

public class Bullet : MonoBehaviour
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
