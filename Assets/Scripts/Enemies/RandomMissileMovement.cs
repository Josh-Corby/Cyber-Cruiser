using UnityEngine;

public class RandomMissileMovement : EnemyMovement
{
    protected override void Start()
    {
        ChooseRandomMovementType();
        base.Start();
    }

    private void ChooseRandomMovementType()
    {
        int movementTypeID = Random.Range(0, 2);

        if (movementTypeID == 0)
        {
            _homeOnPlayer = false;
        }

        if (movementTypeID == 1)
        {
            _homeOnPlayer = true;
        }
    }
}
