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

        bool _initialInputPrevented;

        private void OnEnable()
        {
            if (controls == null)
            {
                controls = new PlayerControls();

                controls.Controls.MouseVectorInput.performed += i => OnMove?.Invoke(i.ReadValue<Vector2>());           
                controls.Controls.Shoot.performed += i => OnFire?.Invoke(true);
                controls.Controls.Shoot.canceled += i => OnFire?.Invoke(false);
                controls.Controls.Shield.performed += i => OnShield?.Invoke();
                controls.Controls.Pause.performed += i => OnPause?.Invoke();

                controls.Controls.TouchPosition.performed += OnPrimaryTouch;
                controls.Controls.Touch1.performed += OnTouch1Performed;
            }

            _initialInputPrevented = true;
            GameManager.OnMissionStart += EnableControls;
        }

        private void OnDisable()
        {
            GameManager.OnMissionStart -= EnableControls;

            DisableControls();          
        }


        public void EnableControls()
        {
            Debug.Log("Controls enabled");
            controls?.Enable();
        }

        public void DisableControls()
        {
            controls?.Disable();
        }

        private void OnPrimaryTouch(InputAction.CallbackContext context)
        {
            Vector2 touchPosition = context.ReadValue<Vector2>();

            if (ClickedOnUI(touchPosition))
            {
                return;
            }

            if (_isGamePaused.Value)
            {
                _initialInputPrevented = true;
                return;
            }

            if (_initialInputPrevented)
            {
                _initialInputPrevented = false;
                return;
            }

            //Debug.Log(TouchPosition);
            OnMove?.Invoke(touchPosition);
        }

        private bool ClickedOnUI(Vector2 position)
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = position;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            // return results.Count > 0;
            foreach (var item in results)
            {
                if (item.gameObject.CompareTag("UI"))
                {
                    return true;
                }
            }
            return false;
        }

        private void OnTouch1Performed(InputAction.CallbackContext context)
        {
            TouchState touch = context.ReadValue<TouchState>();
            TouchPhase state = touch.phase;

            if(state == TouchPhase.Began)
            {
                OnFire?.Invoke(true);
            }

            if(state == TouchPhase.Ended)
            {
                OnFire?.Invoke(false);
            }

        }
    }
}