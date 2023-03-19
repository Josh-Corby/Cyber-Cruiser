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

    public override void ReduceShields(float shieldDamage)
    {
        CurrentShieldStrength -= shieldDamage;
        if(CurrentShieldStrength <= 0)
        {
            DeactivateShields();
        }
    }

    public override void ReflectProjectile(GameObject objectToReflect)
    {
        base.ReflectProjectile(objectToReflect);
        objectToReflect.layer = LayerMask.NameToLayer(ENEMY_PROJECTILE_LAYER_NAME);
    }

}

