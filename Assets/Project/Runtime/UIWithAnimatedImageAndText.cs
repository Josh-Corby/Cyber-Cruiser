using TMPro;
using UnityEngine;

namespace CyberCruiser
{
    public class UIWithAnimatedImageAndText : MonoBehaviour
    {
        private Animator _animator;
        [SerializeField] private TMP_Text _buttonText;
        [SerializeField] private Color _baseColour;
        [SerializeField] private Color _highlightColour;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
        }

        private const string TO_GLOW = "ToGlow";
        private const string FROM_GLOW = "FromGlow";
        public void OnPointerEnter()
        {
            _buttonText.color = _highlightColour;
            _animator.Play(TO_GLOW);
        }

        public void OnPointerExit()
        {
            _buttonText.color = _baseColour;
            _animator.Play(FROM_GLOW);
        }

        private void OnDisable()
        {
            _buttonText.color = _baseColour;
        }
    }
}
