using UnityEngine;
using System;
public class GameManager : MonoBehaviour
{
    public static event Action OnLevelCountDownStart = null;
    public static event Action OnGamePaused = null;
    public static event Action OnGameResumed = null;

    [SerializeField] private GameObject gameplayObjects;
    private bool isPaused = false;

    private void OnEnable()
    {
        InputManager.OnPause += TogglePause;
    }

    private void OnDisable()
    {
        InputManager.OnPause -= TogglePause;
    }

    private void Start()
    {
        StartLevel();
    }

    public void StartLevel()
    {
        isPaused = false;
        Time.timeScale = 1f;
        OnLevelCountDownStart?.Invoke();
        gameplayObjects.SetActive(true);
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
