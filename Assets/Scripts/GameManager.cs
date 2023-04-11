using UnityEngine;
using System.Collections;
using System;
using TMPro;

public class GameManager : GameBehaviour<GameManager>
{
    public bool isPaused = false;
    [SerializeField] private GameObject gameplayObjects;
    public static event Action OnLevelCountDownStart = null;
    public static event Action OnGamePaused = null;
    public static event Action OnGameResumed = null;
  
  
    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
    private void OnEnable()
    {
        InputManager.OnPause += TogglePause;
        UIManager.OnLevelEntry += StartLevel;    
    }

    private void OnDisable()
    {
        InputManager.OnPause -= TogglePause;
        UIManager.OnLevelEntry -= StartLevel;
    }

    public void StartLevel()
    {
        isPaused = false;
        Time.timeScale = 1f;
        OnLevelCountDownStart?.Invoke();
        //gameplayObjects.SetActive(true);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            OnGamePaused?.Invoke();
            PauseGame();
        }
        else if (!isPaused)
        {
            ResumeGame();
            OnGameResumed?.Invoke();
        }
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
