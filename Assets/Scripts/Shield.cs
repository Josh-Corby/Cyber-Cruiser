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
        Vector2 closestPoint = Vector2.zero;
        float closestDistance = Mathf.Infinity;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            float distance = Vector2.Distance(transform.position, contact.point);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPoint = contact.point;
            }
        }
        _shieldController.ProcessCollision(collision.gameObject, closestPoint);
    }


    public void EnableShields()
    {
        _shieldCollider.enabled = true;
        _shieldSprite.enabled = true;
    }

    public void DisableShields()
    {
        _shieldCollider.enabled = false;
        _shieldSprite.enabled = false;
    }
}
