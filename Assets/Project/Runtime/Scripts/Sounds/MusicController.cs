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
                GameManager.OnMissionEnd += StartMenuMusic;
                GameManager.OnMissionStart += StartMissionMusic;
                GameManager.OnIsGamePaused += ChangeBGMVolumeOnGamePauseToggle;
            }

            private void OnDisable()
            {
                GameManager.OnMissionEnd -= StartMenuMusic;
                GameManager.OnMissionStart -= StartMissionMusic;
                GameManager.OnIsGamePaused -= ChangeBGMVolumeOnGamePauseToggle;
            }

            private void Start()
            {
                StartMenuMusic();
            }

            private void StartMenuMusic()
            {
                _audioSource.clip = _menuMusic;
                PlayClip();
            }

            private void StartMissionMusic()
            {
                _audioSource.clip = _missionMusic;
                PlayClip();
            }

            private void ChangeBGMVolumeOnGamePauseToggle(bool isPaused)
            {
                _audioSource.volume = isPaused ? _pausedVolume : _defaultVolume;
            }

        }
    }
}