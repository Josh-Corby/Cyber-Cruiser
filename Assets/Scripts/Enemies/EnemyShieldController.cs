using UnityEngine;

public class EnemyShieldController : ShieldControllerBase, IShield
{
    [SerializeField] protected int maxShieldStrength;
    [SerializeField] protected float CurrentShieldStrength;

    private const string ENEMY_PROJECTILE_LAYER_NAME = "EnemyProjectile";

    public override void ActivateShields()
    {
        CurrentShieldStrength = maxShieldStrength;
        base.ActivateShields();
    }

    public override void DeactivateShields()
    {
        base.DeactivateShields();
    }

    public override void ReduceShields()
    {
        CurrentShieldStrength -= 1;
        if(CurrentShieldStrength <= 0)
        {   
            DeactivateShields();
        }
    }

    public override void ReflectProjectile(Bullet bulletToReflect)
    {
        base.ReflectProjectile(bulletToReflect);
        bulletToReflect.gameObject.layer = LayerMask.NameToLayer(ENEMY_PROJECTILE_LAYER_NAME);
        ReduceShields();

    }

}

