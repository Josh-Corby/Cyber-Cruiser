using UnityEngine;

namespace CyberCruiser
{
    public class Behemoth : Boss, IBoss
    {
        [SerializeField] private Weapon _missileLauncher;
        [SerializeField] private Weapon _homingMissileLauncher;

        private int _attacksSinceHomingAttack = 0;
        private int _attacksToWarCry = 5;
        private const int MINATTACKSTOWARCRY = 3;
        private const int MAXATTACKSTOWARCRY = 7;
        [SerializeField] private int _attacksSinceWarCry = 5;
        [SerializeField] private WarCry _warCry;

        protected override void Start()
        {
            base.Start();
            _attacksToWarCry = Random.Range(MINATTACKSTOWARCRY, MAXATTACKSTOWARCRY);
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
            _attacksToWarCry = Random.Range(MINATTACKSTOWARCRY, MAXATTACKSTOWARCRY);
            _attacksSinceWarCry = 0;
            _attacksSinceHomingAttack += 1;
        }
    }
}