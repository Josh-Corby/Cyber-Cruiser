using UnityEngine;
using System;
using System.Collections;

public class UIManager : GameBehaviour
{
    public static event Action OnMissionStart = null;
    public static event Action<bool> OnGameplayPanelToggled = null;

    [SerializeField] private GameObject _titlePanel, _missionsPanel, _loadoutPanel, _gameplayPanel, _pausePanel, _gameOverPanel, _missionCompletePanel, _optionsPanel, _creditsPanel, _storePanel;
    private GameObject[] _panels;

    public bool GameplayPanel
    {
        set
        {
            _gameplayPanel.SetActive(value);
            OnGameplayPanelToggled(_gameplayPanel.activeSelf); 
        }
        get
        {
            return _gameplayPanel.activeSelf;
        }
    }

    private void OnEnable()
    {
        GameManager.OnIsGamePaused += TogglePauseUI;
        PlayerManager.OnPlayerDeath += GameOverUI;
    }

    private void OnDisable()
    {
        GameManager.OnIsGamePaused -= TogglePauseUI;
        PlayerManager.OnPlayerDeath -= GameOverUI;
    }

    private void Start()
    {
        InitializePanelsArray();
        EnableMainMenu();
    }

    private void InitializePanelsArray()
    {
        _panels = new GameObject[] { _titlePanel, _missionsPanel, _loadoutPanel, _gameplayPanel, _pausePanel, _gameOverPanel, _missionCompletePanel, _optionsPanel, _creditsPanel, _storePanel };
    }

    public void DisablePanels()
    {
        if(GameplayPanel == true)
        {
            GameplayPanel = false;
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
        _titlePanel.SetActive(true);
    }

    public void StartMission()
    {
        DisablePanels();
        GameplayPanel = true;
        OnMissionStart?.Invoke();
    }

    public void EnablePanel(GameObject panelToEnable)
    {
        DisablePanels();
        panelToEnable.SetActive(true);
    }

    private void GameOverUI()
    {
        Cursor.visible = true;
        _gameOverPanel.SetActive(true);
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
        Cursor.visible = true;
        _pausePanel.SetActive(true);
    }

    private void DisablePauseUI()
    {
        Cursor.visible = false;
        _pausePanel.SetActive(false);
    }
}
