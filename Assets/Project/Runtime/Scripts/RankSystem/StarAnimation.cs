using System;
using UnityEngine;
using UnityEngine.UI;

namespace CyberCruiser
{
    public class StarAnimation : GameBehaviour
    {
        [SerializeField] private Animator _starMovementAnimator;
        [SerializeField] private Animator _starEffectAnimator;
        [SerializeField] private Image _effectImage;
        private const string STAR_EFFECT = "GoldStarAnimation";
        private const string STAR_MOVEMENT = "StarMovement";

        public static event Action OnStarAtDestination = null;

        private void OnEnable()
        {
            _effectImage.enabled = false;
        }


        public void PlayMoveAnimation()
        {
            _starMovementAnimator.Play(STAR_MOVEMENT);           
        }

        public void PlayEffectAnimation()
        {
            _effectImage.enabled = true;
            _starEffectAnimator.Play(STAR_EFFECT);
            OnStarAtDestination?.Invoke();
        }
    }
}
