using System;
using UnityEngine;
using Random = UnityEngine.Random;


public class Boss : Enemy
{
    [SerializeField] protected float attackCooldown;
    [SerializeField] protected float attackTimer;

    private IBoss bossMoveset;

    protected virtual void Awake()
    {
        bossMoveset = GetComponent<IBoss>();
    }
    protected override void Start()
    {
        base.Start();
        attackTimer = attackCooldown;
    }

    protected virtual void Update()
    {

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        if (attackTimer <= 0)
        {
            ChooseRandomAttack();
            attackTimer = attackCooldown;
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
}
