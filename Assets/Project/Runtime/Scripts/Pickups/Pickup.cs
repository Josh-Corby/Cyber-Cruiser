using System;
using UnityEngine;

namespace CyberCruiser
{
    public class Pickup : MonoBehaviour
    {
        #region Fields
        [SerializeField] private AddOnSO _pickupSO;
        private PickupInfo _info;
        public PickupType _pickupType;
        public WeaponSO _weaponSO;

        private bool _isAttracted;
        private Transform _attractorTransform;

        [SerializeField] private SpriteRenderer _plasmaTutorialSprite;

        [SerializeField] private float _speed;
        [SerializeField] private IntReference _healthOnPickup;
        [SerializeField] private IntReference _plasmaOnPickup;
        [SerializeField] private IntReference _ionOnPickup;
        #endregion

        public PickupInfo Info { get => _info; }

        #region Actions
        public static event Action<int, int, int> OnResourcePickup = null;
        public static event Action<Pickup> OnPickedUp = null;
        public static event Action<WeaponSO> OnWeaponUpgradePickup = null;
        public static event Action<PickupInfo> OnBossPickup = null;
        public static event Action<Pickup> OnPlasmaPickupSpawned = null;
        #endregion

        private void Awake()
        {
            if(_pickupSO != null)
            {
                _info = _pickupSO.Info;
            }
        }

        private void Start()
        {
            if(_pickupType == PickupType.Normal)
            {
                OnPlasmaPickupSpawned?.Invoke(this);
            }
        }

        public void PickupEffect()
        {
            if(_pickupSO != null)
            {
                _pickupSO.OnPickedUp();
            }

            OnResourcePickup?.Invoke(_healthOnPickup.Value, _plasmaOnPickup.Value, _ionOnPickup.Value);

            switch (_pickupType)
            {
                case PickupType.Boss:
                    OnBossPickup?.Invoke(Info);
                    break;

                case PickupType.Weapon:
                    OnWeaponUpgradePickup?.Invoke(_weaponSO);
                    break;
            }

            OnPickedUp?.Invoke(this);

            if(_pickupType == PickupType.Boss)
            {
                return;
            }

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

        public void EnablePlasmaTutorial()
        {
            if(_plasmaTutorialSprite != null)
            {
                _plasmaTutorialSprite.enabled = true;
                Debug.Log("Enabling plasma tutorial sprite");
            }
        }

        public void DisablePlasmaTutorial()
        {
            if (_plasmaTutorialSprite != null)
            {
                _plasmaTutorialSprite.enabled = false;
                Debug.Log("Enabling plasma tutorial sprite");
            }
        }
    }

    public enum PickupType
    {
        Normal, Boss, Weapon
    }

    [Serializable]
    public struct PickupInfo
    {
        public string Name;
        public string Description;
        public Sprite Sprite;
        public Sprite Popup;
    }
}