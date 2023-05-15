using UnityEngine;

namespace CyberCruiser
{
    namespace Audio
    {
        [RequireComponent(typeof(AudioSource))]
        public class SoundControllerBase : GameBehaviour
        {
            protected AudioSource _audioSource;

            private void Awake()
            {
                _audioSource = GetComponent<AudioSource>();
            }

            public void PlayClip()
            {
                _audioSource.Stop();
                _audioSource.Play();
            }       

            public void PlayOneShot(AudioClip clip)
            {
                _audioSource.PlayOneShot(clip);
            }
        }
    }
}