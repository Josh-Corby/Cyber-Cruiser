using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberCruiser
{
    public class PlayerStats : MonoBehaviour
    {
        [Header("Base Stats")]
        [SerializeField] private IntValue _baseWeaponMaxHeat;
        [SerializeField] private IntValue _baseWeaponUpgradeDurationInSeconds;
        [SerializeField] private IntValue _baseHealthOnPickup;
        [SerializeField] private IntValue _basePlasmaOnPickup;
        [SerializeField] private int _baseRamDamage;
        [SerializeField] private FloatValue _baseHeatPerShot;
        [SerializeField] private FloatValue _baseHeatLossPerFrame;
        [SerializeField] private FloatValue _baseCooldownHeatLossPerFrame;

        [Header("Current Stats")]
        [SerializeField] private IntValue _currentWeaponUpgradeDurationInSeconds;
        [SerializeField] private IntValue _currentRamDamage;
        [SerializeField] private IntValue _currentHealthOnPickup;
        [SerializeField] private IntValue _currentPlasmaOnPickup;
        [SerializeField] private FloatValue _currentHeatPerShot;
        [SerializeField] private BoolValue _doesPlayerHavePulseDetonator;
        [SerializeField] private BoolValue _doesPlayerShieldReflect;
        [SerializeField] private BoolValue _doesPlayerHaveSignalBeam;
        [SerializeField] private BoolValue _doesPlayerHaveEmergencyArsenal;
        [SerializeField] private BoolValue _doesPlayerHaveBackupSystem;

        [Header("Addon Modifiers")]
        [SerializeField] private IntValue _batteryPackUpgradeValue;
        [SerializeField] private IntValue _ramDamageUpgradeValue;
        [SerializeField] private IntValue _healthPickupUpgradeValue;
        [SerializeField] private IntValue _plasmaPickupUpgradeValue;

        private void OnEnable()
        {
            ResetStats();
        }

        private void ResetStats()
        {
            _currentRamDamage.Value = _baseRamDamage;
            _currentHealthOnPickup.Value = _baseHealthOnPickup.Value;
            _currentPlasmaOnPickup.Value = _basePlasmaOnPickup.Value;
            _currentWeaponUpgradeDurationInSeconds.Value = _baseWeaponUpgradeDurationInSeconds.Value;
            _currentHeatPerShot.Value = _baseHeatPerShot.Value;
            _doesPlayerHavePulseDetonator.Value = false;
            _doesPlayerShieldReflect.Value = false;
            _doesPlayerHaveSignalBeam.Value = false;
            _doesPlayerHaveEmergencyArsenal.Value = false;
            _doesPlayerHaveBackupSystem.Value = false;
        }
    }
}
