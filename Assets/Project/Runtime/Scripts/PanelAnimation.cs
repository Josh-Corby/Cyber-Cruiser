using DG.Tweening;
using System;
using Unity.Collections;
using UnityEngine;

public class PanelAnimation : MonoBehaviour
{

    private const int DOWN_POS = -900;
    private const int UP_POS = 0;
    private const float _tweenLength = 0.75f;
    [SerializeField] private GameObject _screenDisplay;

    public static event Action OnPanelOpenAnimationStart = null;
    public static event Action OnPanelCloseAnimationStart = null;
    public static event Action OnPanelDisabled = null;

    private void OnEnable()
    {
        StartOpenUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            StartCloseUI();
        }
    }

    private void StartOpenUI()
    {
        OnPanelOpenAnimationStart?.Invoke();
        transform.localPosition = new Vector2(0, DOWN_POS);
        _screenDisplay.SetActive(false);
        TweenUp();
    }

    private void TweenUp()
    {
        transform.DOLocalMoveY(UP_POS, _tweenLength).SetEase(Ease.OutQuint).OnComplete(EnableScreenDisplay);
    }

    private void EnableScreenDisplay()
    {
        _screenDisplay.SetActive(true);
    }

    public void StartCloseUI()
    {
        OnPanelCloseAnimationStart?.Invoke();
        DisableScreenDisplay();
        TweenDown();
    }

    private void DisableScreenDisplay()
    {
        _screenDisplay.SetActive(false);
    }

    public void TweenDown()
    {
        transform.DOLocalMoveY(DOWN_POS, _tweenLength).SetEase(Ease.OutQuint).OnComplete(OnCloseAnimationFinished);
    }

    private void OnCloseAnimationFinished()
    {
        Debug.Log("panel closed");
        OnPanelDisabled?.Invoke();
        gameObject.SetActive(false);
    }
}
