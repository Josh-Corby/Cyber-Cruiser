using UnityEngine;

namespace CyberCruiser
{
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
                _isEnemyHomingOnPlayer = false;
            }

            if (movementTypeID == 1)
            {
                _isEnemyHomingOnPlayer = true;
            }
        }
    }
}