using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunshipMovement : EnemyMovement
{
    public enum GunshipMoveTypes
    {
        PlayerFollow, SlowPlayerFollow, UpDown
    }

    public GunshipMoveTypes gunshipMoveType;

    private readonly float fastSeekSpeed = 6;
    private readonly float slowSeekSpeed = 3;

    protected override void Start()
    {
        ChooseMovementType();
        base.Start();
    }

    private void ChooseMovementType()
    {
        //check how many other gunships are currently alive to check what movetype should be assigned

        if (EM.gunshipsAlive.Count == 1)
        {
            gunshipMoveType = GunshipMoveTypes.PlayerFollow;
            SetEnemyMovementType(gunshipMoveType);
        }

        //if there are currently any other gunships alive go through them to check what movetype should be assigned to this gunship
        else if (EM.gunshipsAlive.Count > 1)
        {
            CheckForPlayerFollow();
        }
    }

    private void CheckForPlayerFollow()
    {
        //check if any of the other currently alive gunships are directly following the player
        foreach (GameObject gunship in EM.gunshipsAlive)
        {
            if (gunship.GetComponent<GunshipMovement>().gunshipMoveType == GunshipMoveTypes.PlayerFollow)
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
        foreach (GameObject gunship in EM.gunshipsAlive)
        {
            if (gunship.GetComponent<GunshipMovement>().gunshipMoveType == GunshipMoveTypes.SlowPlayerFollow)
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
        switch (gunshipMoveType)
        {
            case GunshipMoveTypes.PlayerFollow:
                _seekPlayerY = true;
                _seekSpeed = fastSeekSpeed;
                break;

            case GunshipMoveTypes.SlowPlayerFollow:
                _seekPlayerY = true;
                _seekSpeed = slowSeekSpeed;
                break;
            case GunshipMoveTypes.UpDown:
                _upDownMovement = true;
                _upDownSpeed = _speed;
                _upDownMoveDistance = 1;
                break;
        }
    }
}
