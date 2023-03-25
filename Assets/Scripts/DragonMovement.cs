using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonMovement : EnemyMovement
{
    //0 for move to bottom right
    //1 for arc movement
    [SerializeField] private bool arcMove = false;

    [HideInInspector] public Vector3 bottomLeftPoint;

    [SerializeField] private float targetRotation;
    [Range(50,360)] private float rotationSpeed;

    protected override void Start()
    {
        ChooseRandomMoveType();
        base.Start();
    }

    protected override void Update()
    {
        if (arcMove)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, targetRotation), rotationSpeed * Time.deltaTime);
        }

        base.Update();
    }

    private void ChooseRandomMoveType()
    {
        int movementTypeID = Random.Range(0, 2);

        if (movementTypeID == 0)
        {
            //move to bottom left
            bottomLeftPoint = ESM.dragonMovePoint.position;

            Vector3 vectorToBottomLeft = bottomLeftPoint - transform.position;
            float angle = Mathf.Atan2(vectorToBottomLeft.y, vectorToBottomLeft.x) * Mathf.Rad2Deg - 90;
            transform.rotation = Quaternion.Euler(0,0,angle);
        }

        if (movementTypeID == 1)
        {
            arcMove = true;
            rotationSpeed = 50;
            //rotationSpeed = Random.Range(50, 361);
        }
    }
}
