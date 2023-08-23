using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberCruiser
{
    public class IntelButton : MonoBehaviour
    {
        private Animator _animator;
        private const string LIGHT_UP = "IntelLightUp";
        private const string LIGHT_DOWN = "IntelLightDown";

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void OnPointerEnter()
        {
            _animator.Play(LIGHT_UP);
        }

        public void OnPointerExit()
        {
            _animator.Play(LIGHT_DOWN);
        }
    }
}
