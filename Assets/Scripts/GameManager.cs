using UnityEngine;
using System.Collections;
using System;
using TMPro;

public class GameManager : GameBehaviour<GameManager>
{
    public bool isPaused = false;
    [SerializeField] private GameObject gameplayObjects;

    public static event Action<bool> OnIsGamePaused = null;
    public static event Action OnMissionStart = null;
    public static event Action OnMissionEnd = null;
  
    private void Awake()
    {
        gameplayObjects.SetActive(false);
        Application.targetFrameRate = 60;
    }
    private void OnEnable()
    {
        InputManager.OnPause += TogglePause;
        UIManager.OnMissionStart += StartLevel;
        UIManager.OnGameplayPanelToggled += ToggleGameplayObjects;
    }

    private void OnDisable()
    {
        InputManager.OnPause -= TogglePause;
        UIManager.OnMissionStart -= StartLevel;
        UIManager.OnGameplayPanelToggled -= ToggleGameplayObjects;
    }

    public void StartLevel()
    {
        isPaused = false;
        Time.timeScale = 1f;
        OnMissionStart?.Invoke();
    }

    private void ToggleGameplayObjects(bool value)
    {
        gameplayObjects.SetActive(value);
    }

    public void EndMission()
    {
        OnMissionEnd?.Invoke();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            PauseGame();
        }
        else if (!isPaused)
        {
            ResumeGame();
        }
        OnIsGamePaused(isPaused);
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}
