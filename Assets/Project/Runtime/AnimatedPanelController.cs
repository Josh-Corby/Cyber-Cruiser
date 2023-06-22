using CyberCruiser.Audio;
using System;
using UnityEngine;

namespace CyberCruiser
{
    [RequireComponent(typeof(Animator))]
    public class AnimatedPanelController : GameBehaviour
    {
        [SerializeField] private AnimatedPanelSoundController _soundController;
        private Animator _animator;

        private GameObject _currentScreen;
        private GameObject _screenToOpen;
        [SerializeField] private GameObject _missionScreen;
        [SerializeField] private GameObject _missionCompleteScreen;
        [SerializeField] private GameObject _gameOverScreen;

        private GameObject _panelToEnable;

        private GameObject _currentPanel;
        [SerializeField] private GameObject _titlePanel;
        [SerializeField] private GameObject _gameplayPanel;
        [SerializeField] private GameObject _menuPanel;
        [SerializeField] private GameObject _pauseScreen;

        private bool _isResumingGame;

        private string _currentState;
        private const string PANEL_UP = "Panel_Up";
        private const string PANEL_DOWN = "Panel_Down";

        public static event Action OnAnimationStart = null;
        public static event Action OnAnimationEnd = null;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            GameManager.OnGamePaused += () => ChangeScreen(_pauseScreen);
            GameManager.OnGameResumed += CloseCurrentScreen;

            PlayerDeathTrigger.OnPlayerDeadOffScreen += CheckGameOverPanelToOpen;
        }

        private void OnDisable()
        {
            GameManager.OnGamePaused -= () => ChangeScreen(_pauseScreen);
            GameManager.OnGameResumed -= CloseCurrentScreen;

            PlayerDeathTrigger.OnPlayerDeadOffScreen -= CheckGameOverPanelToOpen;
        }

        private void Start()
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
            if(_currentScreen != null)
            {
                CloseCurrentScreen();
                return;
            }

            OpenScreen();
        }

        public void GoToMenuPanel()
        {
            _panelToEnable = _menuPanel;
        }

        public void IsResumingGame(bool isResumingGame)
        {
            _isResumingGame = isResumingGame;
        }

        private void CheckGameOverPanelToOpen()
        {
            Cursor.visible = true;
            if (MissionManagerInstance.IsAnyMissionCompleted)
            {
                ChangeScreen(_missionCompleteScreen);
            }

            else
            {
                ChangeScreen(_gameOverScreen);
            }
        }

        #region Animation Events

        private void OnOpenScreenAnimationStart()
        {
            OnAnimationStart?.Invoke();
        }

        private void OnOpenScreenAnimationEnd()
        {
            if (_screenToOpen == _pauseScreen)
            {
                OnAnimationEnd?.Invoke();
            }

            if(_screenToOpen == _missionScreen)
            {
                MissionManagerInstance.ChooseNextMission();
            }

            //Debug.Log("Screen Open");
            _screenToOpen.SetActive(true);
            _currentScreen = _screenToOpen;
            _screenToOpen = null;
        }

        private void OnCloseScreenAnimationStart()
        {
            OnAnimationStart?.Invoke();

            _currentScreen.SetActive(false);          
        }

        private void OnCloseScreenAnimationEnd()
        {

            if (_screenToOpen != null)
            {
                OpenScreen();
            }

            if(_panelToEnable != null)
            {
                _currentPanel.SetActive(false);
                _panelToEnable.SetActive(true);
                _currentPanel = _panelToEnable;
                _panelToEnable = null;

                if(_currentPanel == _gameplayPanel)
                {
                    GameManagerInstance.StartLevel();
                }
            }

            if(_currentScreen == _pauseScreen)
            {
                if (_isResumingGame)
                {
                    OnAnimationEnd?.Invoke();
                    GameManagerInstance.ResumeGame();
                }
            }

            _currentScreen = null;
            _panelToEnable = null;
            _isResumingGame = true;
        }
        #endregion
    }
}
