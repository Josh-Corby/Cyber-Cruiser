using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberCruiser
{
    public class PlayerStats : MonoBehaviour
    {
        [Header ("Base Stats")]
        [SerializeField] private IntValue _baseWeaponMaxHeat;
        [SerializeField] private IntValue _baseWeaponUpgradeDurationInSeconds;
        [SerializeField] private FloatValue _baseHeatPerShot;
        [SerializeField] private FloatValue _baseHeatLossPerFrame;
        [SerializeField] private FloatValue _baseCooldownHeatLossPerFrame;

        [Header("Current Stats")]
        [SerializeField] private IntValue _currentWeaponUpgradeDurationInSeconds;
        [SerializeField] private BoolValue _doesPlayerHavePulseDetonator;


        [Header("Addon Modifiers")]
        [SerializeField] private IntValue _BatteryPackUpgradeValue;

        private void OnEnable()
        {
            ResetStats();
        }

        private void ResetStats()
        {
            //_currentWeaponUpgradeDurationInSeconds.Value = _baseWeaponUpgradeDurationInSeconds.Value;
            _doesPlayerHavePulseDetonator.Value = false;
        }
    }
}
