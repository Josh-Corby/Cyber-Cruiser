using System;
using System.Collections.Generic;
using UnityEngine;

namespace CyberCruiser
{
    public class Pickup : MonoBehaviour
    {
        #region Fields
        public PickupType _pickupType;
        public WeaponUpgradeType _upgradeType;
        [SerializeField] private float _speed;

        [SerializeField] private IntReference _healthOnPickup;
        [SerializeField] private IntReference _plasmaOnPickup;
        [SerializeField] private IntReference _ionOnPickup;

        #endregion

        #region Actions
        public static event Action<int, int, int> OnResourcePickup = null;
        public static event Action<GameObject> OnPickedUp = null;
        public static event Action<WeaponUpgradeType> OnWeaponUpgradePickup = null;
        #endregion

        public void PickupEffect()
        {
            Debug.Log("Picked up");
            switch (_pickupType)
            {
                case PickupType.Normal:
                case PickupType.Boss:
                    OnResourcePickup(_healthOnPickup.Value, _plasmaOnPickup.Value, _ionOnPickup.Value);
                    break;

                case PickupType.Weapon:
                    switch (_upgradeType)
                    {
                        case WeaponUpgradeType.None:
                            return;

                        default:
                            OnWeaponUpgradePickup(_upgradeType);
                            break;
                    }
                    OnResourcePickup(_healthOnPickup.Value, _plasmaOnPickup.Value, _ionOnPickup.Value);

                    break;
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

        public enum WeaponUpgradeType
        {
            None, Scatter_Random, Scatter_Fixed, Pulverizer, Homing, ChainLightning, BFG, Smart
        }
}