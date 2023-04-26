using UnityEngine;

public class ExplosionGraphic : MonoBehaviour
{
    public float explosionRadius;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
