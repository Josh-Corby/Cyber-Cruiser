using UnityEngine;
public interface IShield
{
    public void ActivateShields();

    public void DeactivateShields();

    public void ReduceShields(float shieldDamage);

    public void ReflectProjectile(GameObject objectToReflect);
}
