using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(IBoss))]
public class Boss : Enemy
{

    [SerializeField] private float attackCooldown;
    private float attackTimer;
    private int randomAttackID;

    private IBoss bossMoveset;


    private void Awake()
    {
        base.Awake();
        bossMoveset = GetComponent<IBoss>();
    }
    private void Start()
    {
        base.Start();
        attackTimer = attackCooldown;
    }

    protected void Update()
    {
        base.Update();

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
        randomAttackID = Random.Range(0, 1);
        PerformAttack();
    }

    private void PerformAttack()
    {
        if (randomAttackID == 0)
        {
            bossMoveset.Attack1();
        }

        if (randomAttackID == 1)
        {
            bossMoveset.Attack2();
        }
    }
}
