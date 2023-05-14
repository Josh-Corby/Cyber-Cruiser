public class EnemyWeaponController : Weapon
{
    private void Start()
    {
        EnableWeapon();
    }

    private void EnableWeapon()
    {
        _autoFire = true;
    }

    public void DisableWeapon()
    {
        _autoFire = false;
    }
}
