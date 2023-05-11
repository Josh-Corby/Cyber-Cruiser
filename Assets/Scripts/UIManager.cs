using System;
using UnityEngine;

public class UIManager : GameBehaviour
{
    #region References  
    [SerializeField]
    private GameObject _titlePanel, _missionsPanel, _loadoutPanel, _gameplayPanel,
          _pausePanel, _gameOverPanel, _missionCompletePanel, _optionsPanel, _creditsPanel, _storePanel;

    #endregion

    #region Fields
    private GameObject[] _panels;
    [SerializeField] private GameObject _currentPanel;
    [SerializeField] private GameObject _panelToEnable;
    #endregion

    #region Actions
    public static event Action<bool> OnLevelUIReady = null;
    public static event Action OnMissionsPanelLoaded;
    #endregion

    private void OnEnable()
    {
        GameManager.OnIsGamePaused += TogglePauseUI;
        PlayerManager.OnPlayerDeath += SelectEndOfMissionScreen;
        PanelAnimation.OnPanelDisabled += EnablePanel;
    }

    private void OnDisable()
    {
        GameManager.OnIsGamePaused -= TogglePauseUI;
        PlayerManager.OnPlayerDeath -= SelectEndOfMissionScreen;
        PanelAnimation.OnPanelDisabled -= EnablePanel;
    }

    private void Start()
    {
        _currentPanel = _titlePanel;
        InitializePanelsArray();
        EnableMainMenu();
    }

    private void InitializePanelsArray()
    {
        _panels = new GameObject[] { _titlePanel, _missionsPanel, _loadoutPanel, _gameplayPanel,
            _pausePanel, _gameOverPanel, _missionCompletePanel, _optionsPanel, _creditsPanel, _storePanel };
    }

    public void DisablePanels()
    {
        if (_gameplayPanel.activeSelf == true)
        {
            _gameplayPanel.SetActive(false);
        }

        foreach (GameObject panel in _panels)
        {
            if (panel.activeSelf)
            {
                panel.SetActive(false);
            }
        }
    }

    public void EnableMainMenu()
    {
        DisablePanels();
        _panelToEnable = _titlePanel;
        EnablePanel();
    }

    public void EnableLevelUI()
    {
        if (_currentPanel.TryGetComponent<PanelAnimation>(out var panelAnimation))
        {
            panelAnimation.StartCloseUI();
        }
        else
        {
            DisablePanel();
        }

        _currentPanel.SetActive(false);
        _gameplayPanel.SetActive(true);
        OnLevelUIReady?.Invoke(_gameplayPanel.activeSelf);
    }

    public void GoToScreen(GameObject screen)
    {
        //set panel to change to
        //disable current panel


        //Debug.Log(screen.name);
        _panelToEnable = screen;
        Debug.Log(_panelToEnable.name);

        if(_panelToEnable == _missionsPanel)
        {
            OnMissionsPanelLoaded?.Invoke();
        }
        DisablePanel();
    }

    private void DisablePanel()
    {
        //if panel has animation play it
        if (_currentPanel.TryGetComponent<PanelAnimation>(out var panelAnimation))
        {
            panelAnimation.StartCloseUI();
        }
        else
        {
            //if panel has no animation disable current panel and enable next one
            _currentPanel.SetActive(false);
            EnablePanel();
        }
    }

    private void EnablePanel()
    {
        _panelToEnable.SetActive(true);
        _currentPanel = _panelToEnable;
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

    private void TogglePauseUI(bool value)
    {
        if (value)
        {
            EnablePauseUI();
        }
        else
        {
            DisablePauseUI();
        }
    }

    private void EnablePauseUI()
    {
        IM.IsCursorVisible = true;
        _pausePanel.SetActive(true);
    }

    private void DisablePauseUI()
    {
        IM.IsCursorVisible = false;
        _pausePanel.SetActive(false);
    }
}
