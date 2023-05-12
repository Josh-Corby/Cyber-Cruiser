using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource _bgm, _uiSFX;
    [SerializeField] private AudioClip _menuMusic, _missionMusic, _panelUp, _panelDown;
    [SerializeField][Range(0, 1)] private float _defaultVolume, _pausedVolume;


    private void OnEnable()
    {
        GameManager.OnMissionStart += () => { SetBGMClip(_missionMusic); };
        GameManager.OnMissionEnd += () => { SetBGMClip(_menuMusic); };
        GameManager.OnIsGamePaused += ChangeBGMVolumeOnGamePauseToggle;
        PanelAnimation.OnPanelOpenAnimationStart += () => { SetUISFXClip(_panelUp); };
        PanelAnimation.OnPanelCloseAnimationStart += () => { SetUISFXClip(_panelDown); };
    }

    private void OnDisable()
    {
        GameManager.OnMissionStart -= () => { SetBGMClip(_missionMusic); };
        GameManager.OnMissionEnd -= () => { SetBGMClip(_menuMusic); };
        GameManager.OnIsGamePaused -= ChangeBGMVolumeOnGamePauseToggle;
        PanelAnimation.OnPanelOpenAnimationStart -= () => { SetUISFXClip(_panelUp); };
        PanelAnimation.OnPanelCloseAnimationStart -= () => { SetUISFXClip(_panelDown); };
    }

    private void Start()
    {
        SetBGMClip(_menuMusic);
    }

    private void SetBGMClip(AudioClip clip)
    {
        _bgm.Stop();
        _bgm.clip = clip;
        _bgm.Play();
        ChangeBGMVolumeOnGamePauseToggle(false);
    }

    private void SetUISFXClip(AudioClip clip)
    {
        _uiSFX.Stop();
        _uiSFX.clip = clip;
        _uiSFX.Play();
    }

    private void ChangeBGMVolumeOnGamePauseToggle(bool isPaused)
    {
        _bgm.volume = isPaused ? _pausedVolume : _defaultVolume;
    }
}
