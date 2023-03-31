using UnityEngine;
using System;

public class Shield : MonoBehaviour
{
    [SerializeField] private ShieldControllerBase _shieldController;
    [SerializeField] private Collider2D _shieldCollider;
    public SpriteRenderer _spriteRenderer;

    public Color SpriteRendererColour
    {
        get
        {
            return _spriteRenderer.color;
        }
        set
        {
            _spriteRenderer.color = value;
        }
    }
    private void Awake()
    {
        _shieldController = GetComponentInParent<ShieldControllerBase>();
        _shieldCollider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        _shieldController.ProcessCollision(collision.gameObject);
    }

    public void EnableShields()
    {
        _shieldCollider.enabled = true;
        _spriteRenderer.enabled = true;
    }

    public void DisableShields()
    {
        _shieldCollider.enabled = false;
        _spriteRenderer.enabled = false;
    }
}
