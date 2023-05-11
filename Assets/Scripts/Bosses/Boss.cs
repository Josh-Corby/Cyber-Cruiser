using System;
using UnityEngine;
using Random = UnityEngine.Random;


public class Boss : Enemy, IDamageable
{
    public static event Action<float> OnBossDamage = null;
    public static event Action<PickupType, Vector3> OnBossDied = null;

    private BossMovement _movement;

    [SerializeField] protected float _attackCooldown, _attackTimer;

    private IBoss bossMoveset;

    protected override void Awake()
    {
        _movement = GetComponent<BossMovement>();
        base.Awake();
        bossMoveset = GetComponent<IBoss>();
    }


    protected override void Start()
    {
        base.Start();
        _attackTimer = _attackCooldown;
    }

    protected virtual void Update()
    {
        if (GM.IsPaused) return;

        if (_movement.IsEnemyDead)
        {
            return;
        }
        if (_attackTimer > 0)
        {
            _attackTimer -= Time.deltaTime;
        }

        if (_attackTimer <= 0)
        {
            ChooseRandomAttack();         
        }
    }

    protected virtual void ChooseRandomAttack()
    {
        int randomAttackID = Random.Range(0, 2);
        PerformAttack(randomAttackID);
        _attackTimer = _attackCooldown;
    }

    protected void PerformAttack(int ID)
    {
        if (ID == 0)
        {
            bossMoveset.Attack1();
        }

        if (ID == 1)
        {
            bossMoveset.Attack2();
        }
    }

    public override void Damage(float damage)
    {
        OnBossDamage(_currentHealth);
        base.Damage(damage);
    }

    protected override void Crash()
    {
        base.Crash();
        OnBossDied(PickupType.Health, transform.position);
    }

    public override void Destroy()
    {
        base.Destroy();
    }
}
