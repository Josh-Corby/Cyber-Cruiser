using System;
using TMPro;
using UnityEngine;

namespace CyberCruiser
{
    public class GameManager : GameBehaviour<GameManager>
    {
        [SerializeField] private DistanceManager _distanceManager;
        [SerializeField] private EnemySpawnerManager _enemySpawnerManager;

        [SerializeField] private bool _isPaused = false;
        [SerializeField] private GameObject gameplayObjects;
        private GameState _gameState;

        public GameState CurrentGameState
        {
            get => _gameState;
        }

        #region Actions
        public static event Action<bool> OnIsTimeScalePaused = null;
        public static event Action OnGamePaused = null;
        public static event Action OnGameResumed = null;
        public static event Action OnMissionStart = null;
        public static event Action OnMissionEnd = null;
        public static event Action OnSaveDataCleared = null;
        #endregion

        protected override void Awake()
        {
            base.Awake();
            gameplayObjects.SetActive(false);
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
            _gameState = GameState.Mission;
            ResumeGame();
            ToggleGameplayObjects(true);
            OnMissionStart?.Invoke();
            ResetSystems();
        }

        public void EndMission()
        {
            StopSystems();
            ToggleGameplayObjects(false);
            //TogglePause();
            OnMissionEnd?.Invoke();
        }

        public void ResetSystems()
        {
            _distanceManager.ResetValues();
            _enemySpawnerManager.ResetSpawning();
        }

        public void StartSystems()
        {
            _distanceManager.StartIncreasingDistance();
            _enemySpawnerManager.StartSpawningEnemies();
        }

        public void StopSystems()
        {
            _distanceManager.StopIncreasingDistance();
            _enemySpawnerManager.StopSpawningEnemies();
        }

        private void ToggleGameplayObjects(bool value)
        {
            gameplayObjects.SetActive(value);
        }

        public void TogglePause()
        {
            _isPaused = !_isPaused;

            if (_isPaused)
            {
                PauseGame();
                OnGamePaused?.Invoke();
            }

            else if (!_isPaused)
            {
                OnGameResumed?.Invoke();
            }
        }

        private void PauseGame()
        {
            StopGame();
            _gameState = GameState.Menu;
            OnIsTimeScalePaused?.Invoke(true);
        }

        public void StopGame()
        {
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            _isPaused = false;
            Time.timeScale = 1f;
            _gameState = GameState.Mission;
            OnIsTimeScalePaused?.Invoke(false);
        }

        public void ClearSaveData()
        {
            OnSaveDataCleared?.Invoke();
            /*current stars
             * current rank
             * current mission
             * ion
             * plasma
             * quests cleared
            */
        }
    }

        public enum GameState
        {
            Mission, Menu
        }
}