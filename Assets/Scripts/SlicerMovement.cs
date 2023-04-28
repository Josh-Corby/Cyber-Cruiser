using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SlicerMovement : EnemyMovement
{
    #region Fields
    [SerializeField] private float _seekTime;
    private float _seekCounter;
    #endregion

    #region Properties
    private float SeekTime
    {
        get => _seekTime;
    }

    private float SeekCounter
    {
        get => _seekCounter;
        set => _seekCounter = value;
    }
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
        if (SeekPlayer)
        {
            if (SeekCounter <= 0)
            {
                SeekPlayer = false;
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
            SeekPlayer = true;
            SeekPlayerX = true;
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
