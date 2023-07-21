using System;
using UnityEngine;

namespace CyberCruiser
{
    public class Pickup : MonoBehaviour
    {
        #region Fields
        [SerializeField] private string _pickupName;
        [SerializeField] private Sprite _pickupSprite;
        public PickupType _pickupType;
        public WeaponSO _weaponSO;

        private bool _isAttracted;
        private Transform _attractorTransform;

        [SerializeField] private float _speed;
        [SerializeField] private IntReference _healthOnPickup;
        [SerializeField] private IntReference _plasmaOnPickup;
        [SerializeField] private IntReference _ionOnPickup;
        [SerializeField] private GameEvent _pickupEvent;
        #endregion

        #region Actions
        public static event Action<int, int, int> OnResourcePickup = null;
        public static event Action<Pickup> OnPickedUp = null;
        public static event Action<WeaponSO> OnWeaponUpgradePickup = null;
        public static event Action<string, Sprite> OnBossPickup = null;
        #endregion

        public void PickupEffect()
        {
            Debug.Log("Picked up");
            switch (_pickupType)
            {
                case PickupType.Normal:
                    OnResourcePickup?.Invoke(_healthOnPickup.Value, _plasmaOnPickup.Value, _ionOnPickup.Value);
                    break;

                case PickupType.Boss:
                    OnResourcePickup?.Invoke(_healthOnPickup.Value, _plasmaOnPickup.Value, _ionOnPickup.Value);
                    OnBossPickup?.Invoke(_pickupName, _pickupSprite);
                    break;

                case PickupType.Weapon:
                    OnWeaponUpgradePickup?.Invoke(_weaponSO);
                    OnResourcePickup?.Invoke(_healthOnPickup.Value, _plasmaOnPickup.Value, _ionOnPickup.Value);
                    break;
            }

            if(_pickupEvent != null)
            {
                _pickupEvent.Raise();
            }

            OnPickedUp(this);
            Destroy(gameObject);
        }

        private void Update()
        {

            if(_isAttracted)
            {
                MoveTowardsTarget();
            }

            else
            {
                MoveForward();
            }
        }

        private void MoveForward()
        {
            transform.position += _speed * Time.deltaTime * transform.up;
        }

        public void SetAttractorTarget(GameObject target)
        {
            _isAttracted = true;
            _attractorTransform = target.transform;
        }

        private void MoveTowardsTarget()
        {
            transform.position = Vector2.MoveTowards(transform.position, _attractorTransform.position, _speed * Time.deltaTime);
        }
    }

        public enum PickupType
        {
            Normal, Boss, Weapon
        }
}