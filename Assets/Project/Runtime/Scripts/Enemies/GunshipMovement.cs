using System;
using UnityEngine;

namespace CyberCruiser
{
    public class GunshipMovement : EnemyMovement
    {
        public enum GunshipMoveTypes
        {
            PlayerFollow, SlowPlayerFollow, UpDown
        }

        public GunshipMoveTypes gunshipMoveType;

        private readonly float fastSeekSpeed = 6;
        private readonly float slowSeekSpeed = 3;

        public static event Action<GameObject> OnGunshipSpawned = null;

        protected override void Awake()
        {
            base.Awake();
            OnGunshipSpawned(gameObject);
        }
        protected override void Start()
        {
            ChooseMovementType();
            //base.Start();
        }

        //check how many other gunships are currently alive to check what movetype should be assigned
        private void ChooseMovementType()
        {
            //if this gunship is the only one in the list default
            if (EnemyManagerInstance.GunshipsAlive.Count == 1)
            {
                gunshipMoveType = GunshipMoveTypes.PlayerFollow;
                SetEnemyMovementType(gunshipMoveType);
            }

            //if there are currently any other gunships alive go through them to check what movetype should be assigned to this gunship
            else if (EnemyManagerInstance.GunshipsAlive.Count > 1)
            {
                CheckForPlayerFollow();
            }
        }

        private void CheckForPlayerFollow()
        {
            //check if any of the other currently alive gunships are directly following the player
            foreach (GunshipMovement gunship in EnemyManagerInstance.GunshipsAlive)
            {
                if (gunship == this) continue;

                if (gunship.gunshipMoveType == GunshipMoveTypes.PlayerFollow)
                {
                    //if any of the gunships are directly following the player move on to slow follow check
                    CheckForSlowPlayerFollow();
                    return;
                }
            }
            //if none of the gunships are directly following the player, this gunships movetype is set to direct follow
            gunshipMoveType = GunshipMoveTypes.PlayerFollow;
            SetEnemyMovementType(gunshipMoveType);
        }

        private void CheckForSlowPlayerFollow()
        {
            //chekc if any of the other currently alive gunships are slow following the player
            foreach (GunshipMovement gunship in EnemyManagerInstance.GunshipsAlive)
            {
                if (gunship == this) continue;

                if (gunship.gunshipMoveType == GunshipMoveTypes.SlowPlayerFollow)
                {
                    //if any of the gunships are slow following the player, this gunships movetype is set to updown
                    gunshipMoveType = GunshipMoveTypes.UpDown;
                    SetEnemyMovementType(gunshipMoveType);
                    return;
                }
            }
            //if none ofthe gunshisp are slow following the player, this gunshisp movetype is set to slow follow
            gunshipMoveType = GunshipMoveTypes.SlowPlayerFollow;
            SetEnemyMovementType(gunshipMoveType);
        }

        private void SetEnemyMovementType(GunshipMoveTypes gunshipMoveType)
        {
            _isEnemyMovingUpDown = false;
            _isEnemySeekingPlayer = false;

            switch (gunshipMoveType)
            {
                case GunshipMoveTypes.PlayerFollow:
                    _isEnemySeekingPlayer = true;
                    _isEnemySeekingPlayerOnYAxis = true;
                    _seekSpeed = fastSeekSpeed;
                    break;

                case GunshipMoveTypes.SlowPlayerFollow:
                    _isEnemySeekingPlayer = true;
                    _isEnemySeekingPlayerOnYAxis = true;
                    _seekSpeed = slowSeekSpeed;
                    break;
                case GunshipMoveTypes.UpDown:
                    _isEnemyMovingUpDown = true;
                    _upDownSpeed = _speed;
                    _upDownDistance = 5;
                    break;
            }
        }
    }
}