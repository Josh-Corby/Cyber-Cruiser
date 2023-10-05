using UnityEngine;

namespace CyberCruiser
{
    public class EnemyWeapon : Weapon
    {
        [SerializeField] private BoolReference _isPlayerInvisible;
        [SerializeField] private BoolReference _isTimeStopped;
        [SerializeField] protected EnemyScriptableObject _owner;

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

        protected override void FireBullet(Quaternion direction)
        {
            GameObject spawnedObject = _currentWeapon.objectToFire;
            if (spawnedObject.TryGetComponent<Bullet>(out var enemyBullet))
            {
                enemyBullet.Owner = _owner;
            }

            else if(spawnedObject.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.Owner = _owner;
            }

            Instantiate(spawnedObject, _firePointTransform.position, direction);
            _soundController.PlayNewClip(_currentWeapon.Clip);
        }
    }
}