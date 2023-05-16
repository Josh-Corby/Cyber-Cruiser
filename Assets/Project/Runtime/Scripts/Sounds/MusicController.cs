using System;
using UnityEngine;

namespace CyberCruiser
{
    namespace Audio
    {
        public class MusicController : SoundControllerBase
        {
            [SerializeField] private MusicClip[] _musicClips;

            [Serializable]
            private class MusicClip
            {
                public ClipInfo ClipInfo;
                public MusicTypes AudioType;
            }

            private enum MusicTypes
            {
                MainMenu,
                Mission
            }

            //0 for Menu Music
            //1 for Mussion Music

            [SerializeField]
            [Range(0f, 1f)] private float _pausedVolume;

            private void OnEnable()
            {
                GameManager.OnMissionEnd += StartMenuMusic;
                GameManager.OnMissionStart += StartMissionMusic;
                GameManager.OnIsGamePaused += SetMissionMusicOnTogglePause;
            }

            private void OnDisable()
            {
                GameManager.OnMissionEnd -= StartMenuMusic;
                GameManager.OnMissionStart -= StartMissionMusic;
                GameManager.OnIsGamePaused -= SetMissionMusicOnTogglePause;
            }

            private void Start()
            {
                StartMenuMusic();
            }

            private void StartMenuMusic()
            {
                PlayNewClip(_musicClips[(int)MusicTypes.MainMenu].ClipInfo);
            }

            private void StartMissionMusic()
            {
                PlayNewClip(_musicClips[(int)MusicTypes.Mission].ClipInfo);
            }

            private void SetMissionMusicOnTogglePause(bool isPaused)
            {
                _audioSource.volume = isPaused ? _pausedVolume : _musicClips[(int)MusicTypes.Mission].ClipInfo.Volume;
            }
        }
    }
}