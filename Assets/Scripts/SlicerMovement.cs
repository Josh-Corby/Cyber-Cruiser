using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SlicerMovement : EnemyMovement
{
    #region Properties
    private float SeekTime { get; }

    private float SeekCounter { get; set; }
    #endregion

    #region Actions
    public static event Action<GameObject> OnStartSeeking = null;
    #endregion

    protected override void Start()
    {
        GetMovementType();
    }

    protected override void Update()
    {
        if (_seekPlayer)
        {
            if (SeekCounter <= 0)
            {
                _seekPlayer = false;
                return;
            }
            SeekX();
            SeekCounter -= Time.deltaTime;
            return;
        }
        base.Update();
    }

    private void GetMovementType()
    {
        //if only one slicer was spawned
        if (EM.slicersSeeking.Count == 0)
        {
            _seekPlayer = true;
            _seekPlayerX = true;
            SeekCounter = SeekTime;
            OnStartSeeking(gameObject);
            return;
        }

        //if more than one slicer was spawned
        if (EM.slicersSeeking.Count > 0)
        {
            //check all slicers spawned to see if any of them are seeking the player
            foreach (SlicerMovement slicer in EM.slicersSeeking)
            {
                //skip this slicer
                if (slicer == this) continue;

                //if any slicer is seeking the player
                if (slicer._seekPlayer)
                {
                    //don't seek the player
                    _seekPlayer = false;
                    return;
                }
            }
            //if none of the slicers are seeking the player then seek
            _seekPlayer = true;
            OnStartSeeking(gameObject);
        }
    }
}
