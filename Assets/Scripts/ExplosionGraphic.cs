using UnityEngine;

public class ExplosionGraphic : MonoBehaviour
{
    private float _explosionRadius;
    
    public float ExplosionRadius { get => _explosionRadius; set => _explosionRadius = value; }

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
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
}
