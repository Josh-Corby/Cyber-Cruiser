using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    public static event Action<Vector2> OnMouseMove = null;

    PlayerControls controls;

    private void OnEnable()
    {
        if (controls == null)
        {

            controls = new PlayerControls();

            controls.MouseControls.MouseVectorInput.performed += i => OnMouseMove(i.ReadValue<Vector2>());

            EnableControls();
        }
    }

    private void OnDisable()
    {
        
    }

    public void EnableControls()
    {
        controls.Enable();
    }

    public void DisableControls()
    {
        controls.Disable();
    }
}
