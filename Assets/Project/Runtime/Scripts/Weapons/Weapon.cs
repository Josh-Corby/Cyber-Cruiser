using CyberCruiser.Audio;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CyberCruiser
{
    [RequireComponent(typeof(OneShotAudioController))]
    public class Weapon : GameBehaviour
    {
        [SerializeField] private WeaponScriptableObject _weaponBase;
        [SerializeField] private OneShotAudioController _oneShotAudioController;
        private WeaponStats _stats;
        private GameObject _firePoint;
        private Transform _firePointTransform;

        private bool _readyToFire;
        protected bool _autoFire;

        public bool ReadyToFire { get => _readyToFire; }

        public WeaponStats CurrentStats { get => _stats; set => _stats = value; }

        private void Awake()
        {
            _oneShotAudioController = GetComponent<OneShotAudioController>();
            _firePoint = transform.GetChild(0).gameObject;
            _firePointTransform = _firePoint.transform;
            WeaponSetup();
        }

        protected virtual void OnEnable()
        {
            _readyToFire = true;
        }

        private void WeaponSetup()
        {
            ResetWeapon();

            _oneShotAudioController._oneShotClip = _weaponBase.Stats.FireClip;
        }

        public void ResetWeapon()
        {
            _stats = _weaponBase.Stats;
        }

        private void Update()
        {
            if (_autoFire)
            {
                if (_readyToFire)
                {
                    CheckFireTypes();
                }
            }
        }

        public void CheckFireTypes()
        {
            _readyToFire = false;

            if (_stats.IsWeaponBurstFire)
            {
                StartCoroutine(BurstFire());
                return;
            }

            else if (_stats.IsWeaponMultiFire)
            {
                MultiFire();
            }

            else
            {
                SpreadCheck();
            }

            StartCoroutine(ResetShooting());
        }

        private IEnumerator BurstFire()
        {
            for (int i = 0; i < _stats.AmountOfBursts; i++)
            {
                if (_stats.IsWeaponMultiFire)
                {
                    MultiFire();
                }

                else
                {
                    SpreadCheck();
                }

                yield return new WaitForSeconds(_stats.TimeBetweenBurstShots);
            }
            StartCoroutine(ResetShooting());
        }

        private void MultiFire()
        {
            if (_stats.IsMultiFireSpreadRandom)
            {
                for (int i = 0; i < _stats.MultiFireShots; i++)
                {
                    FireBullet(GetRandomSpreadAngle());
                }
                return;
            }

            for (int i = 0; i < _stats.MultiFireShots; i++)
            {
                Quaternion projectileSpread = GetMultiShotFixedAngle(i);
                FireBullet(projectileSpread);
            }
        }

        private void SpreadCheck()
        {
            if (!_stats.DoesWeaponUseSpread)
            {
                FireBullet(_firePointTransform.rotation);
            }
            else
            {
                FireBullet(GetRandomSpreadAngle());
            }
        }

        private Quaternion GetRandomSpreadAngle()
        {
            Quaternion directionWithSpread = _firePointTransform.rotation * Quaternion.Euler(0, 0, Random.Range(-_stats.SpreadHalfAngle, _stats.SpreadHalfAngle));
            return directionWithSpread;
        }

        private Quaternion GetMultiShotFixedAngle(int multifireBulletIndex)
        {
            float totalSpreadAngle = _stats.SpreadHalfAngle * 2;
            float bulletFireAngle = totalSpreadAngle / (_stats.MultiFireShots - 1) * multifireBulletIndex;
            float finalAngle = bulletFireAngle - _stats.SpreadHalfAngle;
            Quaternion directionWithSpread = _firePointTransform.rotation * Quaternion.Euler(0, 0, finalAngle);
            return directionWithSpread;
        }

        private void FireBullet(Quaternion direction)
        {
            GameObject bullet = Instantiate(_stats.objectToFire, _firePointTransform.position, direction);
            _oneShotAudioController.PlayClip();
        }

        private IEnumerator ResetShooting()
        {
            yield return new WaitForSeconds(_stats.TimeBetweenShots);
            _readyToFire = true;
        }
    }

        [Serializable]
        public class WeaponStats
        {
            public GameObject objectToFire;
            public float TimeBetweenShots;
            public bool IsWeaponAutomatic;
            public AudioClip FireClip;

            [Header("Spread")]
            public bool DoesWeaponUseSpread;
            public float SpreadHalfAngle;

            [Header("Burst Fire")]
            public bool IsWeaponBurstFire;
            public int AmountOfBursts;
            public float TimeBetweenBurstShots;

            [Header("Multi Shot")]
            public bool IsWeaponMultiFire;
            public int MultiFireShots;
            public bool IsMultiFireSpreadRandom;
        }
}