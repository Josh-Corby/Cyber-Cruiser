using UnityEngine;

public class BossMovementArea : MonoBehaviour
{
    [SerializeField] public Vector3 bossMoveArea;

    private void OnEnable()
    {
        BossMovement.OnMovePositionRequested += GetRandomMovePosition;
    }

    private void OnDisable()
    {
        BossMovement.OnMovePositionRequested -= GetRandomMovePosition;
    }
    public void GetRandomMovePosition(BossMovement bossRequesting)
    {
        float randomX = Random.Range(transform.position.x - (bossMoveArea.x / 2), transform.position.x + (bossMoveArea.x / 2));
        float randomY = Random.Range(transform.position.y - (bossMoveArea.y / 2), transform.position.y + (bossMoveArea.y / 2));

        Vector2 randomPosition = new Vector2(randomX, randomY);
        //Debug.Log(randomPosition);

        bossRequesting.SetMovePosition = randomPosition;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, bossMoveArea);
    }


}
