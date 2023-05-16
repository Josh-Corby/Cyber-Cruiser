using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace CyberCruiser
{
    public class SoundSettings : GameBehaviour
    {
        [SerializeField] private AudioMixer _audioMixer;

        [SerializeField] private Slider _masterSlider;
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _effectsSlider;

        private const int DEFAULT_MIXER_VOLUME_IN_DB = 0;
        private const int VALUE_TO_MUTE_MIXER_IN_DB = -80;

        //const strings that represent parameters exposed in master mixer
        private const string MASTER_VOLUME = "masterVolume";
        private const string MUSIC_VOLUME = "musicVolume";
        private const string EFFECTS_VOLUME = "effectsVolume";

        private float _masterVolume;
        private float _musicVolume;
        private float _effectsVolume;

        //const strings that represent the mute state of audio groups
        private const string IS_AUDIO_MUTED = "isAudioMuted";
        private const string IS_MUSIC_MUTED = "isMusicMuted";
        private const string IS_EFFECTS_MUTED = "isEffectsMuted";

        [SerializeField] private bool _isAudioMuted;
        [SerializeField] private bool _isMusicMuted;
        [SerializeField] bool _isEffectsMuted;

        private void Start()
        {
            RestoreAudioSettings();
        }

        private void RestoreAudioSettings()
        {
            _isAudioMuted = PlayerPrefs.GetInt(IS_AUDIO_MUTED, 0) != 0;
            _isMusicMuted = PlayerPrefs.GetInt(IS_MUSIC_MUTED, 0) != 0;
            _isEffectsMuted = PlayerPrefs.GetInt(IS_EFFECTS_MUTED, 0) != 0;

            _masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME, DEFAULT_MIXER_VOLUME_IN_DB);
            _musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME, DEFAULT_MIXER_VOLUME_IN_DB);
            _effectsVolume = PlayerPrefs.GetFloat(EFFECTS_VOLUME, DEFAULT_MIXER_VOLUME_IN_DB);

            SetSlidersToSavedPositions();

            CheckAudioBools();
        }

        private void SetSlidersToSavedPositions()
        {
            _masterSlider.value = _masterVolume;
            _musicSlider.value = _musicVolume;
            _effectsSlider.value = _effectsVolume;
        }

        private void CheckAudioBools()
        {
            if(_isAudioMuted)
            {
                _audioMixer.SetFloat(MASTER_VOLUME, VALUE_TO_MUTE_MIXER_IN_DB);
            }

            else
            {
                SetMasterVolumeIfNotMuted(_masterVolume);
            }

            if(_isMusicMuted) 
            {
                _audioMixer.SetFloat(MUSIC_VOLUME, VALUE_TO_MUTE_MIXER_IN_DB);
            }

            else
            {
                SetMusicVolumeIfNotMuted(_musicVolume);

            }

            if (_isEffectsMuted)
            {
                _audioMixer.SetFloat(EFFECTS_VOLUME, VALUE_TO_MUTE_MIXER_IN_DB);
            }

            else
            {
                SetEffectsVolumeIfNotMuted(_effectsVolume);
            }
        }

        private void SaveAudioSettings()
        {
            PlayerPrefs.SetFloat(MASTER_VOLUME, _masterVolume);
            PlayerPrefs.SetFloat(MUSIC_VOLUME, _musicVolume);
            PlayerPrefs.SetFloat(EFFECTS_VOLUME, _effectsVolume);

            PlayerPrefs.SetInt(IS_AUDIO_MUTED, _isAudioMuted ? 1 : 0);
            PlayerPrefs.SetInt(IS_MUSIC_MUTED, _isMusicMuted ? 1 : 0);
            PlayerPrefs.SetInt(IS_EFFECTS_MUTED, _isEffectsMuted ? 1 : 0);
        }


        #region Set Audio Levels
        public void SetMasterVolumeIfNotMuted(float volume)
        {
            if (!_isAudioMuted)
            {
                _masterVolume = volume;
                _audioMixer.SetFloat(MASTER_VOLUME, _masterVolume);
            }
        }

        public void SetMusicVolumeIfNotMuted(float volume)
        {
            if (!_isMusicMuted)
            {
                _musicVolume = volume;
                _audioMixer.SetFloat(MUSIC_VOLUME, _musicVolume);
            }
        }

        public void SetEffectsVolumeIfNotMuted(float volume)
        {
            if (!_isEffectsMuted)
            {
                _effectsVolume = volume;
                _audioMixer.SetFloat(EFFECTS_VOLUME, _effectsVolume);
            }
        }

        #endregion

        #region Toggle Audio
        public void ToggleAudio()
        {
            _isAudioMuted = !_isAudioMuted;
            if (_isAudioMuted)
            {
                _audioMixer.SetFloat(MASTER_VOLUME, VALUE_TO_MUTE_MIXER_IN_DB);
                return;
            }

            _masterVolume = _masterSlider.value;
            _audioMixer.SetFloat(MASTER_VOLUME, _masterVolume);
        }

        public void ToggleMusic()
        {
            _isMusicMuted = !_isMusicMuted;
            if (_isMusicMuted)
            {
                _audioMixer.SetFloat(MUSIC_VOLUME, VALUE_TO_MUTE_MIXER_IN_DB);
                return;
            }

            _musicVolume = _musicSlider.value;
            _audioMixer.SetFloat(MUSIC_VOLUME, _musicVolume);
        }

        public void ToggleEffects()
        {
            _isEffectsMuted = !_isEffectsMuted;
            if (_isEffectsMuted)
            {
                _audioMixer.SetFloat(EFFECTS_VOLUME, VALUE_TO_MUTE_MIXER_IN_DB);
                return;
            }

            _effectsVolume = _effectsSlider.value;
            _audioMixer.SetFloat(EFFECTS_VOLUME, _effectsVolume);
        }
        #endregion

        private void OnApplicationQuit()
        {
            SaveAudioSettings();
        }
    }
}
