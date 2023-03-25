using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponInfo", menuName = "ScriptableObject/Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
    public string weaponName;
    public GameObject objectToFire;
    public float timeBetweenShots;
    public bool holdToFire;
    public bool useSpread;
    public float spreadAngle;
    public bool burstFire;
    public int bulletsInBurst;
    public float timeBetweenBurstShots;
}
