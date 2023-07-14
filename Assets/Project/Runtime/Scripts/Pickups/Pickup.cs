using System;
using System.Collections.Generic;
using UnityEngine;

namespace CyberCruiser
{
    public class Pickup : MonoBehaviour
    {
        #region Fields
        [SerializeField] private string _pickupName;
        public PickupType _pickupType;
        public WeaponSO _weaponSO;
        [SerializeField] private float _speed;

        [SerializeField] private IntReference _healthOnPickup;
        [SerializeField] private IntReference _plasmaOnPickup;
        [SerializeField] private IntReference _ionOnPickup;
        [SerializeField] private GameEvent _pickupEvent;

        #endregion

        #region Actions
        public static event Action<int, int, int> OnResourcePickup = null;
        public static event Action<GameObject> OnPickedUp = null;
        public static event Action<WeaponSO> OnWeaponUpgradePickup = null;
        public static event Action<string> OnBossPickup = null;
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
                    OnBossPickup?.Invoke(_pickupName);
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

            OnPickedUp(gameObject);
            Destroy(gameObject);
        }

        private void Update()
        {
            MoveForward();
        }

        private void MoveForward()
        {
            transform.position += _speed * Time.deltaTime * transform.up;
        }
    }

        public enum PickupType
        {
            Normal, Boss, Weapon
        }
}