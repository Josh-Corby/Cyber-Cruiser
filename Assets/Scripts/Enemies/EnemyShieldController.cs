using UnityEngine;

public class EnemyShieldController : ShieldControllerBase, IShield
{
    public override void ReflectProjectile(Bullet bulletToReflect)
    {
        //if (bulletToReflect.IsHoming)
        //{
        //    bulletToReflect.IsHoming = false;
        //}

        //base.ReflectProjectile(bulletToReflect);
        //bulletToReflect.SwitchBulletTeam();  
        //ReduceShields(bulletToReflect.damage);
    }

}

