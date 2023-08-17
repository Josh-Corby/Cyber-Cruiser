using UnityEngine;

namespace CyberCruiser
{
    public class UIWithAnimatedImage : MonoBehaviour
    {
        private Animator _animator;

        private const string TO_GLOW = "ToGlow";
        private const string FROM_GLOW = "FromGlow";

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void OnPointerEnter()
        {
            _animator.Play(TO_GLOW);
        }

        public void OnPointerExit()
        {
            _animator.Play(FROM_GLOW);
        }
    }
}
