using System;
using UnityEngine;

namespace CyberCruiser
{
    public class StarAnimation : GameBehaviour
    {
        [SerializeField] private Animator _starMovementAnimator;
        [SerializeField] private Animator _starEffectAnimator;
        private const string STAR_EFFECT = "GoldStarAnimation";
        private const string STAR_MOVEMENT = "StarMovement";

        public static event Action OnStarAtDestination = null;

        public void PlayMoveAnimation()
        {
            _starMovementAnimator.Play(STAR_MOVEMENT);           
        }

        public void PlayEffectAnimation()
        {
            _starEffectAnimator.Play(STAR_EFFECT);
            OnStarAtDestination?.Invoke();
        }
    }
}
