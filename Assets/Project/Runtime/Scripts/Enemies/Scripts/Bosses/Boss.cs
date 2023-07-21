using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CyberCruiser
{
    public class Boss : Enemy, IDamageable
    {
        [SerializeField] protected float _attackCooldown, _attackTimer;
        protected bool _isBossDead;
        private IBoss bossMoveset;

        public static event Action<float> OnBossDamage = null;
        public static event Action<PickupType, Vector3> OnBossDiedPosition = null;
        public static event Action<EnemyTypes> OnBossTypeDied = null;

        protected override void Awake()
        {
            base.Awake();
            bossMoveset = GetComponent<IBoss>();
        }

        protected virtual void Start()
        {
            _attackTimer = _attackCooldown;
            _isBossDead = false;
        }

        protected virtual void Update()
        {
            if (_isBossDead || _enemyMovement.IsPlayerInvisible || _enemyMovement.IsTimeStopped)
            {
                return;
            }

            if (_attackTimer > 0)
            {
                _attackTimer -= Time.deltaTime;
            }

            if (_attackTimer <= 0)
            {
                ChooseRandomAttack();
            }
        }

        protected virtual void ChooseRandomAttack()
        {
            _attackTimer = _attackCooldown;
            int randomAttackID = Random.Range(0, 2);
            PerformAttack(randomAttackID);
        }

        protected void PerformAttack(int ID)
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
            OnBossDamage(_currentHealth);
            base.Damage(damage);
        }

        protected override void Crash()
        {
            _isBossDead = true;
            base.Crash();
            OnBossDiedPosition(PickupType.Boss, transform.position);
        }

        public override void Destroy()
        {
            OnBossTypeDied?.Invoke(EnemyInfo.GeneralStats.Type);
            base.Destroy();
        }
    }
}