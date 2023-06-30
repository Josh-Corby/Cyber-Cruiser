using System;
using System.Collections.Specialized;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CyberCruiser
{
    public class Battlecruiser : Boss, IBoss
    {
        [SerializeField] private GameObject _mineReleasePoint;
        [SerializeField] private EnemyScriptableObject _seekerMineInfo;
        [SerializeField] private int _minesToFire = 1;

        [SerializeField] private ParticleSystem _chargingParticles;
        [SerializeField] private GameObject _pulverizerBeam;
        [SerializeField] private BeamAttack _beamAttack;

        private bool _isBeamCharging = false;

        public static event Action OnDied = null;

        protected override void Awake()
        {
            base.Awake();
            _beamAttack = _pulverizerBeam.GetComponent<BeamAttack>();
        }

        protected override void Update()
        {
            if(_isBossDead)
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

        //check if beam is active
        //if so fire a mine
        protected override void ChooseRandomAttack()
        {
            _attackTimer = _attackCooldown;

            if (_beamAttack.IsBeamFiring && !_isBeamCharging)
            {
                Attack1();
            }

            else
            {
                int randomAttackID = Random.Range(0, 2);
                PerformAttack(randomAttackID);
            }
        }

        //release seeker mines
        public void Attack1()
        {
            ReleaseMines();
        }

        private void ReleaseMines()
        {
            for (int i = 0; i < _minesToFire; i++)
            {
                GameObject mineObject = EnemyManagerInstance.CreateEnemyFromSO(_seekerMineInfo);
                GameObject seekermine = Instantiate(mineObject, _mineReleasePoint.transform.position, _mineReleasePoint.transform.rotation);
                seekermine.transform.SetParent(null);
            }
        }

        //fire laser
        public void Attack2()
        {
            _isBeamCharging = true;
            _chargingParticles.Play();
            Debug.Log("beam charging");
            Invoke(nameof(BeamAttack), 2f);            
        }

        private void BeamAttack()
        {
            _isBeamCharging = false;    
            _chargingParticles.Stop();
            _beamAttack.ResetBeam();
            _beamAttack.lineRenderer.enabled = true;
            _beamAttack.EnableBeam();
            _beamAttack.StartFiring();
        }

        private void StopBeam()
        {
            _beamAttack.StopFiring();
            _beamAttack.DisableBeam();
            _beamAttack.lineRenderer.enabled = false;
        }

        protected override void Crash()
        {
            base.Crash();
            
            _beamAttack.DisableBeam();
            OnDied?.Invoke();
        }
    }
}