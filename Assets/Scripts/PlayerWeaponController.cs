using UnityEngine;
using System;

public class PlayerWeaponController : MonoBehaviour
{
    [SerializeField] private Weapon playerWeapon;
    [SerializeField] private bool fireInput;
    private bool controlsEnabled;

    private void OnEnable()
    {
        InputManager.OnFire += SetFireInput;
        InputManager.OnControlsEnabled += EnableControls;
        InputManager.OnControlsDisabled += DisableControls;

        GameManager.OnGamePaused += DisableControls;
        GameManager.OnGameResumed += EnableControls;
    }

    private void OnDisable()
    {
        InputManager.OnFire -= SetFireInput;
        InputManager.OnControlsEnabled -= EnableControls;
        InputManager.OnControlsDisabled -= DisableControls;

        GameManager.OnGamePaused -= DisableControls;
        GameManager.OnGameResumed -= EnableControls;

    }

    private void Start()
    {
        fireInput = false;
    }

    private void Update()
    {
        if (controlsEnabled)
        {
            if (fireInput)
            {
                CheckForFireInput();
            }
        }
    }

    private void SetFireInput(bool input)
    {
        fireInput = input;
    }

    private void CheckForFireInput()
    {
        if (!playerWeapon.holdToFire)
        {
            CancelFireInput();
        }
        FireWeapon();
    }

    private void FireWeapon()
    {
        if (playerWeapon.readyToFire)
        {
            playerWeapon.StartFireSequence();
        }
    }


    private void CancelFireInput()
    {
        fireInput = false;
    }

    private void EnableControls()
    {
        controlsEnabled = true;
    }

    private void DisableControls()
    {
        controlsEnabled = false;
    }
}
