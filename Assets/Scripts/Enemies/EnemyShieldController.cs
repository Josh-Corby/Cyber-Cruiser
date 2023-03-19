using UnityEngine;

public class EnemyShieldController : ShieldControllerBase, IShield
{
    [SerializeField] protected int maxShieldStrength;
    [SerializeField] protected float CurrentShieldStrength;

    public override void ActivateShields()
    {
        CurrentShieldStrength = maxShieldStrength;
        base.ActivateShields();
    }

    public override void DeactivateShields()
    {
        base.DeactivateShields();
    }

    public void ReduceShields(float shieldDamage)
    {
        CurrentShieldStrength -= shieldDamage;
        if(CurrentShieldStrength <= 0)
        {
            DeactivateShields();
        }
    }
}
