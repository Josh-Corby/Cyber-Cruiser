using UnityEngine;

namespace CyberCruiser
{
    public class Shield : MonoBehaviour
    {
        public ShieldControllerBase _shieldController;
        [SerializeField] private Collider2D _shieldCollider;
        private SpriteRenderer _spriteRenderer;
        private Color _tempColour;
        private float _startAlpha;

        private void Awake()
        {
            _shieldController = GetComponentInParent<ShieldControllerBase>();
            _shieldCollider = GetComponent<Collider2D>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Start()
        {
            _startAlpha = _spriteRenderer.color.a;
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
            _spriteRenderer.enabled = value;
        }

        public void SetTargetAlpha(float currentStrength, float maxStrength)
        {
            float currentPercentStrength = currentStrength / maxStrength;
            float targetAlpha = _startAlpha * currentPercentStrength;
            ChangeSpriteRendererAlpha(targetAlpha);
        }

        public void ChangeSpriteRendererAlpha(float targetAlpha)
        {
            _tempColour = _spriteRenderer.color;
            _tempColour.a = targetAlpha;
            _spriteRenderer.color = _tempColour;
        }
    }
}