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

    public GunshipMoveTypes moveType;

    protected override void Start()
    {
        base.Start();

        ChooseMovementType();

        OnGunshipSpawn(ESM.gunshipsAlive, gameObject);
    }

    private void ChooseMovementType()
    {
        //check how many other gunships are currently alive to check what movetype should be assigned

        if(ESM.gunshipsAlive.Count == 0)
        {
            moveType = GunshipMoveTypes.PlayerFollow;
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
            if (gunship.GetComponent<Gunship>().moveType == GunshipMoveTypes.PlayerFollow)
            {
                //if any of the gunships are directly following the player move on to slow follow check
                CheckForSlowPlayerFollow();
                return;
            }
        }
        //if none of the gunships are directly following the player, this gunships movetype is set to direct follow
        moveType = GunshipMoveTypes.PlayerFollow;
    }

    private void CheckForSlowPlayerFollow()
    {
        //chekc if any of the other currently alive gunships are slow following the player
        foreach (GameObject gunship in ESM.gunshipsAlive)
        {
            if(gunship.GetComponent<Gunship>().moveType == GunshipMoveTypes.SlowPlayerFollow)
            {
                //if any of the gunships are slow following the player, this gunships movetype is set to updown
                moveType = GunshipMoveTypes.UpDown;
                return;
            }
        }
        //if none ofthe gunshisp are slow following the player, this gunshisp movetype is set to slow follow
        moveType = GunshipMoveTypes.SlowPlayerFollow;
    }


    public override void Destroy()
    {
        OnGunshipDied(ESM.gunshipsAlive, gameObject);
        base.Destroy();
    }
}
