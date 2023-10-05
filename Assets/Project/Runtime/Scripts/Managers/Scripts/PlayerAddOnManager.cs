using System;
using System.Collections.Generic;
using UnityEngine;

namespace CyberCruiser
{
    public class PlayerAddOnManager : GameBehaviour
    {
        #region Add On Data
        [Header("Add Ons")]

        [SerializeField] private AddOnSO _attractorUnit;
        [SerializeField] private AddOnSO _backupSystem;
        [SerializeField] private AddOnSO _batteryPack;
        [SerializeField] private AddOnSO _burstVents;
        [SerializeField] private AddOnSO _chainLightning;
        [SerializeField] private AddOnSO _emergencyArsenal;
        [SerializeField] private AddOnSO _invisibilityShield;
        [SerializeField] private AddOnSO _pulseDetonator;  
        [SerializeField] private AddOnSO _reflectorShield;
        [SerializeField] private AddOnSO _shieldGenerator;
        [SerializeField] private AddOnSO _signalBeacon;  
        [SerializeField] private AddOnSO _thermalWelding;
        [SerializeField] private AddOnSO _thrusterBoost;
        [SerializeField] private AddOnSO _timeStop;
        #endregion

        #region Properties
        public AddOnSO AttractorUnit { get => _attractorUnit; }
        public AddOnSO BackupSystem { get => _backupSystem; }
        public AddOnSO BatteryPack { get  => _batteryPack; }
        public AddOnSO BurstVents { get => _burstVents; }
        public AddOnSO ChainLightning { get => _chainLightning; }
        public AddOnSO EmergencyArsenal { get => _emergencyArsenal; }
        public AddOnSO InvisibilityShield { get => _invisibilityShield; }
        public AddOnSO PulseDetonator {  get => _pulseDetonator; }
        public AddOnSO ReflectorShield {  get => _reflectorShield; }
        public AddOnSO ShieldGenerator {  get => _shieldGenerator; }
        public AddOnSO SignalBeacon { get => _signalBeacon; }
        public AddOnSO ThermalWelding {  get => _thermalWelding; }
        public AddOnSO ThrusterBoost {  get => _thrusterBoost; }
        public AddOnSO TimeStop { get => _timeStop; }
        #endregion
    }
}