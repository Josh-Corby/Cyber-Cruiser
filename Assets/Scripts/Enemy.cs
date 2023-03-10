using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public enum movementDirection 
    { 
        Up, Down, Left, Right
    
    }
    [SerializeField] private movementDirection moveDirection;
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private float speed;
    private Vector2 direction;

    private void Start()
    {
        switch (moveDirection)
        {
            case movementDirection.Up:
                direction = Vector2.up;
                break;
            case movementDirection.Down:
                direction = Vector2.down;
                break;
            case movementDirection.Left:
                direction = Vector2.left;
                break;
            case movementDirection.Right:
                direction = Vector2.right;
                break;
        }


        currentHealth = maxHealth;
    }

    private void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    public void Damage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Destroy();
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    private void MoveForward()
    {

    }
}
