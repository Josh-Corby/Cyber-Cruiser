using System;
using UnityEngine;
using Random = UnityEngine.Random;


public class Boss : Enemy, IDamageable
{
    public static event Action<UISlider, float> OnBossDamage = null;
    public static event Action OnBossDied = null;

    private BossMovement _movement;

    [SerializeField] protected float _attackCooldown;
    [SerializeField] protected float _attackTimer;

    private IBoss bossMoveset;

    protected override void Awake()
    {
        base.Awake();
        _movement = GetComponent<BossMovement>();
        SetBossSpeed();
        bossMoveset = GetComponent<IBoss>();
    }

    private void SetBossSpeed()
    {
        _movement.speed = _unitInfo.speed;
    }
    protected override void Start()
    {
        base.Start();
        _attackTimer = _attackCooldown;
    }

    protected virtual void Update()
    {

        if (_attackTimer > 0)
        {
            _attackTimer -= Time.deltaTime;
        }

        if (_attackTimer <= 0)
        {
            ChooseRandomAttack();
            _attackTimer = _attackCooldown;
        }
    }

    protected void ChooseRandomAttack()
    {
        int randomAttackID = Random.Range(0, 2);
        PerformAttack(randomAttackID);
    }

    private void PerformAttack(int ID)
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
        OnBossDamage(GUIM.bossHealthBar, CurrentHealth);
        base.Damage(damage);
    }

    public override void Destroy()
    {
        OnBossDied?.Invoke();
        base.Destroy();
    }
}
