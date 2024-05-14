using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace CyberCruiser
{
    public class InputManager : GameBehaviour<InputManager>
    {
        PlayerControls controls;
        [SerializeField] BoolReference _isGamePaused;

        public bool IsCursorVisible
        {
            set => Cursor.visible = value;
        }

        #region Actions
        public static event Action<Vector2> OnMove = null;
        public static event Action<bool> OnFire = null;
        public static event Action OnShield = null;
        public static event Action OnPause = null;
        #endregion

        private void OnEnable()
        {
            if (controls == null)
            {
                controls = new PlayerControls();

                controls.Controls.MouseVectorInput.performed += i => OnMouseMove(i);           
                controls.Controls.Shoot.performed += i => OnFire?.Invoke(true);
                controls.Controls.Shoot.canceled += i => OnFire?.Invoke(false);
                controls.Controls.Shield.performed += i => OnShield?.Invoke();
                controls.Controls.Pause.performed += i => OnPause?.Invoke();
            }

            GameManager.OnMissionStart += EnableControls;
        }

        private void OnDisable()
        {
            GameManager.OnMissionStart -= EnableControls;

            DisableControls();          
        }

        public void EnableControls()
        {
            controls?.Enable();
        }

        public void DisableControls()
        {
            controls?.Disable();
        }

        private void OnMouseMove(InputAction.CallbackContext context)
        {
            Vector2 movePosition = context.ReadValue<Vector2>();
            Debug.Log(movePosition);
            OnMove?.Invoke(movePosition);
        }
    }
}