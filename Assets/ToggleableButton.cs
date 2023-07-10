using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.UI;

namespace CyberCruiser
{
    public class ToggleableButton : MonoBehaviour
    {
        [SerializeField] private BoolReference _toggleReference;
        [SerializeField] private Sprite _activeSprite;
        [SerializeField] private Sprite _inactiveSprite;

        private Image _buttonImage;

        private void Awake()
        {
            _buttonImage = GetComponent<Image>();
        }

        private void OnEnable()
        {
            _buttonImage.sprite = _toggleReference.Value ? _activeSprite : _inactiveSprite;
        }

        public void OnClick()
        {
            _buttonImage.sprite = _buttonImage.sprite == _activeSprite ? _inactiveSprite : _activeSprite;
        }
    }
}
