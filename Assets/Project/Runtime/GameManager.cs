using System;
using UnityEngine;

namespace CyberCruiser
{
    public class GameManager : GameBehaviour<GameManager>
    {
        [SerializeField] private bool _isPaused = false;
        [SerializeField] private GameObject gameplayObjects;
        private GameState _gameState;

        public GameState CurrentGameState
        {
            get => _gameState;
        }

        #region Actions
        public static event Action<bool> OnIsGamePaused = null;
        public static event Action OnGamePaused = null;
        public static event Action OnGameResumed = null;
        public static event Action OnMissionStart = null;
        public static event Action OnMissionEnd = null;
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
        }

        private void OnDisable()
        {
            InputManager.OnPause -= TogglePause;
        }

        public void StartLevel()
        {
            _gameState = GameState.Mission;
            _isPaused = false;
            Time.timeScale = 1f;
            ToggleGameplayObjects(true);
            OnMissionStart?.Invoke();
        }

        private void ToggleGameplayObjects(bool value)
        {
            gameplayObjects.SetActive(value);
        }

        public void EndMission()
        {
            ToggleGameplayObjects(false);
            //TogglePause();
            OnMissionEnd?.Invoke();
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
            OnIsGamePaused?.Invoke(true);
        }

        public void StopGame()
        {
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            Time.timeScale = 1f;
            _gameState = GameState.Mission;
            OnIsGamePaused?.Invoke(false);
        }
    }

        public enum GameState
        {
            Mission, Menu
        }
}