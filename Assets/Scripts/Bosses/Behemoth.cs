using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Behemoth : Boss, IBoss
{
    [SerializeField] private Weapon _missileLauncher;
    [SerializeField] private Weapon _homingMissileLauncher;

    [SerializeField] private EnemyScriptableObject _missile;
    [SerializeField] private EnemyScriptableObject _homingMissile;

    private int _attacksSinceHomingAttack;

    public static event Action OnDied;

    private void Start()
    {
        _attacksSinceHomingAttack = 0;
    }
    protected override void ChooseRandomAttack()
    {
        if(_attacksSinceHomingAttack == 1)
        {
            Attack2();
        }
        else
        {
            int randomAttackID = Random.Range(0, 2);
            PerformAttack(randomAttackID);
        }
    }



    //missiles in random directions
    public void Attack1()
    {
        _missileLauncher.CheckFireTypes();
        _attacksSinceHomingAttack += 1;
    }

    //shoot homing missiles at different directions
    public void Attack2()
    {
        _homingMissileLauncher.CheckFireTypes();
        _attacksSinceHomingAttack = 0;
    }

    protected override void Crash()
    {
        base.Crash();
        if (OnDied != null) OnDied?.Invoke();
    }
}
