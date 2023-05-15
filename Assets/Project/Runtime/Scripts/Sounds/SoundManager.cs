using UnityEngine;

public class SoundManager : GameBehaviour<SoundManager>
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource _music;
    [SerializeField] private AudioSource _uiSFX;
    [SerializeField] private AudioSource _gameSFX;
    [SerializeField] private AudioSource _bossSFX;
    [SerializeField] private AudioSource _pickupSFX;

    #region Audio Clips
    [Header("UI Clips")]
    [SerializeField] private AudioClip _menuMusic;
    [SerializeField] private AudioClip _missionMusic;
    [SerializeField] private AudioClip _panelUp;
    [SerializeField] private AudioClip _panelDown;
    [SerializeField] private AudioClip _buttonClick;
    [Header("Boss Clips")]
    [SerializeField] private AudioClip _bossSpawn;
    [SerializeField] private AudioClip _bossCrash;

    [Header("Pickup Clips")]
    [SerializeField] private AudioClip _weaponPackStart;
    [SerializeField] private AudioClip _weaponPackEnd;

    #endregion

    [SerializeField][Range(0, 1)] private float _defaultVolume, _pausedVolume;

    private void Start()
    {
        PlayClipOnSource(_menuMusic, _music);
    }


    private void PlayClipOnSource(AudioClip clip, AudioSource audioSource)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();

        if (audioSource == _music)
        {
            ChangeBGMVolumeOnGamePauseToggle(false);
        }
    }

    private void ChangeBGMVolumeOnGamePauseToggle(bool isPaused)
    {
        _music.volume = isPaused ? _pausedVolume : _defaultVolume;
    }

    public void OnButtonClick()
    {
        PlayClipOnSource(_buttonClick, _uiSFX);
    }
}
