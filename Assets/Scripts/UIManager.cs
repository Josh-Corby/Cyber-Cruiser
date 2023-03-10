using UnityEngine;
using System;

public class UIManager : MonoBehaviour
{
    public static event Action OnLevelEntry = null;

    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameplayPanel;

    private void Start()
    {
        MainMenu();
    }

    public void MainMenu()
    {
        mainMenuPanel.SetActive(true);
        gameplayPanel.SetActive(false);
    }

    public void StartGame()
    {
        mainMenuPanel.SetActive(false);
        gameplayPanel.SetActive(true);
        OnLevelEntry?.Invoke();
    }
}
