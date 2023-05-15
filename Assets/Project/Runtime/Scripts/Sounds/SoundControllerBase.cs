using UnityEngine;

namespace CyberCruiser
{
    namespace Audio
    {
        [RequireComponent(typeof(AudioSource))]
        public abstract class SoundControllerBase : GameBehaviour
        {
            protected AudioSource _audioSource;

            public AudioSource AudioSource { get => _audioSource; }

            private void Awake()
            {
                _audioSource = GetComponent<AudioSource>();
            }

            public virtual void PlayClip()
            {
                _audioSource.Stop();
                _audioSource.Play();
            }       
        }
    }
}