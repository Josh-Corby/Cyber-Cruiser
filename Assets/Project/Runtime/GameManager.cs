using System;
using TMPro;
using UnityEngine;

namespace CyberCruiser
{
    public class GameManager : GameBehaviour<GameManager>
    {
        [SerializeField] private DistanceManager _distanceManager;
        [SerializeField] private EnemySpawnerManager _enemySpawnerManager;
        [SerializeField] private BoolValue _isGamePaused;
        [SerializeField] private GameObject _playerObject;
        [SerializeField] private AnimatedPanelController _animPanelController;

        #region Actions
        public static event Action<bool> OnIsTimeScalePaused = null;
        public static event Action OnMissionStart = null;
        public static event Action OnMissionEnd = null;
        #endregion

        protected override void Awake()
        {
            base.Awake();
            _playerObject.SetActive(false);
            Application.targetFrameRate = 60;
        }

        private void OnEnable()
        {
            InputManager.OnPause += TogglePause;
            PlayerManager.OnPlayerDeath += StopSystems;
            WaveCountdownManager.OnCountdownDone += StartSystems;
        }

        private void OnDisable()
        {
            InputManager.OnPause -= TogglePause;
            PlayerManager.OnPlayerDeath -= StopSystems;
            WaveCountdownManager.OnCountdownDone -= StartSystems;
        }

        public void StartLevel()
        {
            ResumeGame();
            TogglePlayerObject(true);
            ResetSystems();
            OnMissionStart?.Invoke();
        }

        public void EndMission()
        {
            OnMissionEnd?.Invoke();
            StopSystems();
            TogglePlayerObject(false);
        }

        public void StartSystems()
        {
            _distanceManager.StartLevelCounting();
            _enemySpawnerManager.StartSpawningEnemies();
        }

        public void StopSystems()
        {
            _distanceManager.StopIncreasingDistance();
            _enemySpawnerManager.StopSpawningEnemies();
        }

        public void ResetSystems()
        {
            _distanceManager.ResetValues();
            _enemySpawnerManager.ResetSpawning();
        }

        private void TogglePlayerObject(bool value)
        {
            _playerObject.SetActive(value);
        }

        public void TogglePause()
        {
            if (!_isGamePaused.Value)
            {             
                PauseGame();              
            }

            else if (_isGamePaused.Value)
            {
                _animPanelController.CloseCurrentScreen();
            }
        }

        private void PauseGame()
        {
            Time.timeScale = 0f;
            OnIsTimeScalePaused?.Invoke(true);
            _isGamePaused.Value = true;
            _animPanelController.ChangeToPauseScreen();
        }

        public void ResumeGame()
        {
            _isGamePaused.Value = false;
            Time.timeScale = 1f;
            OnIsTimeScalePaused?.Invoke(false);
        }
    }

        public enum GameState
        {
            Mission, Menu
        }
}