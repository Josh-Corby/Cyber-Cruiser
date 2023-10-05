using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CyberCruiser
{
    public class DistanceManager : GameBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text _distanceCounterText;
        [SerializeField] private UISlider _distanceSlider;
        [SerializeField] private GameObject _distanceSliderObject;

        #region Fields
        [Header("Distance Increments")]
        [SerializeField] private int _baseDistancePerSecond = 10;
        [SerializeField] private IntValue _distancePerSecond;
        [SerializeField] private int _plasmaGenerationDistance = 100;
        [SerializeField] private int _bossDistanceIncrement = 500;

        private int _distanceInt;
        private int _plasmaDropDistance;
        private int _weaponUpgradeDropDistance;
        private int _previousBossDistance;
        private int _currentBossDistance;
        #endregion

        private Coroutine _increaseDistanceCoroutine = null;

        #region Properties
        public int DistanceInt
        {
            get => _distanceInt;
            private set
            {
                if (_distanceInt != value)
                {
                    OnDistanceTraveled?.Invoke();
                }

                _distanceInt = value;
                OnDistanceChanged?.Invoke(_distanceInt);
                UpdateDistanceSlider();
                UpdateDistanceText();
            }
        }

        #endregion

        #region Actions
        public static event Action<int> OnDistanceChanged = null;
        public static event Action OnDistanceTraveled = null;
        public static event Action OnBossDistanceReached = null;
        public static event Action OnPickupDistanceReached = null;
        public static event Action OnWeaponPackDistanceReached = null;
        #endregion

        private void Awake()
        {
            _distanceSliderObject.SetActive(false);
        }

        private void OnEnable()
        {
            Boss.OnBossTypeDied += (bossType) => StartIncreasingDistance();         
        }

        private void OnDisable()
        {
            Boss.OnBossTypeDied -= (bossType) => StartIncreasingDistance();
        }

        #region Distance Control
        public void InitialiseLevelCounting()
        {
            _distancePerSecond.Value = _baseDistancePerSecond;
            StartIncreasingDistance();
            GenerateNewPlasmaDropDistance();
            GenerateNewWeaponUpgradeDropDistance();
        }

        public void StartIncreasingDistance()
        {
            if(_increaseDistanceCoroutine != null)
            {
                StopCoroutine(_increaseDistanceCoroutine);
            }

            _increaseDistanceCoroutine = StartCoroutine(IncreaseDistance());
            _distanceSliderObject.SetActive(true);
        }

        private IEnumerator IncreaseDistance()
        {
            while (true)
            {
                DistanceInt += 1;
              
                CheckDistance();
                yield return new WaitForSeconds(1f/_distancePerSecond.Value);
            }
        }

        public void StopIncreasingDistance()
        {
            if (_increaseDistanceCoroutine != null)
            {
                StopCoroutine(_increaseDistanceCoroutine);
            }
        }

        private void CheckDistance()
        {
            if (_distanceInt <= 0)
            {
                return;
            }

            if (_distanceInt % _plasmaGenerationDistance == 0)
            {
                GenerateNewPlasmaDropDistance();
            }

            if (_distanceInt == _currentBossDistance)
            {
                BossDistanceReached();
            }

            if (_distanceInt == _plasmaDropDistance)
            {
                OnPickupDistanceReached?.Invoke();
            }

            if (_distanceInt == _weaponUpgradeDropDistance)
            {
                OnWeaponPackDistanceReached?.Invoke();
            }
        }

        private void BossDistanceReached()
        {
            _distanceSliderObject.SetActive(false);
            _previousBossDistance = _currentBossDistance;
            _currentBossDistance += _bossDistanceIncrement;
            StopIncreasingDistance();
            GenerateNewWeaponUpgradeDropDistance();
            GenerateNewPlasmaDropDistance();
            OnBossDistanceReached?.Invoke();
        }

        public void ResetValues()
        {
            StopIncreasingDistance();
            ResetDistanceSlider();
            DistanceInt = 0;
            _previousBossDistance = 0;
            _currentBossDistance = _bossDistanceIncrement;
        }
        #endregion

        #region Pickup Distance Management
        private void GenerateNewPlasmaDropDistance()
        {
            _plasmaDropDistance = Random.Range(_distanceInt + 15, _distanceInt + 99);
        }

        private void GenerateNewWeaponUpgradeDropDistance()
        {
            _weaponUpgradeDropDistance = Random.Range(_previousBossDistance + 15, _currentBossDistance - 1);
        }
        #endregion

        #region UI
        private void ResetDistanceSlider()
        {
            _distanceSlider.EnableAndSetSlider(0, 0, _currentBossDistance);
        }

        private void UpdateDistanceSlider()
        {
            _distanceSlider.EnableAndSetSlider(DistanceInt, _previousBossDistance, _currentBossDistance);
        }

        private void UpdateDistanceText()
        {
            _distanceCounterText.text = _distanceInt.ToString();
        }
        #endregion
    }
}