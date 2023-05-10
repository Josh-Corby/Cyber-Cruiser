using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource _bgm, _uiSFX;
    [SerializeField] private AudioClip _menuMusic, _missionMusic;
    [SerializeField][Range(0, 1)] private float _defaultVolume, _pausedVolume;


    private void OnEnable()
    {
        GameManager.OnMissionStart += () => { SetBGMClip(_missionMusic); };
        GameManager.OnMissionEnd += () => { SetBGMClip(_menuMusic); };
        GameManager.OnIsGamePaused += ChangeBGMVolumeOnGamePauseToggle;
    }

    private void OnDisable()
    {
        GameManager.OnMissionStart -= () => { SetBGMClip(_missionMusic); };
        GameManager.OnMissionEnd -= () => { SetBGMClip(_menuMusic); };
        GameManager.OnIsGamePaused -= ChangeBGMVolumeOnGamePauseToggle;
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

    private void ChangeBGMVolumeOnGamePauseToggle(bool isPaused)
    {
        _bgm.volume = isPaused ? _pausedVolume : _defaultVolume;
    }
}