using System;
using TMPro;
using UnityEngine;

public class WaveCountdownManager : GameBehaviour
{
    [SerializeField] private TMP_Text _waveCountdownText;

    private const float WAVECOUNTDOWNTIME = 3f;
    private const float STARTTEXTTIMER = 1f;

    #region Fields
    private float _waveCountdownTime = 3f;
    private float _startTextTimer = 1f;
    private bool _isCountdownDone, _isCountingDown;
    #endregion

    public static event Action OnCountdownDone = null;

    private string WaveCountdownText
    {
        set => _waveCountdownText.text = value;
    }

    private void OnEnable()
    {
        GameManager.OnMissionStart += StartWaveCountdown;
    }

    private void OnDisable()
    {
        GameManager.OnMissionStart -= StartWaveCountdown;
    }

    private void Start()
    {
        _isCountingDown = false;
    }
    private void StartWaveCountdown()
    {
        _isCountdownDone = false;
        _waveCountdownText.enabled = true;
        _waveCountdownTime = WAVECOUNTDOWNTIME;
        _startTextTimer = STARTTEXTTIMER;
        _isCountingDown = true;
    }

    private void Update()
    {
        if (!_isCountingDown) return;
        if (GM.IsPaused) return;

        while (_waveCountdownTime >= 0)
        {
            WaveCountdownText = _waveCountdownTime.ToString("F2");
            _waveCountdownTime -= Time.deltaTime;
            return;
        }

        if (!_isCountdownDone)
        {
            OnCountdownDone?.Invoke();
            WaveCountdownText = "GO!";
            _isCountdownDone = true;
        }
        while (_startTextTimer >= 0)
        {
            _startTextTimer -= Time.deltaTime;
            return;
        }

        if (_waveCountdownText.enabled == true)
        {
            _waveCountdownText.enabled = false;
        }
    }
}