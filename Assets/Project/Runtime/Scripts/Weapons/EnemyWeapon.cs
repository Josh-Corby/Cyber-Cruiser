using UnityEngine;

namespace CyberCruiser
{
    public class EnemyWeapon : Weapon
    {
        [SerializeField] private BoolReference _isPlayerInvisible;
        [SerializeField] private BoolReference _isTimeStopped;

        protected override void OnEnable()
        {
            _autoFire = _currentWeapon.IsWeaponAutomatic;
            base.OnEnable();
        }

        protected override void Update()
        {
            if (_isPlayerInvisible.Value || _isTimeStopped.Value)
            {
                return;
            }

            base.Update();
        }
    }
}