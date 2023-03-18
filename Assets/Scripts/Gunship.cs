using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Gunship : Enemy, IDamageable
{
    public static event Action<List<GameObject>, GameObject> OnGunshipSpawn = null;
    public static event Action<List<GameObject>, GameObject> OnGunshipDied = null;

    public enum GunshipMoveTypes
    {
        PlayerFollow, SlowPlayerFollow, UpDown
    }

    public GunshipMoveTypes gunshipMoveType;

    private float fastSeekSpeed = 4;
    private float slowSeekSpeed = 2;

    protected override void Start()
    {
        moveDirection = MovementDirection.Left;
        moveType = MovementTypes.Forward;
        ChooseMovementType();

        base.Start();

        OnGunshipSpawn(ESM.gunshipsAlive, gameObject);
    }

    private void ChooseMovementType()
    {
        //check how many other gunships are currently alive to check what movetype should be assigned

        if(ESM.gunshipsAlive.Count == 0)
        {
            gunshipMoveType = GunshipMoveTypes.PlayerFollow;
            SetEnemyMovementType(gunshipMoveType);
        }

        //if there are currently any other gunships alive go through them to check what movetype should be assigned to this gunship
        else if(ESM.gunshipsAlive.Count > 0)
        {
            CheckForPlayerFollow();
        }
    }

    private void CheckForPlayerFollow()
    {
        //check if any of the other currently alive gunships are directly following the player
        foreach (GameObject gunship in ESM.gunshipsAlive)
        {
            if (gunship.GetComponent<Gunship>().gunshipMoveType == GunshipMoveTypes.PlayerFollow)
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
        foreach (GameObject gunship in ESM.gunshipsAlive)
        {
            if(gunship.GetComponent<Gunship>().gunshipMoveType == GunshipMoveTypes.SlowPlayerFollow)
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
                seekPlayer = true;
                seekSpeed = fastSeekSpeed;
                break;

            case GunshipMoveTypes.SlowPlayerFollow:
                seekPlayer = true;
                seekSpeed = slowSeekSpeed;
                break;
            case GunshipMoveTypes.UpDown:
                upDown = true;
                break;
        }
    }
    public override void Destroy()
    {
        OnGunshipDied(ESM.gunshipsAlive, gameObject);
        base.Destroy();
    }
}
