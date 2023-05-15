using System;
using UnityEngine;

namespace CyberCruiser
{
    public class Behemoth : Boss, IBoss
    {
        [SerializeField] private Weapon _missileLauncher;
        [SerializeField] private Weapon _homingMissileLauncher;

        [SerializeField] private EnemyScriptableObject _missile;
        [SerializeField] private EnemyScriptableObject _homingMissile;

        private int _attacksSinceHomingAttack;

        public static event Action OnDied;

        protected override void Start()
        {
            base.Start();
            _attacksSinceHomingAttack = 0;
        }

        protected override void ChooseRandomAttack()
        {
            if (_attacksSinceHomingAttack == 1)
            {
                Attack2();
            }
            else
            {
                base.ChooseRandomAttack();
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
}