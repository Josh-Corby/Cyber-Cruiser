using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource _bgm;
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

    private void OnEnable()
    {
        Boss.OnBossDied += (pickup, vector3) => { PlayClipOnSource(_bossCrash, _bossSFX); };
        EnemySpawner.OnBossSpawned += (enemySO) => PlayClipOnSource(_bossSpawn, _bossSFX);
        GameManager.OnIsGamePaused += ChangeBGMVolumeOnGamePauseToggle;
        GameManager.OnMissionStart += () => { PlayClipOnSource(_missionMusic, _bgm); };
        GameManager.OnMissionEnd += () => { PlayClipOnSource(_menuMusic, _bgm); };
        PanelAnimation.OnPanelOpenAnimationStart += () => { PlayClipOnSource(_panelUp, _uiSFX); };
        PanelAnimation.OnPanelCloseAnimationStart += () => { PlayClipOnSource(_panelDown, _uiSFX); };
        PlayerWeaponController.OnWeaponUpgradeStart += (slider, f) => { PlayClipOnSource(_weaponPackStart, _pickupSFX); };
        PlayerWeaponController.OnWeaponUpgradeFinished += (slider) => { PlayClipOnSource(_weaponPackEnd, _pickupSFX); };
    }

    private void OnDisable()
    {
        Boss.OnBossDied -= (pickup, vector3) => { PlayClipOnSource(_bossCrash, _gameSFX); };
        EnemySpawner.OnBossSpawned -= (enemySO) => PlayClipOnSource(_bossSpawn, _gameSFX);
        GameManager.OnIsGamePaused -= ChangeBGMVolumeOnGamePauseToggle;
        GameManager.OnMissionStart -= () => { PlayClipOnSource(_missionMusic, _bgm); };
        GameManager.OnMissionEnd -= () => { PlayClipOnSource(_menuMusic, _bgm); };
        PanelAnimation.OnPanelOpenAnimationStart -= () => { PlayClipOnSource(_panelUp, _uiSFX); };
        PanelAnimation.OnPanelCloseAnimationStart -= () => { PlayClipOnSource(_panelDown, _uiSFX); };
        PlayerWeaponController.OnWeaponUpgradeStart -= (slider, f) => { PlayClipOnSource(_weaponPackStart, _pickupSFX); };
        PlayerWeaponController.OnWeaponUpgradeFinished -= (slider) => { PlayClipOnSource(_weaponPackEnd, _pickupSFX); };
    }

    private void Start()
    {
        PlayClipOnSource(_menuMusic, _bgm);
    }

    private void PlayClipOnSource(AudioClip clip, AudioSource audioSource)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();

        if (audioSource == _bgm)
        {
            ChangeBGMVolumeOnGamePauseToggle(false);
        }
    }

    private void ChangeBGMVolumeOnGamePauseToggle(bool isPaused)
    {
        _bgm.volume = isPaused ? _pausedVolume : _defaultVolume;
    }

    public void OnButtonClick()
    {
        PlayClipOnSource(_buttonClick, _uiSFX);
    }
}
