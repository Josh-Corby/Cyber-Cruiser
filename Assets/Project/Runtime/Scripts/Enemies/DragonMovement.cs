using UnityEngine;

namespace CyberCruiser
{
    public class DragonMovement : EnemyMovement
    {
        //0 for move to bottom right
        //1 for arc movement
        [SerializeField] private bool arcMove = false;

        [Range(50, 360)] private float rotationSpeed;

        [SerializeField] private Vector3 startRotation;
        private Vector3 _targetRotation = new(0, 180, 0);
        protected override void Start()
        {
            startRotation = new Vector3(0, transform.eulerAngles.y, 0);

            ChooseRandomMoveType();
        }

        protected override void Update()
        {
            if (arcMove)
            {
                Quaternion targetRotation = Quaternion.Euler(_targetRotation);
                Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(startRotation.x, startRotation.y, rotation.eulerAngles.z);
            }
            base.Update();
        }

        private void ChooseRandomMoveType()
        {
            int movementTypeID = Random.Range(0, 2);

            if (movementTypeID == 0)
            {
                //Code that I couldn't get to work
                //bottomLeftPoint = ESM.dragonMovePoint;
                //Vector3 vectorToBottomLeft = bottomLeftPoint.position - transform.position;
                //float angle = Mathf.Atan2(vectorToBottomLeft.y, -vectorToBottomLeft.x) * Mathf.Rad2Deg;

                //Hard coded solution
                transform.eulerAngles = new Vector3(0, 180, -29);
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
}