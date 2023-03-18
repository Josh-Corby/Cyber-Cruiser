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
        Forward, None
    }

    [SerializeField] protected MovementDirection moveDirection;
    [SerializeField] public MovementTypes moveType;

    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    public float speed;
    [SerializeField] private float upDownSpeed;
    private Vector2 direction;
    [SerializeField] protected bool seekPlayer;
    [SerializeField] protected float seekSpeed;

    [SerializeField] private bool explodeOnDeath;
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionDamage;
    [SerializeField] private GameObject explosionGraphic;
    private Transform player;

    [SerializeField] private bool bossEnemy;
    [SerializeField] private GameObject goalPoint;
    private Vector3 startPosition;
    [SerializeField] protected bool upDown;
    //if true backforth will move up, otherwise move down
    [SerializeField] private bool backForthUp;


    [SerializeField] private int backForthMoveDistance;
    private readonly float yUp = 4.5f;
    private readonly float yDown = -4.5f;
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

        //if (upDown)
        //{
        //    yUp = new Vector2(goalPoint.transform.position.x, goalPoint.transform.position.y + backForthMoveDistance);
        //    yDown = new Vector2(goalPoint.transform.position.x, goalPoint.transform.position.y - backForthMoveDistance);
        //}
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
                break;
        }

        if (upDown)
        {
            if (backForthUp)
            {
                if(transform.position.y < yUp)
                {
                    transform.position += new Vector3(0, upDownSpeed * Time.deltaTime, 0);
                }
                else
                {
                    backForthUp = false;
                }
                //if (Vector2.Distance(transform.TransformPoint(transform.localPosition), UpPosition) <= 0.1f)
                //{
                //    backForthUp = false;
                //}
                //transform.position = new Vector2(transform.position.x, transform.position.y + speed * Time.deltaTime);
                //transform.position = Vector2.MoveTowards(transform.position, UpPosition, speed * Time.deltaTime);
            }

            if (!backForthUp)
            {
                if(transform.position.y> yDown)
                {
                    transform.position -= new Vector3(0, upDownSpeed * Time.deltaTime, 0);
                }
                else
                {
                    backForthUp = true;
                }
                //if (Vector2.Distance(transform.TransformPoint(transform.localPosition), DownPosition) <= 0.1f)
                //{
                //    backForthUp = true;
                //}

                //transform.position = new Vector2(transform.position.x, transform.position.y - speed * Time.deltaTime);
                //transform.position = Vector2.MoveTowards(transform.position, DownPosition, speed * Time.deltaTime);
            }
        }


        if (seekPlayer)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, player.position.y), seekSpeed * Time.deltaTime);
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
