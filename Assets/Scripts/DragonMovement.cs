using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonMovement : EnemyMovement
{
    //0 for move to bottom right
    //1 for arc movement
    [SerializeField] private bool arcMove = false;

    [HideInInspector] public Vector3 bottomLeftPoint;

    private const float TARGET_ROTATION = 90f;
    [Range(50,360)] private float rotationSpeed;

    private Vector3 startRotation;
    protected override void Start()
    {
        ChooseRandomMoveType();
        base.Start();

        float x = transform.rotation.x;
        float y = transform.rotation.y;
        float z = transform.rotation.z;
        startRotation = new Vector3 (x, y, z);
    }

    protected override void Update()
    {
        if (arcMove)
        {
            Quaternion targetRotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, TARGET_ROTATION);
            Quaternion rotation =  Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(startRotation.x, startRotation.y, rotation.eulerAngles.z);

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
            transform.rotation = Quaternion.Euler(startRotation.x, startRotation.y,angle);
        }

        if (movementTypeID == 1)
        {
            arcMove = true;
            rotationSpeed = Random.Range(50, 361);
        }
    }

    protected override void DeathMovement()
    {
        transform.position += _speed * Time.deltaTime * Vector3.down;
    }
}
