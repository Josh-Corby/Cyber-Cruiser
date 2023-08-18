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
        [SerializeField] protected SoundControllerBase _soundController;
        private GameObject _firePoint;
        protected Transform _firePointTransform;
        private SpriteRenderer _muzzleFlashRenderer;

        private bool _readyToFire;
        [SerializeField] protected bool _autoFire;

        public bool ReadyToFire { get => _readyToFire; }

        private Coroutine _burstFireRoutine;
        private Coroutine _muzzleFlashRoutine;

        private void Awake()
        {
            _soundController = GetComponent<SoundControllerBase>();
            _firePoint = transform.GetChild(0).gameObject;
            _firePointTransform = _firePoint.transform;
            _muzzleFlashRenderer = GetComponentInChildren<SpriteRenderer>();

            //if (_muzzleFlashRenderer != null)
            //{
            //    _muzzleFlashRenderer.sprite = _currentWeapon.MuzzleFlash;
            //    _muzzleFlashRenderer.enabled = false;
            //}
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

            if (_muzzleFlashRenderer != null)
            {
                _muzzleFlashRenderer.sprite = _currentWeapon.MuzzleFlash;
            }
        }

        public void CheckFireTypes()
        {
            _readyToFire = false;

            if (_currentWeapon.IsWeaponBurstFire)
            {
                if (_burstFireRoutine != null)
                {
                    StopCoroutine(_burstFireRoutine);
                }

                _burstFireRoutine = StartCoroutine(BurstFire());
                return;
            }

            else if (_currentWeapon.IsWeaponMultiFire)
            {
                MultiFire();
            }

            else
            {
                FireEffects();
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
                    FireEffects();
                }

                yield return new WaitForSeconds(_currentWeapon.TimeBetweenBurstShots);
            }
            StartCoroutine(ResetShooting());
            StopCoroutine(BurstFire());
        }

        private void MultiFire()
        {
            FireEffects();
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

        protected virtual void FireBullet(Quaternion direction)
        {
            GameObject bullet = Instantiate(_currentWeapon.objectToFire, _firePointTransform.position, direction);
            _soundController.PlayNewClip(_currentWeapon.Clip);
        }

        private void FireEffects()
        {
            _soundController.PlayNewClip(_currentWeapon.Clip);

            if (_muzzleFlashRenderer != null)
            {
                if (_muzzleFlashRoutine != null)
                {
                    StopCoroutine(_muzzleFlashRoutine);
                }
                _muzzleFlashRoutine = StartCoroutine(MuzzleFlashRoutine());
            }
        }

        private IEnumerator MuzzleFlashRoutine()
        {
            _muzzleFlashRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
            _muzzleFlashRenderer.enabled = false;
        }

        private IEnumerator ResetShooting()
        {
            yield return new WaitForSeconds(_currentWeapon.TimeBetweenShots);
            _readyToFire = true;
        }
    }
}