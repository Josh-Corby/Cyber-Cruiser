using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    public static event Action<Vector2> OnMouseMove = null;
    public static event Action OnControlsEnabled = null;
    public static event Action OnControlsDisabled = null;

    PlayerControls controls;


    private void OnEnable()
    {
        if (controls == null)
        {

            controls = new PlayerControls();

            controls.MouseControls.MouseVectorInput.performed += i => OnMouseMove(i.ReadValue<Vector2>());

            LevelUIManager.OnCountdownDone += EnableControls;
        }
    }

    private void OnDisable()
    {
        LevelUIManager.OnCountdownDone -= EnableControls;
        DisableControls();
    }

    public void EnableControls()
    {
        controls.Enable();
        OnControlsEnabled?.Invoke();
    }

    public void DisableControls()
    {
        controls.Disable();
        OnControlsDisabled?.Invoke();
    }
}
