using CyberCruiser.Audio;
using System;
using TMPro;
using UnityEngine;

namespace CyberCruiser
{
    public class WaveCountdownManager : GameBehaviour
    {
        [SerializeField] private TMP_Text _waveCountdownText;

        private const float WAVECOUNTDOWNTIME = 3f;
        private const float STARTTEXTTIMER = 1f;

        [SerializeField] private SoundControllerBase _soundController;
        [SerializeField] private ClipInfo _countdownClip;

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
            GameManager.OnMissionEnd += StopWaveCountdown;
        }

        private void OnDisable()
        {
            GameManager.OnMissionStart -= StartWaveCountdown;
            GameManager.OnMissionEnd -= StopWaveCountdown;
        }

        private void Start()
        {
            _isCountingDown = false;
        }

        private void Update()
        {
            if (!_isCountingDown) return;

            while (_waveCountdownTime >= 0)
            {
                WaveCountdownText = _waveCountdownTime.ToString("F2");
                _waveCountdownTime -= Time.deltaTime;
                return;
            }

            if (!_isCountdownDone)
            {
                OnCountdownDone?.Invoke();
                WaveCountdownText = "Commence Mission";
                _waveCountdownText.color = Color.green;
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
                WaveCountdownText = "";
            }
        }

        private void StartWaveCountdown()
        {
            _isCountdownDone = false;
            _soundController.PlayNewClip(_countdownClip);
            _waveCountdownText.color = Color.red;
            _waveCountdownText.enabled = true;
            _waveCountdownTime = WAVECOUNTDOWNTIME;
            _startTextTimer = STARTTEXTTIMER;
            _isCountingDown = true;
        }

        private void StopWaveCountdown()
        {
            _isCountingDown = false;
        }
    }
}