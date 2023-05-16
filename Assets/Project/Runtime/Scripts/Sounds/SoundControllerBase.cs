using UnityEngine;

namespace CyberCruiser
{
    namespace Audio
    {
        [RequireComponent(typeof(AudioSource))]
        public class SoundControllerBase : GameBehaviour
        {
            [SerializeField] protected AudioSource _audioSource;

            protected void Awake()
            {
                _audioSource = GetComponent<AudioSource>();
            }

            public void PlayNewClip(ClipInfo clipInfo)
            {
                if(_audioSource == null)
                {
                    Debug.LogWarning("Audio source hasn't been assigned yet");
                    return;
                }

                _audioSource.clip = clipInfo.Clip;
                _audioSource.volume = clipInfo.OverrideSourceVolume ? clipInfo.Volume : 1;

                CheckIfClipIsOneShot(clipInfo);
            }

            private void CheckIfClipIsOneShot(ClipInfo clipInfo)
            {
                if (clipInfo.PlayOneShot)
                {
                    PlayOneShot(clipInfo.Clip);
                }

                else
                {
                    PlayClip();
                }
            }

            private void PlayClip()
            {
                _audioSource.Play();
            }

            public void PlayOneShot(AudioClip clip)
            {
                _audioSource.PlayOneShot(clip);
            }
        }
    }
}