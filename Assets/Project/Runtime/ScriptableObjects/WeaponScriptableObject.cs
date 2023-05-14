using UnityEngine;

[CreateAssetMenu(fileName = "WeaponInfo", menuName = "ScriptableObject/Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
    public WeaponStats Stats;
    public bool IsFireObjectAnEnemy;
}
