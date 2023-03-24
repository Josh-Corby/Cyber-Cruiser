using UnityEngine;

public class ExplosionGraphic : MonoBehaviour
{
    [SerializeField] private float fadeTime = 0.3f;

    private SpriteRenderer explosionImage;
    private Color originalColor;
    private float elapsedTime = 0f;
    public float explosionRadius;

    private void Start()
    {
        explosionImage = GetComponent<SpriteRenderer>();
        originalColor = explosionImage.color;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);

        explosionImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

        if(elapsedTime >= fadeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
