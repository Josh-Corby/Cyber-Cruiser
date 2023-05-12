using System;
using UnityEngine;

public class GameManager : GameBehaviour<GameManager>
{
    [SerializeField] private bool _isPaused = false;
    [SerializeField] private GameObject gameplayObjects;

    #region Properties
    public bool IsPaused { get => _isPaused; private set => _isPaused = value; }
    #endregion

    #region Actions
    public static event Action<bool> OnIsGamePaused = null;
    public static event Action OnMissionStart = null;
    public static event Action OnMissionEnd = null;
    #endregion
    private void Awake()
    {
        gameplayObjects.SetActive(false);
        Application.targetFrameRate = 60;
    }

    private void OnEnable()
    {
        InputManager.OnPause += TogglePause;
        UIManager.OnLevelUIReady += StartLevel;
    }

    private void OnDisable()
    {
        InputManager.OnPause -= TogglePause;
        UIManager.OnLevelUIReady -= StartLevel;
    }

    public void StartLevel(bool value)
    {
        _isPaused = false;
        Time.timeScale = 1f;
        ToggleGameplayObjects(value);
        OnMissionStart?.Invoke();
    }

    private void ToggleGameplayObjects(bool value)
    {
        gameplayObjects.SetActive(value);
    }

    public void EndMission()
    {
        ToggleGameplayObjects(false);
        OnMissionEnd?.Invoke();
    }

    public void TogglePause()
    {
        _isPaused = !_isPaused;

        if (_isPaused)
        {
            PauseGame();
        }
        else if (!_isPaused)
        {
            ResumeGame();
        }
        OnIsGamePaused(_isPaused);
    }

    private void PauseGame()
    {
        //Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}
