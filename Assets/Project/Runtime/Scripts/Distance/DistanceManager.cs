using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CyberCruiser
{
    public class DistanceManager : GameBehaviour
    {
        #region References
        [SerializeField] private TMP_Text _distanceCounterText;
        [SerializeField] private UISlider _distanceSlider;
        #endregion

        #region Fields
        [Header("Check Distances")]
        [SerializeField] private int _bossSpawnDistance = 500;
        private const int MILESTONE_DISTANCE = 100;

        [Header("General Distance")]
        [SerializeField] private float _distanceFloat;
        [SerializeField] private int _distanceInt;
        [SerializeField] private bool _isDistanceIncreasing;

        [Header("Boss distances")]
        private int _previousBossDistance, _currentBossDistance;

        [Header("Distance Milestones")]
        private int _currentDistanceMilestone;
        private bool isDistanceMilestoneIncreased;

        [Header("Pickup values")]
        [SerializeField] private int _plasmaDropDistance, _weaponUpgradeDropDistance;
        private bool _isPlasmaSpawned, _isWeaponUpgradeSpawned = false;
        #endregion

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
                UpdateDistanceText();
            }
        }
        #endregion

        #region Actions
        public static event Action<PickupType> OnPlasmaDistanceReached = null;
        public static event Action<int> OnDistanceChanged = null;
        public static event Action OnDistanceTraveled = null;
        public static event Action OnWeaponUpgradeDistanceReached = null;
        public static event Action OnBossDistanceReached = null;
        #endregion

        private void OnEnable()
        {
            Boss.OnBossDiedPosition += (p, v) => StartIncreasingDistance();
            PickupManager.OnPlasmaSpawned += () => StartCoroutine(WaitToResetPlasmaSpawned());
            PickupManager.OnWeaponUpgradeSpawned += () => StartCoroutine(WaitToResetPickupSpawned());
        }

        private void OnDisable()
        {
            Boss.OnBossDiedPosition -= (p, v) => StartIncreasingDistance();
            PickupManager.OnPlasmaSpawned -= () => StartCoroutine(WaitToResetPlasmaSpawned());
            PickupManager.OnWeaponUpgradeSpawned -= () => StartCoroutine(WaitToResetPickupSpawned());
        }

        private void Start()
        {
            _isDistanceIncreasing = false;
        }

        private void Update()
        {
            if (!_isDistanceIncreasing) return;

            IncreaseDistance();
        }


        private void IncreaseDistance()
        {
            _distanceFloat += Time.deltaTime * 10;
            DistanceInt = (int)_distanceFloat;

            CheckDistance();
        }

        private void CheckDistance()
        {
            if(_distanceInt <=0)
            {
                return;
            }

            //increment distance milestone every 100 units
            if (_distanceInt % MILESTONE_DISTANCE == 0 && !isDistanceMilestoneIncreased)
            {
                StartCoroutine(IncreaseDistanceMilestone());
                GenerateNewPlasmaDropDistance();
            }

            //start boss fight at boss distance
            if (_distanceInt > 0)
            {
                if (_distanceInt % _currentBossDistance == 0)
                {
                    BossDistanceReached();
                }
            }

            //spawn plasma at seeded distance
            if (_distanceInt == _plasmaDropDistance && !_isPlasmaSpawned)
            {
                PlasmaDistanceReached();
            }

            //spawn weapon pack at seeded distance
            if (_distanceInt == _weaponUpgradeDropDistance && !_isWeaponUpgradeSpawned)
            {
                WeaponUpgradeDistanceReached();
            }
        }


        private void PlasmaDistanceReached()
        {
            _isPlasmaSpawned = true;
            OnPlasmaDistanceReached(PickupType.Plasma);
        }

        private void WeaponUpgradeDistanceReached()
        {
            _isWeaponUpgradeSpawned = true;
            OnWeaponUpgradeDistanceReached?.Invoke();
        }

        private IEnumerator WaitToResetPlasmaSpawned()
        {
            yield return new WaitForSeconds(0.1f);
            _isPlasmaSpawned = false;
        }

        private IEnumerator WaitToResetPickupSpawned()
        {
            yield return new WaitForSeconds(0.1f);
            _isWeaponUpgradeSpawned = false;
        }

 

        private void GenerateNewPlasmaDropDistance()
        {
            _plasmaDropDistance = Random.Range(_currentDistanceMilestone + 15, _currentDistanceMilestone + 99);
            _isPlasmaSpawned = false;
        }

        private void GenerateNewWeaponUpgradeDropDistance()
        {
            _weaponUpgradeDropDistance = Random.Range(_previousBossDistance + 15, _currentBossDistance);
            Debug.Log(_weaponUpgradeDropDistance);
            _isWeaponUpgradeSpawned = false;
        }


        private IEnumerator IncreaseDistanceMilestone()
        {
            isDistanceMilestoneIncreased = true;
            _currentDistanceMilestone += MILESTONE_DISTANCE;

            yield return new WaitForSeconds(1f);
            isDistanceMilestoneIncreased = false;
        }

        private void BossDistanceReached()
        {
            StopIncreasingDistance();
            _previousBossDistance = _currentBossDistance;
            _currentBossDistance += _bossSpawnDistance;    
            GenerateNewWeaponUpgradeDropDistance();
            GenerateNewPlasmaDropDistance();
            OnBossDistanceReached?.Invoke();
        }

        public void ResetValues()
        {
            _currentDistanceMilestone = 0;
            _previousBossDistance = 0;
            _distanceFloat = 0;
            DistanceInt = 0;
            _currentBossDistance = _bossSpawnDistance;
            isDistanceMilestoneIncreased = false;
            _isDistanceIncreasing = false;
        }

        public void StartIncreasingDistance()
        {
            _distanceFloat += 1;
            _distanceInt += 1;
            _isDistanceIncreasing = true;
            GenerateNewPlasmaDropDistance();
            GenerateNewWeaponUpgradeDropDistance();
        }

        public void StopIncreasingDistance()
        {
            _isDistanceIncreasing = false;
        }


        #region UI
        private void ResetDistanceSlider()
        {
            _distanceSlider.EnableAndSetSlider(0, 0, _currentBossDistance);
        }

        private void IncrementDistanceSlider()
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