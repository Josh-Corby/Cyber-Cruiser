using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

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
    [SerializeField] protected bool seekPlayerY;
    [SerializeField] protected float seekSpeed;

    [SerializeField] private bool explodeOnDeath;
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionDamage;
    [SerializeField] private GameObject explosionGraphic;
    [SerializeField] private Transform player;

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

    [SerializeField] private bool sinUpDown;
    [SerializeField] private float frequency;
    [SerializeField] private float magnitude;

    [SerializeField] private bool homeOnPlayer;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float homeTime;
    private float homeCounter;
    [SerializeField] private Quaternion targetRotation;

    //offset for random starting sin value
    private float startTime;
    protected void Awake()
    {
        player = PM.player.transform;
        startPosition = transform.position;
        goalPoint = ESM.bossGoalPosition;
    }

    protected virtual void Start()
    {
        if (homeOnPlayer)
        {
            homeCounter = homeTime;
        }

        startTime = Random.Range(0f, Mathf.PI * 2f);

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

        currentHealth = maxHealth;
    }

    protected virtual void Update()
    {
        if (bossEnemy)
        {
            if (!goalPositionReached)
            {
                MoveTowardGoalPosition();
            }
        }

        if (homeOnPlayer)
        {
            RotateTowardsPlayer();
        }

        if (moveType == MovementTypes.Forward)
        {
            MoveForward();
        }

        if (upDown)
        {
            UpDownMovement();
            return;
        }

        if (seekPlayerY)
        {
            SeekPlayerY();
            return;
        }

        if (sinUpDown)
        {
            SinUpDown();
            return;
        }
    }

    private void MoveTowardGoalPosition()
    {
        transform.position = Vector2.MoveTowards(transform.position, goalPoint.transform.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, goalPoint.transform.position) <= 0.1)
        {
            goalPositionReached = true;
        }
        return;
    }

    private void MoveForward()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    private void UpDownMovement()
    {
        if (backForthUp)
        {
            if (transform.position.y < yUp)
            {
                transform.position += new Vector3(0, upDownSpeed * Time.deltaTime, 0);
            }
            else
            {
                backForthUp = false;
            }
        }

        if (!backForthUp)
        {
            if (transform.position.y > yDown)
            {
                transform.position -= new Vector3(0, upDownSpeed * Time.deltaTime, 0);
            }
            else
            {
                backForthUp = true;
            }
        }
    }

    private void SeekPlayerY()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, player.position.y), seekSpeed * Time.deltaTime);
    }

    private void SinUpDown()
    {
        float yPos = Mathf.Sin((Time.time - startTime) * frequency) * magnitude;
        transform.position = new Vector3(transform.position.x, transform.position.y + yPos, transform.position.z);
    }

    private void RotateTowardsPlayer()
    {
        homeCounter -= Time.deltaTime;

        if (homeCounter <= 0)
        {
            direction = transform.up;
            homeOnPlayer = false;
            Debug.Log("No longer rotating towards player");
            return;
        }

        Vector3 vectorToPlayer = player.position - transform.position;
        float angle = MathF.Atan2(vectorToPlayer.y, vectorToPlayer.x) * Mathf.Rad2Deg - 90;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, turnSpeed * Time.deltaTime);
        direction = transform.up;
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
            explosionEffect.transform.localScale = Vector3.one * 10;

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
