using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class WaveCountdownManager : GameBehaviour
{
    [SerializeField] private TMP_Text _waveCountdownText;
    private Coroutine _waveCountdownCoroutine;

    public static event Action OnCountdownDone = null;

    public string WaveCountDown
    {
        set
        {
            _waveCountdownText.text = value;
        }

        get
        {
            return _waveCountdownText.text;
        }
    }

    private void OnEnable()
    {
        GameManager.OnMissionStart += ResetWaveCountdown;
    }

    private void OnDisable()
    {
        GameManager.OnMissionStart -= ResetWaveCountdown;
    }

    private void ResetWaveCountdown()
    {
        if (_waveCountdownCoroutine != null)
        {
            StopCoroutine(_waveCountdownCoroutine);
        }
        StartWaveCountdown();
    }

    private void StartWaveCountdown()
    {
        _waveCountdownCoroutine = StartCoroutine(WaveCountdown());
    }

    private IEnumerator WaveCountdown()
    {
        _waveCountdownText.enabled = true;
        float waveCountdown = 3f;
        float startActiveTimer = 1f;

        while (waveCountdown >= 0)
        {
            WaveCountDown = waveCountdown.ToString("F2");
            waveCountdown -= Time.deltaTime;
            yield return null;
        }

        OnCountdownDone?.Invoke();
        WaveCountDown = "GO!";

        while (startActiveTimer >= 0)
        {
            startActiveTimer -= Time.deltaTime;
            yield return null;
        }
        _waveCountdownText.enabled = false;
    }
}