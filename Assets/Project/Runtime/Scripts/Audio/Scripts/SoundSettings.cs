using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace CyberCruiser
{
    public class SoundSettings : GameBehaviour
    {
        [SerializeField] private AudioMixer _audioMixer;

        private const int DEFAULT_MIXER_VOLUME_IN_DB = 0;
        private const int VALUE_TO_MUTE_MIXER_IN_DB = -80;

        //const strings that represent parameters exposed in master mixer
        private const string MASTER_VOLUME = "masterVolume";
        private const string MUSIC_VOLUME = "musicVolume";
        private const string EFFECTS_VOLUME = "effectsVolume";

        //const strings that represent the mute state of audio groups
        private const string IS_AUDIO_MUTED = "isAudioMuted";
        private const string IS_MUSIC_MUTED = "isMusicMuted";
        private const string IS_EFFECTS_MUTED = "isEffectsMuted";

        [Header("Audio Levels")]
        [SerializeField] private FloatValue _masterVolume;
        [SerializeField] private FloatValue _musicVolume;
        [SerializeField] private FloatValue _effectsVolume;

        [Header("Mute States")]
        [SerializeField] private BoolValue _isAudioMuted;
        [SerializeField] private BoolValue _isMusicMuted;
        [SerializeField] private BoolValue _isEffectsMuted;

        //properties used to get and set SO values easier
        private float MasterVolume { get => _masterVolume.Value; set => _masterVolume.Value = value; }
        private float MusicVolume { get => _musicVolume.Value; set => _musicVolume.Value = value; }
        private float EffectsVolume { get => _effectsVolume.Value; set => _effectsVolume.Value = value; }
        private bool IsAudioMuted { get => _isAudioMuted.Value; set => _isAudioMuted.Value = value; }
        private bool IsMusicMuted { get => _isMusicMuted.Value; set => _isMusicMuted.Value = value; }
        private bool IsEffectsMuted { get => _isEffectsMuted.Value; set => _isEffectsMuted.Value = value; }

        private void Start()
        {
            RestoreAudioSettings();
        }

        private void RestoreAudioSettings()
        {
            IsAudioMuted = PlayerPrefs.GetInt(IS_AUDIO_MUTED, 0) != 0;
            IsMusicMuted = PlayerPrefs.GetInt(IS_MUSIC_MUTED, 0) != 0;
            IsEffectsMuted = PlayerPrefs.GetInt(IS_EFFECTS_MUTED, 0) != 0;

            MasterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME, DEFAULT_MIXER_VOLUME_IN_DB);
            MusicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME, DEFAULT_MIXER_VOLUME_IN_DB);
            EffectsVolume = PlayerPrefs.GetFloat(EFFECTS_VOLUME, DEFAULT_MIXER_VOLUME_IN_DB);

            CheckAudioBools();
        }

        private void CheckAudioBools()
        {
            if(IsAudioMuted)
            {
                _audioMixer.SetFloat(MASTER_VOLUME, VALUE_TO_MUTE_MIXER_IN_DB);
            }

            else
            {
                SetMasterVolume(MasterVolume);
            }

            if(IsMusicMuted) 
            {
                _audioMixer.SetFloat(MUSIC_VOLUME, VALUE_TO_MUTE_MIXER_IN_DB);
            }

            else
            {
                SetMusicVolume(MusicVolume);
            }

            if (IsEffectsMuted)
            {
                _audioMixer.SetFloat(EFFECTS_VOLUME, VALUE_TO_MUTE_MIXER_IN_DB);
            }

            else
            {
                SetEffectsVolume(EffectsVolume);
            }
        }

        private void SaveAudioSettings()
        {
            PlayerPrefs.SetFloat(MASTER_VOLUME, MasterVolume);
            PlayerPrefs.SetFloat(MUSIC_VOLUME, MusicVolume);
            PlayerPrefs.SetFloat(EFFECTS_VOLUME, EffectsVolume);

            PlayerPrefs.SetInt(IS_AUDIO_MUTED, IsAudioMuted ? 1 : 0);
            PlayerPrefs.SetInt(IS_MUSIC_MUTED, IsMusicMuted ? 1 : 0);
            PlayerPrefs.SetInt(IS_EFFECTS_MUTED, IsEffectsMuted ? 1 : 0);
        }

        #region Set Audio Levels
        private void SetMixerVolume(string mixerName, bool isMixerMuted, float mixerVolume, float newMixerVolume)
        {
            mixerVolume = newMixerVolume;
            if(!isMixerMuted)
            {
                _audioMixer.SetFloat(mixerName, mixerVolume);
            }
        }

        private void SetMasterVolume(float newMasterVolume)
        {
            SetMixerVolume(MASTER_VOLUME, IsAudioMuted, MasterVolume, newMasterVolume);
        }

        private void SetMusicVolume(float newMusicVolume)
        {
            SetMixerVolume(MUSIC_VOLUME, IsMusicMuted, MusicVolume, newMusicVolume);
        }

        private void SetEffectsVolume(float newEffectsVolume)
        {
            SetMixerVolume(EFFECTS_VOLUME, IsEffectsMuted, EffectsVolume, newEffectsVolume);
        }

        public void SetMasterVolumeFromSlider(float sliderValue)
        {
            float sliderValueToDB = Mathf.Log10(sliderValue) * 20;
            SetMasterVolume(sliderValueToDB);
        }

        public void SetMusicVolumeFromSlider(float sliderValue)
        {
            float sliderValueToDB = Mathf.Log10(sliderValue) * 20;
            SetMusicVolume(sliderValueToDB);
        }

        public void SetEffectsVolumeFromSlider(float sliderValue)
        {
            float sliderValueToDB = Mathf.Log10(sliderValue) * 20;
            SetEffectsVolume(sliderValueToDB);
        }
        #endregion

        #region Toggle Audio
        private void MuteMixer(string mixerName)
        {
            _audioMixer.SetFloat(mixerName, VALUE_TO_MUTE_MIXER_IN_DB);
        }

        private void ToggleMixer(string mixerName, float mixerVolume, ref bool isMixerMuted, Action<float> function)
        {
            if(!isMixerMuted)
            {
                MuteMixer(mixerName);
            }

            isMixerMuted = !isMixerMuted;
            if (!isMixerMuted)
            {
                function(mixerVolume);
            }
        }

        public void ToggleAudio()
        {
            bool currentMuteState = IsAudioMuted;
            void SetMasterVolumeWrapper(float value) { SetMasterVolume(MasterVolume); }
            ToggleMixer(MASTER_VOLUME, MasterVolume, ref _isAudioMuted.Value, SetMasterVolumeWrapper);
            IsAudioMuted = !currentMuteState;
        }

        public void ToggleMusic()
        {
            bool currentMuteState = IsMusicMuted;
            void SetMusicVolumeWrapper(float value) { SetMusicVolume(MusicVolume); }
            ToggleMixer(MUSIC_VOLUME, MusicVolume, ref _isMusicMuted.Value, SetMusicVolumeWrapper);
            IsMusicMuted = !currentMuteState;
        }

        public void ToggleEffects()
        {
            bool currentMuteState = IsEffectsMuted;
            void SetEffectsVolumeWrapper(float value) { SetEffectsVolume(EffectsVolume); }
            ToggleMixer(EFFECTS_VOLUME, EffectsVolume, ref _isEffectsMuted.Value, SetEffectsVolumeWrapper);
            IsEffectsMuted = !currentMuteState;
        }
        #endregion

        private void OnApplicationQuit()
        {
            SaveAudioSettings();
        }
    }
}
