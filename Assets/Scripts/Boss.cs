using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(IBoss))]
public class Boss : Enemy
{

    [SerializeField] private float attackCooldown;
    [SerializeField] private float attackTimer;

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
