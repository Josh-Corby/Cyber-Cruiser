using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseDetonator : GameBehaviour
{
    [SerializeField] private float _pulseSpeed;
    [SerializeField] private float _pulseDamage;
    private Vector2 _minSize = new Vector2(0.1f, 0.1f);
    private bool _isDetonating;

    private void Start()
    {
        transform.localScale = _minSize;
    }
    private void Update()
    {
        if (!GM.isPaused)
        {
            if (_isDetonating)
            {
                Detonation();
            }
        }      
    }
    public void Detonate()
    {
        _isDetonating = true;
    }

    private void Detonation()
    {
        while (transform.localScale.x < 350)
        {
            transform.localScale += Vector3.one * 20;
            return;
        }
        _isDetonating = false;
        transform.localScale = _minSize;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Shield>(out var shield))
        {
            if (collision.gameObject.GetComponentInParent<Enemy>())
            {
                shield._shieldController.ReduceShields(_pulseDamage);
            }
        }

        else if (collision.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.Damage(_pulseDamage);
        }
    }
}
