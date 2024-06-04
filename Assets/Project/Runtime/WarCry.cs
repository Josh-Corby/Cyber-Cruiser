using CyberCruiser.Audio;
using System.Collections;
using TMPro;
using UnityEngine;

namespace CyberCruiser
{
    [RequireComponent(typeof(SoundControllerBase))]
    public class WarCry : MonoBehaviour
    {
        [SerializeField] private CircleCollider2D _collider;
        [SerializeField] private ClipInfo _warCryClip;
        [SerializeField] private SoundControllerBase _soundController;
        [SerializeField] private ParticleSystem _warCryParticles;

        private void Awake()
        {
            _collider = GetComponent<CircleCollider2D>();
            _collider.enabled = false;
        }

        public void StartWarCry()
        {
            _collider.enabled = true;
            _soundController.PlayNewClip(_warCryClip);
            StartCoroutine(ExpandWarCry());
            _warCryParticles.Play();
        }

        private IEnumerator ExpandWarCry()
        {
            while(transform.localScale.x < 300)
            {
                transform.localScale *= 1.1f;
                yield return new WaitForSeconds(0.01f);
            }

            transform.localScale = Vector3.one;
            _collider.enabled = false;
            StopCoroutine(ExpandWarCry());
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("Collision");

            if(collision.TryGetComponent<PlayerManager>(out var player))
            {
                player.Overload();
            }

            else if (collision.GetComponent<Shield>() != null)
            {
                var playerManager = collision.GetComponentInParent<PlayerManager>();
                playerManager.Overload();
            }
        }
    }
}
