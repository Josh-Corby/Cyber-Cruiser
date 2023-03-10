using UnityEngine;
using System;
using System.Collections;
using TMPro;
public class GameplayUIManager : MonoBehaviour
{
    public static event Action OnCountdownDone = null;

    private Coroutine waveCountdown;

    [SerializeField] private TMP_Text waveCountdownText;
    [SerializeField] private TMP_Text plasmaCountText;
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pausePanel;

    private void OnEnable()
    {
        GameManager.OnLevelCountDownStart += StartMission;
        GameManager.OnGamePaused += EnablePauseUI;
        GameManager.OnGameResumed += DisablePauseUI;

        PlayerManager.OnPlayerDeath += GameOverUI;
        PlayerManager.OnPlasmaChange += UpdatePlasmaText;
    }

    private void OnDisable()
    {
        GameManager.OnLevelCountDownStart -= StartMission;
        GameManager.OnGamePaused -= EnablePauseUI;
        GameManager.OnGameResumed -= DisablePauseUI;

        PlayerManager.OnPlayerDeath -= GameOverUI;
        PlayerManager.OnPlasmaChange -= UpdatePlasmaText;
    }

    private void StartMission()
    {
        ResetWaveCountdown();
        gameplayPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
    }

    private void ResetWaveCountdown()
    {
        if (waveCountdown != null)
        {
            StopCoroutine(waveCountdown);
        }
        StartWaveCountdown();
    }
    private void StartWaveCountdown()
    {
        waveCountdown = StartCoroutine(WaveCountdown());
    }

    private IEnumerator WaveCountdown()
    {
        waveCountdownText.enabled = true;
        float waveCountdown = 3f;
        float startActiveTimer = 1f;

        while (waveCountdown >= 0)
        {
            waveCountdownText.text = waveCountdown.ToString("F2");
            waveCountdown -= Time.deltaTime;
            yield return null;
        }

        OnCountdownDone?.Invoke();
        waveCountdownText.text = "GO!";

        while(startActiveTimer >= 0)
        {
            startActiveTimer -= Time.deltaTime;
            yield return null;
        }
        waveCountdownText.enabled = false;
    }

    private void UpdatePlasmaText(int plasmaCount)
    {
        plasmaCountText.text = plasmaCount.ToString();
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
