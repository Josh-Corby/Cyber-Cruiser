using UnityEngine;

namespace CyberCruiser
{
    public class Boundaries : MonoBehaviour
    {
        private Vector2 _screenBounds;
        [SerializeField] private SpriteRenderer _playerSprite;
        [SerializeField] float width = 1;
        [SerializeField] float height = 1;
        [SerializeField] private BoolReference _isDead;

        void Start()
        {
            _screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
            width = _playerSprite.bounds.size.x /2;
            height = _playerSprite.bounds.size.y / 2;
        }

        void LateUpdate()
        {
            if(!_isDead.Value)
            {
                Vector3 viewPos = transform.position;
                viewPos.x = Mathf.Clamp(viewPos.x, _screenBounds.x * -1 + width,_screenBounds.x - width);
                viewPos.y = Mathf.Clamp(viewPos.y, _screenBounds.y * -1 + height,_screenBounds.y - height);
                transform.position = viewPos;
            }
        }
    }
}
