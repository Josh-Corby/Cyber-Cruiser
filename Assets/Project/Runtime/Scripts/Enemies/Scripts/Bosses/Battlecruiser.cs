using System;
using System.Collections.Specialized;
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

        private readonly Vector2 SHIELDACTIVATIONRANGE = new Vector2(8, 12);

        [SerializeField] private ShieldControllerBase _shields;
        private bool _isBeamCharging = false;

        protected override void Awake()
        {
            base.Awake();
            _beamAttack = _pulverizerBeam.GetComponent<BeamAttack>();
            _shields = GetComponentInChildren<ShieldControllerBase>();
        }

        private void OnEnable()
        {
            ShieldControllerBase.OnShieldDeactivated += OnShieldsDeactivated;
        }

        private void OnDisable()
        {
            ShieldControllerBase.OnShieldDeactivated -= OnShieldsDeactivated;
        }

        protected override void Start()
        {
            base.Start();
            _beamAttack._owner = EnemyInfo;
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
            _chargingParticles.Stop();
            _beamAttack.ResetBeam();
            _beamAttack.EnableBeam();
            _beamAttack.StartFiring();
            _isBeamCharging = false;    
        }

        public void Attack3()
        {
            throw new NotImplementedException();
        }

        protected override void Crash()
        {
            _beamAttack.ResetBeam();
            _beamAttack.StopFiring();
            base.Crash();
        }

        private void StartShieldCountdown()
        {
            Invoke(nameof(ActivateShields), Random.Range(SHIELDACTIVATIONRANGE.x, SHIELDACTIVATIONRANGE.y));
            Debug.Log("shields warming up");
        }

        private void ActivateShields()
        {
            _shields.ActivateShields();
        }

        private void OnShieldsDeactivated(ShieldControllerBase shield)
        {
            if(shield == _shields)
            {
                //shield deactivated
                Debug.Log("shields deactivated");
                StartShieldCountdown();
            }
        }
    }
}