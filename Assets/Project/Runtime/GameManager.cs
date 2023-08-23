using System;
using UnityEngine;

namespace CyberCruiser
{
    public class GameManager : GameBehaviour
    {
        [SerializeField] private DistanceManager _distanceManager;
        [SerializeField] private EnemySpawnerManager _enemySpawnerManager;
        [SerializeField] private BoolValue _isGamePaused;
        [SerializeField] private GameObject _playerObject;
        [SerializeField] private AnimatedPanelController _animPanelController;

        private bool _inMission;

        public bool InMission { get => _inMission; private set => _inMission = value; }
        #region Actions
        public static event Action<bool> OnIsTimeScalePaused = null;
        public static event Action OnMissionStart = null;
        public static event Action OnMissionEnd = null;

        [SerializeField]
        #endregion

        private void Awake()
        {
            _playerObject.SetActive(false);
            Application.targetFrameRate = 60;
        }

        private void OnEnable()
        {
            InputManager.OnPause += TogglePause;
            PlayerManager.OnPlayerDeath += StopSystems;
            WaveCountdownManager.OnCountdownDone += StartSystems;
            AnimatedPanelController.OnGameplayPanelClosed += DisablePlayerObject;
        }

        private void OnDisable()
        {
            InputManager.OnPause -= TogglePause;
            PlayerManager.OnPlayerDeath -= StopSystems;
            WaveCountdownManager.OnCountdownDone -= StartSystems;
            AnimatedPanelController.OnGameplayPanelClosed -= DisablePlayerObject;
        }

        public void StartMission()
        {
            ResumeGame();
            EnablePlayerObject();
            ResetSystems();
            OnMissionStart?.Invoke();
            InMission = true;
        }

        public void EndMission()
        {
            OnMissionEnd?.Invoke();
            StopSystems();
            InMission = false;
        }

        public void StartSystems()
        {
            _distanceManager.InitialiseLevelCounting();
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

        private void EnablePlayerObject()
        {
            _playerObject.SetActive(true);
        }

        private void DisablePlayerObject()
        {
            _playerObject.SetActive(false);
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
}