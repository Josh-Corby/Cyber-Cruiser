using UnityEngine;
using System;

public class Shield : MonoBehaviour
{
    [SerializeField] private ShieldControllerBase _shieldController;
    [SerializeField] private Collider2D _shieldCollider;
    [SerializeField] private SpriteRenderer _shieldRenderer;
    [SerializeField] protected int _shieldCollisionDamage;

    private void Awake()
    {
        _shieldController = GetComponentInParent<ShieldControllerBase>();
        _shieldCollider = GetComponent<Collider2D>();
        _shieldRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        _shieldController.ProcessCollision(collision.gameObject,_shieldCollisionDamage);
    }

    public void EnableShields()
    {
        _shieldCollider.enabled = true;
        _shieldRenderer.enabled = true;
    }

    public void DisableShields()
    {
        _shieldCollider.enabled = false;
        _shieldRenderer.enabled = false;
    }
}
