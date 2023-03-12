using UnityEngine;
using System;
public class Enemy : GameBehaviour, IDamageable
{
    public enum movementDirection 
    { 
        Up, Down, Left, Right, DownLeft
    
    }

    public static event Action<GameObject> OnEnemyDied = null;

    [SerializeField] private movementDirection moveDirection;
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private float speed;
    private Vector2 direction;
    [SerializeField] private bool copyPlayerY;

    [SerializeField] private bool explodeOnDeath;
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionDamage;
    [SerializeField] private GameObject explosionGraphic;
    private Transform player;

    private void Awake()
    {
        player = PM.player.transform;
    }

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
            case movementDirection.DownLeft:
                direction = new Vector2(-1, -1);
                break;
        }


        currentHealth = maxHealth;
    }

    private void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;

        if (copyPlayerY)
        {
            transform.position = new Vector2(transform.position.x, player.position.y);
        }
    }

    public void Damage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            if (explodeOnDeath)
            {
                Explode();
            }
            else
            {
                Destroy();
            }
        }
    }

    private void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D collider in colliders)
        {
            GameObject explosionEffect = Instantiate(explosionGraphic, transform);
            explosionEffect.GetComponent<ExplosionGraphic>().explosionRadius = explosionRadius;
            explosionEffect.transform.SetParent(null);

            if (!collider.TryGetComponent<PlayerManager>(out var player))
            {
                continue;
            }
            else
            {
                player.Damage(explosionDamage);
            }       
        }

        Destroy();
    }

    public void Destroy()
    {
        if (ESM.enemiesAlive.Contains(gameObject))
        {
            OnEnemyDied(gameObject);
        }
        Destroy(gameObject);
    }

}
