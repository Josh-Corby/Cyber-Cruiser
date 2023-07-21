using System.Collections;
using UnityEngine;

namespace CyberCruiser
{
    public class SimpleFlash : MonoBehaviour
    {
        [Tooltip("Material to switch during the flash.")]
        [SerializeField] private Material _flashMaterial;

        [Tooltip("Duration of the flash.")]
        [SerializeField] private float _duration;

        //The SpriteRenderer that should flash.
        private SpriteRenderer _spriteRenderer;

        //Original material of the sprite
        private Material _originalMaterial;

        private Coroutine _flashRoutine;

        private void Start()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _originalMaterial = _spriteRenderer.material;
        }

        public void Flash()
        {
            if(_flashRoutine != null)
            {
                StopCoroutine(_flashRoutine);
            }

            _flashRoutine = StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            _spriteRenderer.material = _flashMaterial;

            yield return new WaitForSeconds(_duration);

            _spriteRenderer.material = _originalMaterial;

            _flashRoutine = null;
        }
    }
}
