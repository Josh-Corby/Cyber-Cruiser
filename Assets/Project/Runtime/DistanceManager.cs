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
        private float _currentDistancePerSecond;
        [SerializeField] private float _baseDistancePerSecond = 10;
        [SerializeField] private float _thrusterBoostDistancePerSecond = 20;
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
        private int DistanceInt
        {
            get => _distanceInt;
            set
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
        #endregion

        [Header("Game Events")]
        [SerializeField] private GameEvent OnPickupDistanceReached;
        [SerializeField] private GameEvent OnWeaponPackDistanceReached;

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
            SetDistancePerSecond(_baseDistancePerSecond);
            StartIncreasingDistance();
            GenerateNewPlasmaDropDistance();
            GenerateNewWeaponUpgradeDropDistance();
        }

        public void StartIncreasingDistance()
        {
            _increaseDistanceCoroutine = StartCoroutine(IncreaseDistance());
            _distanceSliderObject.SetActive(true);
        }

        private IEnumerator IncreaseDistance()
        {
            while (true)
            {
                DistanceInt += 1;
              
                CheckDistance();
                yield return new WaitForSeconds(1 / _currentDistancePerSecond);
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
                OnPickupDistanceReached.Raise();
            }

            if (_distanceInt == _weaponUpgradeDropDistance)
            {
                OnWeaponPackDistanceReached.Raise();
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

        private void SetDistancePerSecond(float newDistancePerSecond)
        {
            _currentDistancePerSecond = newDistancePerSecond;
        }

        public void ThrusterBoostUpgrade()
        {
            SetDistancePerSecond(_thrusterBoostDistancePerSecond);
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