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

        [SerializeField] private GameObject _currentScreen;
        private GameObject _screenToOpen;
        [SerializeField] private GameObject _missionScreen;
        [SerializeField] private GameObject _pauseScreen;
        [SerializeField] private GameObject _missionCompleteScreen;
        [SerializeField] private GameObject _gameOverScreen;

        [SerializeField] private GameObject _currentGameScreen;

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

        public void ChangeScreenCheckinfIfInMission(GameObject screenToOpen)
        {
            //if in a mission go to the pause screen
            if (_gameManager.InMission)
            {
                SwitchScreens(_currentGameScreen);
            }

            //otherwise go to intended screen
            else
            {
                ChangeScreen(screenToOpen);
            }

        }

        public void SwitchScreens(GameObject screenToOpen)
        {
            _screenToOpen = screenToOpen;
            _currentScreen.SetActive(false);
            _currentScreen = _screenToOpen;
            _currentScreen.SetActive(true);
        }

        public void SwitchPanels(GameObject panelToEnable = null)
        {
            if(panelToEnable != null)
            {
                _panelToEnable = panelToEnable;
            }

            _currentPanel.SetActive(false);
            _currentPanel = _panelToEnable;
            _currentPanel.SetActive(true);
            _panelToEnable = null;
        }

        public void ChangeToPauseScreen()
        {
            _currentGameScreen = _pauseScreen;
            ChangeScreen(_currentGameScreen);
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
                _currentGameScreen = _missionCompleteScreen;         
            }

            else
            {
                _currentGameScreen = _gameOverScreen;
            }
            _gameplayPanel.SetActive(false);
            ChangeScreen(_currentGameScreen);

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
            if (_screenToOpen == _pauseScreen)
            {
                _gameplayPanel.SetActive(false);
            }
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

            //if a screen has been closed in mission resume the game
            if (_gameManager.InMission)
            {
                Debug.Log("Resuming game");
                _inputManager.EnableControls();
                _gameManager.ResumeGame();
                _screenToOpen = null;
                _gameplayPanel.SetActive(true);
            }

            if (_screenToOpen != null)
            {
                OpenScreen();
            }

            //if game panel is changing
            if (_panelToEnable != null)
            {
                //ckear screen after gameplay screen is closed
                if (_currentPanel == _gameplayPanel)
                {
                    OnGameplayPanelClosed?.Invoke();
                }

                //switch panels
                SwitchPanels();

                //if we have gone to the gameplay panel start mission
                if (_currentPanel == _gameplayPanel)
                {
                    _gameManager.StartMission();
                }
            }

            _currentScreen = null;
            _panelToEnable = null;
            _isResumingGame = true;
        }
        #endregion
    }
}
