using UnityEngine;

namespace CyberCruiser
{
    [CreateAssetMenu(fileName = "WeaponInfo", menuName = "ScriptableObject/Weapon")]
    public class WeaponScriptableObject : ScriptableObject
    {
        public WeaponStats Stats;
    }
}