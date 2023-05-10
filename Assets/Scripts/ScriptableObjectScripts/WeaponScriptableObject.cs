using UnityEngine;

[CreateAssetMenu(fileName = "WeaponInfo", menuName = "ScriptableObject/Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
    public string weaponName;
    public GameObject objectToFire;
    public bool isObjectEnemy;
    public float timeBetweenShots;
    public bool holdToFire;
    public bool useSpread;
    public float spreadAngle;
    public bool burstFire;
    public int bursts;
    public float timeBetweenBurstShots;
    public bool multiFire;
    public int multiFireShots;
    public bool isMultiFireSpreadRandom;
}
