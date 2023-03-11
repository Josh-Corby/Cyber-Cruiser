using UnityEngine;

public class ExplosionGraphic : MonoBehaviour
{
    [SerializeField] private float fadeTime = 1f;

    private SpriteRenderer explosionImage;
    private Color originalColor;
    private float elapsedTime = 0f;

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
    }
}
