using UnityEngine;
using Random = UnityEngine.Random;

namespace CyberCruiser
{
    public class Behemoth : Boss, IBoss
    {
        [SerializeField] private Weapon _missileLauncher;
        [SerializeField] private Weapon _homingMissileLauncher;

        [SerializeField] private EnemyScriptableObject _missile;
        [SerializeField] private EnemyScriptableObject _homingMissile;

        private int _attacksSinceHomingAttack = 0;
        [SerializeField] private int _attacksSinceWarCry = 0;
        [SerializeField] private WarCry _warCry;

        protected override void Start()
        {
            base.Start();
        }

        protected override void ChooseRandomAttack()
        {
            _attackTimer = _attackCooldown;

            if (_attacksSinceHomingAttack == 2)
            {
                Attack2();
                return;
            }

            if (_attacksSinceWarCry >= 5)
            {
                Attack3();
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
            _attacksSinceWarCry += 1;
        }

        //shoot homing missiles at different directions
        public void Attack2()
        {
            _homingMissileLauncher.CheckFireTypes();
            _attacksSinceHomingAttack = 0;
            _attacksSinceWarCry += 1;
        }

        public void Attack3()
        {
            _warCry.StartWarCry();
            _attacksSinceWarCry = 0;
            _attacksSinceHomingAttack += 1;
        }
    }
}