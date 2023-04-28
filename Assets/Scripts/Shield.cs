using UnityEngine;
using System;

public class Shield : MonoBehaviour
{
    public ShieldControllerBase _shieldController;
    [SerializeField] private Collider2D _shieldCollider;
    public SpriteRenderer _shieldSprite;

    public Color SpriteRendererColour
    {
        get
        {
            return _shieldSprite.color;
        }
        set
        {
            _shieldSprite.color = value;
        }
    }
    private void Awake()
    {
        _shieldController = GetComponentInParent<ShieldControllerBase>();
        _shieldCollider = GetComponent<Collider2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 closestCollision = GetClosestCollisionPoint(collision.contacts);
        _shieldController.ProcessCollision(collision.gameObject, closestCollision);
    }

    private Vector2 GetClosestCollisionPoint(ContactPoint2D[] contacts)
    {
        Vector2 closestPoint = Vector2.zero;
        float closestDistance = Mathf.Infinity;

        foreach (ContactPoint2D contact in contacts)
        {
            float distance = Vector2.Distance(transform.position, contact.point);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPoint = contact.point;
            }
        }
        return closestPoint;
    }

    public void ToggleShields(bool value)
    {
        _shieldCollider.enabled = value;
        _shieldSprite.enabled = value;
    }
}
