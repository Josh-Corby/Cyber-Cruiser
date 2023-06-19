using System;
using System.Collections;
using UnityEngine;

namespace CyberCruiser
{
    public class Behemoth : Boss, IBoss
    {
        [SerializeField] private Weapon _missileLauncher;
        [SerializeField] private Weapon _homingMissileLauncher;

        [SerializeField] private EnemyScriptableObject _missile;
        [SerializeField] private EnemyScriptableObject _homingMissile;

        [SerializeField] private int _attacksSinceHomingAttack;


        [SerializeField] private CircleCollider2D _warCryCollider;
        private float _currentWarCryRadius;
        [SerializeField] private float _warCryRange;

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
                _attackTimer = _attackCooldown;
                Attack2();
                return;
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