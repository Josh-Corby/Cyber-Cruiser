using System;
using UnityEngine;

public class Behemoth : Boss, IBoss
{
    [SerializeField] private Weapon _missileLauncher;
    [SerializeField] private Weapon _homingMissileLauncher;

    [SerializeField] private EnemyScriptableObject _missile;
    [SerializeField] private EnemyScriptableObject _homingMissile;

    public static event Action OnDied;

    //missiles in random directions
    public void Attack1()
    {
        _missileLauncher.CheckFireTypes();
    }

    //shoot homing missiles at different directions
    public void Attack2()
    {
        _homingMissileLauncher.CheckFireTypes();
    }

    protected override void Crash()
    {
        base.Crash();
        if (OnDied != null) OnDied?.Invoke();
    }
}
