using UnityEngine;

public class BossMovementArea : MonoBehaviour
{
    [SerializeField] public Vector3 bossMoveArea;
    private Vector2 randomPosition;
    private float randomX;
    private float randomY;

    private void OnEnable()
    {
        BossMovement.OnMovePositionRequested += GetRandomMovePosition;
    }

    private void OnDisable()
    {
        BossMovement.OnMovePositionRequested -= GetRandomMovePosition;
    }
    public void GetRandomMovePosition(BossMovement bossRequesting, BossMovementType bossRequestingType)
    {
        switch (bossRequestingType)
        {
            case BossMovementType.UpDown:
                randomY = Random.Range(transform.position.y - (bossMoveArea.y / 2), transform.position.y + (bossMoveArea.y / 2));
                randomX = bossRequesting.gameObject.transform.position.x;
                break;
            case BossMovementType.Free:
                randomX = Random.Range(transform.position.x - (bossMoveArea.x / 2), transform.position.x + (bossMoveArea.x / 2));
                randomY = Random.Range(transform.position.y - (bossMoveArea.y / 2), transform.position.y + (bossMoveArea.y / 2));
                break;
        }

        randomPosition = new Vector2(randomX, randomY);
        bossRequesting.SetMovePosition = randomPosition;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, bossMoveArea);
    }


}
