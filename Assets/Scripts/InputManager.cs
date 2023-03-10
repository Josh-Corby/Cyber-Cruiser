using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    public static event Action<Vector2> OnMouseMove = null;
    public static event Action<bool> OnFire = null;
    public static event Action OnShield = null;
    public static event Action OnControlsEnabled = null;
    public static event Action OnControlsDisabled = null;
    public static event Action OnPause = null;

    PlayerControls controls;


    private void OnEnable()
    {
        if (controls == null)
        {

            controls = new PlayerControls();

            controls.Controls.MouseVectorInput.performed += i => OnMouseMove(i.ReadValue<Vector2>());
            controls.Controls.Shoot.performed += i => OnFire(true);
            controls.Controls.Shoot.canceled += i => OnFire(false);
            controls.Controls.Shield.performed += i => OnShield?.Invoke();
            controls.Controls.Pause.performed += i => OnPause?.Invoke();

            GameManager.OnLevelCountDownStart += DisableControls;
            GameplayUIManager.OnCountdownDone += EnableControls;
            PlayerManager.OnPlayerDeath += DisableControls;
        }
    }

    private void OnDisable()
    {
        GameManager.OnLevelCountDownStart -= DisableControls;
        GameplayUIManager.OnCountdownDone -= EnableControls;
        PlayerManager.OnPlayerDeath -= DisableControls;
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
