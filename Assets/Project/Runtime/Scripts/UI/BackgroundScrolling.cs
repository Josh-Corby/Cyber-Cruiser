using UnityEngine;
using UnityEngine.UI;

namespace CyberCruiser
{
    public class BackgroundScrolling : GameBehaviour
    {
        [SerializeField] private RawImage _image;
        [SerializeField] private Vector2 _speed;

        [SerializeField] private IntReference _scrollSpeedMultiplier;

        private void Update()
        {
            _image.uvRect = new Rect(_image.uvRect.position + new Vector2(_speed.x, _speed.y) * _scrollSpeedMultiplier.Value * Time.deltaTime, _image.uvRect.size);
        }
    }
}