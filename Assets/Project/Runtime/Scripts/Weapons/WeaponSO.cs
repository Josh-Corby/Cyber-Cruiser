using CyberCruiser.Audio;
using UnityEngine;

namespace CyberCruiser
{
    [CreateAssetMenu]
    public class WeaponSO : ScriptableObject
    {
        public string WeaponName;
        public string WeaponDescription;
        public Sprite WeaponUISprite;
        public Sprite MuzzleFlash;

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