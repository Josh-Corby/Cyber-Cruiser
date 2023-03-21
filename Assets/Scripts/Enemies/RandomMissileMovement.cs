using UnityEngine;

public class RandomMissileMovement : EnemyMovement
{
    private float missileTurnSpeed = 5;
    private float missileHomeTime = 2;

    protected override void Start()
    {
        ChooseRandomMovementType();
        base.Start();
    }

    private void ChooseRandomMovementType()
    {
        int movementTypeID = Random.Range(0, 2);

        if(movementTypeID == 0)
        {
            homeOnPlayer = false;
        }

        if(movementTypeID == 1)
        {
            homeOnPlayer = true;
            turnSpeed = missileTurnSpeed;
            homeTime = missileHomeTime;
        }
    }
}
