using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource _bgm;
    [SerializeField] private AudioSource _uiSFX;
    [SerializeField] private AudioSource _gameSFX;
    [SerializeField] private AudioClip _menuMusic, _missionMusic, _panelUp, _panelDown;
    [SerializeField][Range(0, 1)] private float _defaultVolume, _pausedVolume;


    private void OnEnable()
    {
        GameManager.OnIsGamePaused += ChangeBGMVolumeOnGamePauseToggle;
        GameManager.OnMissionStart += () => { PlayClipOnSource(_missionMusic, _bgm); };
        GameManager.OnMissionEnd += () => { PlayClipOnSource(_menuMusic, _bgm); };     
        PanelAnimation.OnPanelOpenAnimationStart += () => { PlayClipOnSource(_panelUp, _uiSFX); };
        PanelAnimation.OnPanelCloseAnimationStart += () => { PlayClipOnSource(_panelDown, _uiSFX); };
    }

    private void OnDisable()
    {
        GameManager.OnIsGamePaused -= ChangeBGMVolumeOnGamePauseToggle;
        GameManager.OnMissionStart -= () => { PlayClipOnSource(_missionMusic, _bgm); };
        GameManager.OnMissionEnd -= () => { PlayClipOnSource(_menuMusic, _bgm); };   
        PanelAnimation.OnPanelOpenAnimationStart -= () => { PlayClipOnSource(_panelUp, _uiSFX); };
        PanelAnimation.OnPanelCloseAnimationStart -= () => { PlayClipOnSource(_panelDown, _uiSFX); };
    }

    private void Start()
    {
        PlayClipOnSource(_menuMusic,_bgm);
    }

    private void PlayClipOnSource(AudioClip clip, AudioSource audioSource)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();

        if(audioSource == _bgm)
        {
            ChangeBGMVolumeOnGamePauseToggle(false);
        }
    }

    private void ChangeBGMVolumeOnGamePauseToggle(bool isPaused)
    {
        _bgm.volume = isPaused ? _pausedVolume : _defaultVolume;
    }
}
