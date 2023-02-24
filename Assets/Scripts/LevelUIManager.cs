using UnityEngine;
using System;
using System.Collections;
using TMPro;
public class LevelUIManager : MonoBehaviour
{
    public static event Action OnCountdownDone = null;

    [SerializeField] private TMP_Text waveCountdownText;

    private void OnEnable()
    {
        GameManager.OnLevelCountDownStart += StartWaveCountdown;
    }

    private void OnDisable()
    {
        GameManager.OnLevelCountDownStart -= StartWaveCountdown;
    }

    private void StartWaveCountdown()
    {
        StartCoroutine(WaveCountdown());
    }
    private IEnumerator WaveCountdown()
    {
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
}
