using UnityEngine;
using System;
using System.Collections.Generic;
public class Enemy : GameBehaviour, IDamageable
{
    public static event Action<List<GameObject>, GameObject> OnEnemyDied = null;

    public enum MovementDirection
    {
        Up, Down, Left, Right, DownLeft, None
    }

    public enum MovementTypes
    {
        Forward, BackForth, SeekPlayer, None
    }

    [SerializeField] protected MovementDirection moveDirection;
    [SerializeField] private MovementTypes moveType;

    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    public float speed;
    private Vector2 direction;
    [SerializeField] private bool copyPlayerY;

    [SerializeField] private bool explodeOnDeath;
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionDamage;
    [SerializeField] private GameObject explosionGraphic;
    private Transform player;

    [SerializeField] private bool bossEnemy;
    [SerializeField] private GameObject goalPoint;
    private Vector3 startPosition;
    //if true backforth will move up, otherwise move down
    [SerializeField] private bool backForthUp;


    [SerializeField] private int backForthMoveDistance;
    private Vector2 UpPosition;
    private Vector2 DownPosition;
    private bool goalPositionReached;


    protected void Awake()
    {
        player = PM.player.transform;
        startPosition = transform.position;
        goalPoint = ESM.bossGoalPosition;
    }

    protected virtual void Start()
    {     
        goalPositionReached = false;
        switch (moveDirection)
        {
            case MovementDirection.Up:
                direction = Vector2.up;
                break;
            case MovementDirection.Down:
                direction = Vector2.down;
                break;
            case MovementDirection.Left:
                direction = Vector2.left;
                break;
            case MovementDirection.Right:
                direction = Vector2.right;
                break;
            case MovementDirection.DownLeft:
                direction = new Vector2(-1, -1);
                break;
            case MovementDirection.None:
                direction = Vector2.zero;
                break;
        }

        switch (moveType)
        {
            default:
                break;

            case MovementTypes.BackForth:
                UpPosition = new Vector2(goalPoint.transform.position.x, startPosition.y + backForthMoveDistance);
                DownPosition = new Vector2(goalPoint.transform.position.x, startPosition.y - backForthMoveDistance);
                break;
        }

        currentHealth = maxHealth;
    }

    protected virtual void Update()
    {
        if (bossEnemy)
        {
            if (!goalPositionReached)
            {
                transform.position = Vector2.MoveTowards(transform.position, goalPoint.transform.position, speed * Time.deltaTime);

                if (Vector2.Distance(transform.position, goalPoint.transform.position) <= 0.1)
                {
                    goalPositionReached = true;
                }
                return;
            }
        }

        //goal position has been reached if it exits. Move on to normal movement patterns
        switch (moveType)
        {
            case MovementTypes.None:
                break;

            case MovementTypes.Forward:

                transform.position += (Vector3)direction * speed * Time.deltaTime;

                if (copyPlayerY)
                {
                    transform.position = new Vector2(transform.position.x, player.position.y);
                }
                break;

            case MovementTypes.BackForth:

                if (backForthUp)
                {
                    transform.position = Vector2.MoveTowards(transform.position, UpPosition, speed * Time.deltaTime);

                    if (Vector2.Distance(transform.position, UpPosition) <= 0.1f)
                    {
                        backForthUp = false;
                    }
                }

                if (!backForthUp)
                {
                    transform.position = Vector2.MoveTowards(transform.position, DownPosition, speed * Time.deltaTime);

                    if (Vector2.Distance(transform.position, DownPosition) <= 0.1f)
                    {
                        backForthUp = true;
                    }
                }
                break;

            case MovementTypes.SeekPlayer:
                transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
                break;
        }

        if (moveType == MovementTypes.None)
        {
            return;
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

    public virtual void Destroy()
    {
        if (ESM.enemiesAlive.Contains(gameObject))
        {
            OnEnemyDied(ESM.enemiesAlive, gameObject);
        }
        Destroy(gameObject);
    }
}
