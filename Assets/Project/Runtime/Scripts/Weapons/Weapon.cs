using CyberCruiser.Audio;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CyberCruiser
{
    [RequireComponent(typeof(SoundControllerBase))]
    public class Weapon : GameBehaviour
    {
        [SerializeField] protected WeaponSO _currentWeapon;
        [SerializeField] private SoundControllerBase _soundController;
        private GameObject _firePoint;
        private Transform _firePointTransform;

        private bool _readyToFire;
        [SerializeField] protected bool _autoFire;

        public bool ReadyToFire { get => _readyToFire; }

        private void Awake()
        {
            _soundController = GetComponent<SoundControllerBase>();
            _firePoint = transform.GetChild(0).gameObject;
            _firePointTransform = _firePoint.transform;
        }

        protected virtual void OnEnable()
        {
            _readyToFire = true;
        }

        protected virtual void Update()
        {
            if (_autoFire)
            {
                if (_readyToFire)
                {
                    CheckFireTypes();
                }
            }
        }

        public void SetWeapon(WeaponSO newWeapon)
        {
            _currentWeapon = newWeapon;
        }

        public void CheckFireTypes()
        {
            _readyToFire = false;

            if (_currentWeapon.IsWeaponBurstFire)
            {
                StartCoroutine(BurstFire());
                return;
            }

            else if (_currentWeapon.IsWeaponMultiFire)
            {
                MultiFire();
            }

            else
            {
                PlayFireSound();
                SpreadCheck();
            }

            StartCoroutine(ResetShooting());
        }

        private IEnumerator BurstFire()
        {
            for (int i = 0; i < _currentWeapon.AmountOfBursts; i++)
            {
                if (_currentWeapon.IsWeaponMultiFire)
                {
                    MultiFire();
                }

                else
                {
                    SpreadCheck();
                    PlayFireSound();
                }

                yield return new WaitForSeconds(_currentWeapon.TimeBetweenBurstShots);
            }
            StartCoroutine(ResetShooting());
        }

        private void MultiFire()
        {
            PlayFireSound();
            if (_currentWeapon.IsMultiFireSpreadRandom)
            {
                for (int i = 0; i < _currentWeapon.MultiFireShots; i++)
                {
                    FireBullet(GetRandomSpreadAngle());
                }
                return;
            }

            for (int i = 0; i < _currentWeapon.MultiFireShots; i++)
            {
                Quaternion projectileSpread = GetMultiShotFixedAngle(i);
                FireBullet(projectileSpread);
            }
        }

        private void SpreadCheck()
        {
            if (!_currentWeapon.DoesWeaponUseSpread)
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
            Quaternion directionWithSpread = _firePointTransform.rotation * Quaternion.Euler(0, 0, Random.Range(-_currentWeapon.SpreadHalfAngle, _currentWeapon.SpreadHalfAngle));
            return directionWithSpread;
        }

        private Quaternion GetMultiShotFixedAngle(int multifireBulletIndex)
        {
            float totalSpreadAngle = _currentWeapon.SpreadHalfAngle * 2;
            float bulletFireAngle = totalSpreadAngle / (_currentWeapon.MultiFireShots - 1) * multifireBulletIndex;
            float finalAngle = bulletFireAngle - _currentWeapon.SpreadHalfAngle;
            Quaternion directionWithSpread = _firePointTransform.rotation * Quaternion.Euler(0, 0, finalAngle);
            return directionWithSpread;
        }

        private void FireBullet(Quaternion direction)
        {
            GameObject bullet = Instantiate(_currentWeapon.objectToFire, _firePointTransform.position, direction);
            _soundController.PlayNewClip(_currentWeapon.Clip);
        }

        private void PlayFireSound()
        {
            _soundController.PlayNewClip(_currentWeapon.Clip);
        }

        private IEnumerator ResetShooting()
        {
            yield return new WaitForSeconds(_currentWeapon.TimeBetweenShots);
            _readyToFire = true;
        }
    }
}