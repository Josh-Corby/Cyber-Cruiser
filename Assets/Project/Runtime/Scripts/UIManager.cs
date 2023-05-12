using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class UIManager : GameBehaviour<UIManager>
{
    #region References  
    [SerializeField] private GameObject _titlePanel;
    [SerializeField] private GameObject _missionsPanel;
    [SerializeField] private GameObject _gameplayPanel;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _missionCompletePanel;
    [SerializeField] private GameObject _gameOverPanel;
    #endregion

    #region Fields
    [SerializeField] private GameObject _currentPanel;
    [SerializeField] private GameObject _panelToEnable;
    [SerializeField] private float _enablePanelDelay;
    #endregion

    #region Actions
    public static event Action<bool> OnLevelUIReady = null;
    public static event Action OnMissionsPanelLoaded;
    #endregion

    private void OnEnable()
    {
        GameManager.OnGamePaused += EnablePauseUI;
        GameManager.OnGameResumed += DisablePauseUI;
        PlayerManager.OnPlayerDeath += SelectEndOfMissionScreen;
        PanelAnimation.OnPanelDisabled += ValidateCurrentPanel;
    }

    private void OnDisable()
    {
        GameManager.OnGamePaused -= EnablePauseUI;
        GameManager.OnGameResumed -= DisablePauseUI;
        PlayerManager.OnPlayerDeath -= SelectEndOfMissionScreen;
        PanelAnimation.OnPanelDisabled -= ValidateCurrentPanel;
    }

    private void Start()
    {
        GoToScreen(_titlePanel);
    }

    public void EnableMainMenu()
    {
        if (_gameplayPanel.activeSelf == true)
        {
            _gameplayPanel.SetActive(false);
        }
        GoToScreen(_titlePanel);
    }

    public void GoToScreen(GameObject screen)
    {
        _panelToEnable = screen;

        if(_panelToEnable == null)
        {
            return;
        }

        if(_currentPanel == null)
        {
            ValidatePanelToEnable();
            return;
        }
        DisableCurrentPanel();      
    }

    private void DisableCurrentPanel()
    {
        if (_currentPanel.TryGetComponent<PanelAnimation>(out var panelAnimation))
        {
            panelAnimation.StartCloseUI();
        }
        else
        {
            _currentPanel.SetActive(false);
            ValidatePanelToEnable();
        }       
    }

    private void ValidateCurrentPanel()
    {
        if(_currentPanel == _pausePanel)
        {
            GM.ResumeGame();
        }
        ValidatePanelToEnable();
    }

    private void ValidatePanelToEnable()
    {
        if (_panelToEnable == null)
        {
            _currentPanel = null;
            return;
        }

        EnableNextPanel();
    }

    private void EnableNextPanel()
    {
        if (_panelToEnable == null) return;
        _panelToEnable.SetActive(true);
        _currentPanel = _panelToEnable;


        if (_currentPanel == _missionsPanel)
        {
            OnMissionsPanelLoaded?.Invoke();
        }

        if(_currentPanel == _gameplayPanel)
        {
            OnLevelUIReady?.Invoke(_gameplayPanel.activeSelf);
        }
    }

    private void SelectEndOfMissionScreen()
    {
        _gameplayPanel.SetActive(false);
        IM.IsCursorVisible = true;
        if (MM.IsAnyMissionCompleted)
        {
            _missionCompletePanel.SetActive(true);
            _currentPanel = _missionCompletePanel;
        }
        else
        {
            _gameOverPanel.SetActive(true);
            _currentPanel = _gameOverPanel;
        }
    }

    private void EnablePauseUI()
    {
        _panelToEnable = _pausePanel;
        ValidatePanelToEnable();
        IM.IsCursorVisible = true;
    }

    public void DisablePauseUI()
    {
        DisableCurrentPanel();
        IM.IsCursorVisible = false;
    }
}
