using UnityEngine;
using System;
using System.Collections;
using TMPro;
public class GameplayUIManager : MonoBehaviour
{
    public static event Action OnCountdownDone = null;

    [SerializeField] private TMP_Text waveCountdownText;
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pausePanel;

    private void OnEnable()
    {
        GameManager.OnLevelCountDownStart += StartMission;
        GameManager.OnGamePaused += EnablePauseUI;
        GameManager.OnGameResumed += DisablePauseUI;

        PlayerManager.OnPlayerDeath += GameOverUI;
    }

    private void OnDisable()
    {
        GameManager.OnLevelCountDownStart -= StartMission;
        GameManager.OnGamePaused -= EnablePauseUI;
        GameManager.OnGameResumed -= DisablePauseUI;

        PlayerManager.OnPlayerDeath -= GameOverUI;
    }

    private void StartMission()
    {
        StartWaveCountdown();
        gameplayPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
    }

    private void StartWaveCountdown()
    {
        StartCoroutine(WaveCountdown());
    }

    private IEnumerator WaveCountdown()
    {
        gameOverPanel.SetActive(false);
        waveCountdownText.enabled = true;
        float waveCountdown = 3f;

        while(waveCountdown >= 0)
        {
            waveCountdownText.text = waveCountdown.ToString("F2");
            waveCountdown -= Time.deltaTime;
            yield return null;
        }

        OnCountdownDone?.Invoke();
        waveCountdownText.text = "GO!";

        float startActiveTimer = 1f;

        while(startActiveTimer >= 0)
        {
            startActiveTimer -= Time.deltaTime;
            yield return null;
        }
        waveCountdownText.enabled = false;
    }

    private void GameOverUI()
    {
        Cursor.visible = true;
        gameOverPanel.SetActive(true);
    }

    private void EnablePauseUI()
    {
        pausePanel.SetActive(true);
    }

    private void DisablePauseUI()
    {
        pausePanel.SetActive(false);
    }
}
