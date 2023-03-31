using UnityEngine;
public interface IShield
{
    public void ActivateShields();

    public void DeactivateShields();

    public void ReduceShields(float damage);

    public void ReflectProjectile(Bullet bulletToReflect);

    public int ShieldMaxStrength { get; set; }

    public float ShieldCurrentStrength { get; set; }

    public float ShieldCollisionDamage { get; set; }

    public float ShieldRendererMaxAlpha { get; set; }

    public float ShieldRendererCurrentAlpha { get; set; }


}
