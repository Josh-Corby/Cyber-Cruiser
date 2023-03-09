using UnityEngine;
using System;

public class PlayerWeaponController : MonoBehaviour
{
    [SerializeField] private Weapon playerWeapon;
    private bool fireInput;
    private bool controlsEnabled;

    private void OnEnable()
    {
        InputManager.OnFire += SetFireInput;
        GameManager.OnGamePaused += DisableControls;
        GameManager.OnGameResumed += EnableControls;
    }

    private void OnDisable()
    {
        InputManager.OnFire -= SetFireInput;

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
        playerWeapon.Fire();
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
