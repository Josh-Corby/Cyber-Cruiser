using CyberCruiser.Audio;
using System;
using UnityEngine;

namespace CyberCruiser
{
    [RequireComponent(typeof(Animator))]
    public class AnimatedPanelController : GameBehaviour
    {
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private MissionManager _missionManager;
        [SerializeField] private AnimatedPanelSoundController _soundController;
        [SerializeField] private InputManager _inputManager;

        private Animator _animator;

        private GameObject _currentScreen;
        private GameObject _screenToOpen;
        [SerializeField] private GameObject _missionScreen;
        [SerializeField] private GameObject _pauseScreen;
        [SerializeField] private GameObject _missionCompleteScreen;
        [SerializeField] private GameObject _gameOverScreen;

        private GameObject _panelToEnable;
        private GameObject _currentPanel;
        [SerializeField] private GameObject _titlePanel;
        [SerializeField] private GameObject _gameplayPanel;
        [SerializeField] private GameObject _menuPanel;

        private bool _isResumingGame;

        private string _currentState;
        private const string PANEL_UP = "Panel_Up";
        private const string PANEL_DOWN = "Panel_Down";

        public static event Action OnGameplayPanelClosed = null;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            PlayerDeathTrigger.OnPlayerDeadOffScreen += CheckGameOverPanelToOpen;
        }

        private void OnDisable()
        {
            PlayerDeathTrigger.OnPlayerDeadOffScreen -= CheckGameOverPanelToOpen;
        }

        private void Start()
        {
            OnGameStart();
        }

        private void OnGameStart()
        {
            _gameplayPanel.SetActive(false);
            _currentPanel = _titlePanel;
            _isResumingGame = true;
        }

        private void ChangeAnimationState(string newState)
        {
            if (newState == _currentState)
            {
                return;
            }

            _animator.Play(newState);
            _currentState = newState;
        }

        private void OpenScreen()
        {
            ChangeAnimationState(PANEL_UP);
            _soundController.PlaySound(0);
        }

        public void CloseCurrentScreen()
        {
            _soundController.PlaySound(1);
            ChangeAnimationState(PANEL_DOWN);
        }

        public void GoToPanel(GameObject panelToEnable)
        {
            _panelToEnable = panelToEnable;
            CloseCurrentScreen();
        }

        public void SetNextPanel(GameObject panelToEnable)
        {
            _panelToEnable = panelToEnable;
        }

        public void EnablePanel(GameObject panelToEnable)
        {
            panelToEnable.SetActive(true);
        }


        public void DisablePanel(GameObject panelToDisable)
        {
            panelToDisable.SetActive(false);
        }

        public void ChangeScreen(GameObject screenToOpen)
        {
            _screenToOpen = screenToOpen;
            if (_currentScreen != null)
            {
                CloseCurrentScreen();
                return;
            }

            OpenScreen();
        }

        public void SwitchPanels(GameObject panelToEnable)
        {
            _currentPanel.SetActive(false);
            _currentPanel = panelToEnable;
            _currentPanel.SetActive(true);  
        }

        public void ChangeToPauseScreen()
        {
            ChangeScreen(_pauseScreen);
        }

        public void IsGameResumingWhenScreenCloses(bool isResumingGame)
        {
            _isResumingGame = isResumingGame;
        }

        private void CheckGameOverPanelToOpen()
        {
            Cursor.visible = true;
            if (_missionManager.IsAnyMissionCompleted)
            {
                ChangeScreen(_missionCompleteScreen);
            }

            else
            {
                ChangeScreen(_gameOverScreen);
            }
        }

        public void EndMission()
        {
            _isResumingGame = false;
            _panelToEnable = _menuPanel;
            ChangeScreen(_missionScreen);
        }

        #region Animation Events

        public void OnOpenScreenAnimationStart()
        {
            _inputManager.DisableControls();
        }

        public void OnOpenScreenAnimationEnd()
        {
            if (_screenToOpen == _pauseScreen)
            {
                _inputManager.EnableControls();
            }

            _screenToOpen.SetActive(true);
            _currentScreen = _screenToOpen;
            _screenToOpen = null;
        }

        public void OnCloseScreenAnimationStart()
        {
            _inputManager.DisableControls();
            _currentScreen.SetActive(false);
        }

        public void SetIsResumingGame(bool isResumingGame)
        {
            _isResumingGame = isResumingGame;
        }

        public void OnCloseScreenAnimationEnd()
        {

            if (_screenToOpen != null)
            {
                OpenScreen();
            }

            if (_panelToEnable != null)
            {
                if (_currentPanel == _gameplayPanel)
                {
                    OnGameplayPanelClosed?.Invoke();
                }
                _currentPanel.SetActive(false);
                _currentPanel = _panelToEnable;
                _panelToEnable.SetActive(true);
                _panelToEnable = null;

                if (_currentPanel == _gameplayPanel)
                {
                    _gameManager.StartMission();
                }
            }

            if (_currentScreen == _pauseScreen)
            {
                if (_isResumingGame)
                {
                    _inputManager.EnableControls();
                    _gameManager.ResumeGame();
                }
            }

            _currentScreen = null;
            _panelToEnable = null;
            _isResumingGame = true;
        }
        #endregion
    }
}
