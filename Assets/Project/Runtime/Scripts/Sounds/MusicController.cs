using UnityEngine;

namespace CyberCruiser
{
    namespace Audio
    {
        public class MusicController : SoundControllerBase
        {
            [SerializeField] private AudioClip _menuMusic;
            [SerializeField] private AudioClip _missionMusic;

            [SerializeField]
            [Range(0f, 1f)] private float _pausedVolume;

            [SerializeField]
            [Range(0f, 1f)] private float _defaultVolume;

            private void OnEnable()
            {
                GameManager.OnIsGamePaused += ChangeBGMVolumeOnGamePauseToggle;
            }

            private void OnDisable()
            {
                GameManager.OnIsGamePaused -= ChangeBGMVolumeOnGamePauseToggle;
            }

            private void Start()
            {
                _audioSource.clip = _menuMusic;
                PlayClip();
            }

            private void ChangeBGMVolumeOnGamePauseToggle(bool isPaused)
            {
                _audioSource.volume = isPaused ? _pausedVolume : _defaultVolume;
            }

        }
    }
}