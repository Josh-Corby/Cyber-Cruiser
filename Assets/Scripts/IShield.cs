public interface IShield
{
    public void ReduceShields(float damage);

    public void ActivateShields();

    public void DeactivateShields();

    public void ReflectProjectile(Bullet bulletToReflect);

    public int ShieldMaxStrength { get; set; }

    public float ShieldCurrentStrength { get; set; }

    public float ShieldCollisionDamage { get; set; }
}
