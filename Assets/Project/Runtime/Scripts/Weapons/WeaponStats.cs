using CyberCruiser.Audio;
using System;
using UnityEngine;

namespace CyberCruiser
{
    [Serializable]
    public class WeaponStats
    {
        public GameObject objectToFire;
        public float TimeBetweenShots;
        public bool IsWeaponAutomatic;
        public ClipInfo Clip;

        [Header("Spread")]
        public bool DoesWeaponUseSpread;
        public float SpreadHalfAngle;

        [Header("Burst Fire")]
        public bool IsWeaponBurstFire;
        public int AmountOfBursts;
        public float TimeBetweenBurstShots;

        [Header("Multi Shot")]
        public bool IsWeaponMultiFire;
        public int MultiFireShots;
        public bool IsMultiFireSpreadRandom;
    }
}