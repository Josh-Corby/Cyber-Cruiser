using CyberCruiser;

public interface IDamageable
{
    public void Damage(float damage, EnemyScriptableObject instigator);

    public void Destroy();
}
