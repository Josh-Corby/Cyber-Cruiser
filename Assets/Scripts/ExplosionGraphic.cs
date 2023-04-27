using UnityEngine;

public class ExplosionGraphic : MonoBehaviour
{
    public float explosionRadius;

    private void Start()
    {
        Invoke(nameof(Destroy), 0.2f);
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
