using System;
using UnityEngine;

public enum GameState
{
    Mission, Menu
}

public class GameManager : GameBehaviour<GameManager>
{
    [SerializeField] private bool _isPaused = false;
    [SerializeField] private GameObject gameplayObjects;
    private GameState _gameState;

    #region Properties
    public GameState GameState
    {
        get => _gameState;
    }
    #endregion

    #region Actions
    public static event Action<bool> OnIsGamePaused = null;
    public static event Action OnGamePaused = null;
    public static event Action OnGameResumed = null;
    public static event Action OnMissionStart = null;
    public static event Action OnMissionEnd = null;
    #endregion

    protected override void Awake()
    {
        base.Awake();
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
        TogglePause();
    }

    public void TogglePause()
    {
        _isPaused = !_isPaused;

        if (_isPaused)
        {
            PauseGame();
            OnGamePaused?.Invoke();
        }

        else if (!_isPaused)
        {
            OnGameResumed?.Invoke();
        }
        OnIsGamePaused?.Invoke(_isPaused);
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        _gameState = GameState.Menu;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        _gameState = GameState.Mission;
    }
}
