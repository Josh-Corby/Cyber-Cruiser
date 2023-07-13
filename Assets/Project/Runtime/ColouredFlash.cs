using System.Collections;
using UnityEngine;

namespace CyberCruiser
{
    public class ColouredFlash : MonoBehaviour
    {
        [Tooltip("Material to switch to during the flash.")]
        [SerializeField] private Material _flashMaterial;

        [Tooltip("Duration of the flash.")]
        [SerializeField] private float _duration;

        [SerializeField] private Color _flashColour;

        private SpriteRenderer _spriteRenderer;
        private Material _originalMaterial;
        private Coroutine _flashRoutine;

        void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _originalMaterial = _spriteRenderer.material;
            _flashMaterial = new Material(_flashMaterial);
        }

        public void Flash()
        {
            if (_flashRoutine != null)
            {
                StopCoroutine(_flashRoutine);
            }

            _flashRoutine = StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            _spriteRenderer.material = _flashMaterial;
            _flashMaterial.color = _flashColour;

            yield return new WaitForSeconds(_duration);

            _spriteRenderer.material = _originalMaterial;
            _flashRoutine = null;
        }
    }
}
