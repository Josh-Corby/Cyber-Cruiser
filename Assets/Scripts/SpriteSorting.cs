using UnityEngine;

public class SpriteSorting : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Transform _baseObject;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _baseObject = transform.parent;
    }

    void Update()
    {
        int sortingOrder = (int)(_baseObject.position.y * -100);
        _spriteRenderer.sortingOrder = sortingOrder;
    }
}
