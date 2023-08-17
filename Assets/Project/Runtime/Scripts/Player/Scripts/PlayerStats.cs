using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberCruiser
{
    public class PlayerStats : MonoBehaviour
    {
        [Header("Base Stats")]
        [SerializeField] private int _baseWeaponMaxHeat;
        [SerializeField] private int _baseWeaponUpgradeDurationInSeconds;
        [SerializeField] private int _baseHealthOnPickup;
        [SerializeField] private int _basePlasmaOnPickup;
        [SerializeField] private int _baseRamDamage;
        [SerializeField] private IntValue _basePlasmaCost;
        [SerializeField] private float _baseHeatLossPerFrame;
        [SerializeField] private float _baseCooldownHeatLossPerFrame;
        [SerializeField] private FloatValue _baseHeatPerShot;

        [Header("Current Stats")]
        [SerializeField] private IntValue _currentWeaponUpgradeDurationInSeconds;
        [SerializeField] private IntValue _currentRamDamage;
        [SerializeField] private IntValue _currentHealthOnPickup;
        [SerializeField] private IntValue _currentPlasmaOnPickup;
        [SerializeField] private IntValue _currentPlasmaCost;
        [SerializeField] private FloatValue _currentHeatPerShot;

        [Space]
        [Tooltip("Current active state of toggleable addons")]
        [SerializeField] private BoolValue[] _pickupStates;

        private void OnEnable()
        {
            ResetStats();
        }

        private void ResetStats()
        {
            _currentRamDamage.Value = _baseRamDamage;
            _currentHealthOnPickup.Value = _baseHealthOnPickup;
            _currentPlasmaOnPickup.Value = _basePlasmaOnPickup;
            _currentWeaponUpgradeDurationInSeconds.Value = _baseWeaponUpgradeDurationInSeconds;
            _currentHeatPerShot.Value = _baseHeatPerShot.Value;
            _currentPlasmaCost.Value = _basePlasmaCost.Value;
            ResetBools();
        }

        private void ResetBools()
        {
            for (int i = 0; i < _pickupStates.Length; i++)
            {
                _pickupStates[i].Value = false;
            }
        }
    }
}
