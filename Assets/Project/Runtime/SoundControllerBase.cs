using System.Collections;
using UnityEngine;

namespace CyberCruiser
{
    namespace Audio
    {
        [RequireComponent(typeof(AudioSource))]
        public class SoundControllerBase : GameBehaviour
        {
            [SerializeField] protected AudioSource _audioSource;
            [SerializeField] protected bool _pauseOnGamePaused = true;
            private Coroutine _fadeOutCoroutine = null;

            protected void Awake()
            {
                _audioSource = GetComponent<AudioSource>();
            }

            protected virtual void OnEnable()
            {
                if (_pauseOnGamePaused)
                {
                    GameManager.OnIsTimeScalePaused  += ToggleAudio;
                }
            }

            protected virtual void OnDisable()
            {
                if (_pauseOnGamePaused)
                {
                    GameManager.OnIsTimeScalePaused -= ToggleAudio;
                }
            }

            private IEnumerator VolumeFadeCoroutine()
            {
                while (_audioSource.volume > 0)
                {
                    _audioSource.volume -= 0.1f;
                    yield return new WaitForEndOfFrame();
                }

                _audioSource.Pause();
            }

            protected void ToggleAudio(bool isAudioPausing)
            {
                if (_fadeOutCoroutine != null)
                {
                    StopCoroutine(_fadeOutCoroutine);
                }

                if (isAudioPausing)
                {               
                    _fadeOutCoroutine = StartCoroutine(VolumeFadeCoroutine());
                }

                else
                {
                    _audioSource.volume = 1;
                    _audioSource.UnPause();
                }
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