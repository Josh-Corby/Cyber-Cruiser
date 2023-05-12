using DG.Tweening;
using System;
using Unity.Collections;
using UnityEngine;

public class PanelAnimation : MonoBehaviour
{
    private Animator _animator;
    private string _currentState;
    private const string PANEL_UP = "Panel_Up";
    private const string PANEL_DOWN = "Panel_Down";

    [SerializeField] private GameObject _screenDisplay;
    public static event Action OnPanelOpenAnimationStart = null;
    public static event Action OnPanelCloseAnimationStart = null;
    public static event Action OnPanelDisabled = null;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        StartOpenUI();
    }

    private void ChangeAnimationState(string newState)
    {
        if(newState == _currentState)
        {
            return;
        }

        _animator.Play(newState);
        _currentState = newState;
    }

    public void EnableScreenDisplay()
    {
        _screenDisplay.SetActive(true);
    }

    public void DisableScreenDisplay()
    {
        _screenDisplay.SetActive(false);
    }

    private void StartOpenUI()
    {
        DisableScreenDisplay();
        OnPanelOpenAnimationStart?.Invoke();
        ChangeAnimationState(PANEL_UP);
    }

    public void StartCloseUI()
    {
        DisableScreenDisplay();
        OnPanelCloseAnimationStart?.Invoke();
        ChangeAnimationState(PANEL_DOWN);
    }

    private void OnCloseAnimationFinished()
    {
        Debug.Log("panel closed");
        OnPanelDisabled?.Invoke();
        gameObject.SetActive(false);
    }
}
