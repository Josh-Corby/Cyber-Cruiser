using System;
using UnityEngine;

namespace CyberCruiser
{
    public class InputManager : GameBehaviour<InputManager>
    {
        PlayerControls controls;

        public bool IsCursorVisible
        {
            set => Cursor.visible = value;
        }

        #region Actions
        public static event Action<Vector2> OnMouseMove = null;
        public static event Action<bool> OnFire = null;
        public static event Action OnShield = null;
        public static event Action OnPause = null;
        #endregion

        private void OnEnable()
        {
            if (controls == null)
            {
                controls = new PlayerControls();

                controls.Controls.MouseVectorInput.performed += i => OnMouseMove?.Invoke(i.ReadValue<Vector2>());
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

    }
}