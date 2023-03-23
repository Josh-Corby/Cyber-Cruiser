using UnityEngine;
public interface IShield
{
    public void ActivateShields();

    public void DeactivateShields();

    public void ReduceShields();

    public void ReflectProjectile(Bullet bulletToReflect);
}
