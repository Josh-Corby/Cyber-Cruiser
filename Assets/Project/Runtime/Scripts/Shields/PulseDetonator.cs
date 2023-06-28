using UnityEngine;

namespace CyberCruiser
{
    public class PulseDetonator : GameBehaviour
    {
        [SerializeField] private float _pulseSpeed;
        [SerializeField] private float _pulseDamage;
        private Vector2 _minSize = new Vector2(0.1f, 0.1f);
        private bool _isDetonating;
        private Collider2D _col;

        private void Awake()
        {
            _col = GetComponent<Collider2D>();
        }

        private void Start()
        {
            transform.localScale = _minSize;
            _col.enabled = false;
        }

        private void Update()
        {
            if (_isDetonating)
            {
                Detonation();
            }
        }

        public void Detonate()
        {
            _isDetonating = true;
            _col.enabled = true;
        }

        private void Detonation()
        {
            while (transform.localScale.x < 350)
            {
                transform.localScale += Vector3.one * 20;
                return;
            }
            _isDetonating = false;
            _col.enabled = false;
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
}