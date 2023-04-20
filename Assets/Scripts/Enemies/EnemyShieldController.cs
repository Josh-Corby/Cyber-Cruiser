using UnityEngine;

public class EnemyShieldController : ShieldControllerBase, IShield
{
    private const string ENEMY_PROJECTILE_LAYER_NAME = "EnemyProjectile";

    public override void ReflectProjectile(Bullet bulletToReflect)
    {

        if (bulletToReflect.IsHoming)
        {
            bulletToReflect.IsHoming = false;
        }
        base.ReflectProjectile(bulletToReflect);
        bulletToReflect.gameObject.layer = LayerMask.NameToLayer(ENEMY_PROJECTILE_LAYER_NAME);
        //bulletToReflect.spriteRenderer.color = Color.black;    
        ReduceShields(bulletToReflect.damage);
    }

}

