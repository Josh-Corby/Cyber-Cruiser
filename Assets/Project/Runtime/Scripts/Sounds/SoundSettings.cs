using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace CyberCruiser
{
    public class SoundSettings : GameBehaviour
    {
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private AudioMixerGroup _musicMixer;
        [SerializeField] private AudioMixerGroup _effectsMixer;

        //const strings that represent parameters exposed in master mixer
        private const string MASTER_VOLUME = "masterVolume";
        private const string MUSIC_VOLUME = "musicVolume";
        private const string EFFECTS_VOLUME = "effectsVolume";

        //const strings that represent the mute state of audio groups
        private const string IS_AUDIO_MUTED = "";
        private const string IS_MUSIC_MUTED = "";
        private const string IS_EFFECTS_MUTED = "";

        private bool _isAudioMuted;
        private bool _isMusicMuted;
        private bool _isEffects;

        private void Start()
        {
            RestoreAudioSettings();
        }

        private void RestoreAudioSettings()
        {
            SetGameVolume(PlayerPrefs.GetFloat(MASTER_VOLUME, 1));
            SetMusicVolume(PlayerPrefs.GetFloat(MUSIC_VOLUME, 1));
            SetEffectsVolume(PlayerPrefs.GetFloat(EFFECTS_VOLUME, 1));

            _isAudioMuted = PlayerPrefs.GetInt(IS_AUDIO_MUTED, 1) != 0;
            _isMusicMuted = PlayerPrefs.GetInt(IS_MUSIC_MUTED, 1) != 0;
            _isEffects = PlayerPrefs.GetInt(IS_EFFECTS_MUTED, 1) != 0;
        }



        private void SaveAudioSettings()
        {
            PlayerPrefs.SetFloat(MASTER_VOLUME, GetGameVolume());
            PlayerPrefs.SetFloat(MUSIC_VOLUME, GetMusicVolume());
            PlayerPrefs.SetFloat(EFFECTS_VOLUME, GetEffectsVolume());
        }

        private float GetGameVolume()
        {
            if (_audioMixer.GetFloat(MASTER_VOLUME, out float value))
            {
                return value;
            }

            return 0;
        }

        private float GetMusicVolume()
        {
            if (_audioMixer.GetFloat(MASTER_VOLUME, out float value))
            {
                return value;
            }

            return 0;
        }

        private float GetEffectsVolume()
        {
            if (_audioMixer.GetFloat(MASTER_VOLUME, out float value))
            {
                return value;
            }

            return 0;
        }


        public void SetGameVolume(float volume)
        {
            _audioMixer.SetFloat(MASTER_VOLUME, volume);
        }

        public void SetMusicVolume(float volume)
        {
            _audioMixer.SetFloat(MUSIC_VOLUME, volume);
        }

        public void SetEffectsVolume(float volume)
        {
            _audioMixer.SetFloat(EFFECTS_VOLUME, volume);
        }

        public void ToggleMusic()
        {

        }

        public void ToggleEffects()
        {

        }

        private void OnApplicationQuit()
        {
            SaveAudioSettings();
        }
    }
}
