using UnityEngine;

namespace CyberCruiser
{
    public class PlayerShipController : GameBehaviour
    {
        private const string PLAYER_ALIVE_LAYER = "Player";
        private const string PLAYER_DEAD_LAYER = "DeadPlayer";

        #region References
        [SerializeField] private GameObject _playerSprite;
        [SerializeField] private Transform _spawnPosition;
        [SerializeField] private BoolReference _isPlayerDeadReference;
        [SerializeField] private InputManager _inputManager;
        [SerializeField] private Rigidbody2D _rb;
        #endregion

        #region Fields
        public Vector2 GetInput { get => _input; }
        [SerializeField] private bool _moveWithJoystick = false;
        [SerializeField] private Vector2 _input = Vector2.zero;
        [SerializeField] private float baseSpeed = 0.5f;
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float distanceToStopRotation = 5f;
        [SerializeField] private bool _lerpMovement = true;
        [SerializeField] private float _crashSpeed;
        private bool _controlsEnabled;

        private readonly float minAngle = -20;
        private readonly float maxAngle = 20;
        private Quaternion targetRotation;

        [SerializeField] private VariableJoystick _joystick; 
        #endregion

        #region Properties
        public bool ControlsEnabled { get => _controlsEnabled; set => _controlsEnabled = value; }
        #endregion

        private void OnEnable()
        {
            GoToStartPos();
            CyberKrakenGrappleTentacle.OnGrappleEnd += EnableControls;
            InputManager.OnMove += RecieveInput;
            Cursor.lockState = CursorLockMode.Confined;
            gameObject.layer = LayerMask.NameToLayer(PLAYER_ALIVE_LAYER);
        }

        private void OnDisable()
        {
            InputManager.OnMove -= RecieveInput;
            CyberKrakenGrappleTentacle.OnGrappleEnd -= EnableControls;
        }

        private void Update()
        {
            if (_isPlayerDeadReference.Value == true)
            {
                DeathMovement();
                return;
            }

            if (_controlsEnabled)
            {
                PlayerMovement();
            }
        }

        private void PlayerMovement()
        {
            Vector2 desiredMoveLocation = transform.position;

            if(_moveWithJoystick)
            {
                _input = new Vector2(_joystick.Horizontal, _joystick.Vertical);
                _input *= .75f;
                desiredMoveLocation += _input;

                //_rb.AddForce(_input * baseSpeed * 1.5f);
                if (!_lerpMovement)
                {
                    transform.position = Vector2.MoveTowards(transform.position, desiredMoveLocation, baseSpeed * 1.25f * Time.deltaTime);
                }

                if (_lerpMovement)
                {
                    transform.position = Vector2.Lerp(transform.position, desiredMoveLocation, baseSpeed * 1.25f * Time.deltaTime);
                }
            }


            else
            {
                desiredMoveLocation = Camera.main.ScreenToWorldPoint(_input);
                if (!_lerpMovement)
                {
                    transform.position = Vector2.MoveTowards(transform.position, desiredMoveLocation, baseSpeed * Time.deltaTime);
                }

                if (_lerpMovement)
                {
                    transform.position = Vector2.Lerp(transform.position, desiredMoveLocation, baseSpeed * Time.deltaTime);
                }

            }

            float yDiff = Mathf.Abs(desiredMoveLocation.y - transform.position.y);
            if (yDiff > distanceToStopRotation)
            {

                if (desiredMoveLocation.y > transform.position.y)
                {
                    targetRotation = Quaternion.Euler(0, 0, maxAngle);
                }

                else if (desiredMoveLocation.y < transform.position.y)
                {
                    targetRotation = Quaternion.Euler(0, 0, minAngle);
                }
            }

            else
            {
                targetRotation = Quaternion.Euler(0, 0, 0);
            }
            _playerSprite.transform.rotation = Quaternion.RotateTowards(_playerSprite.transform.rotation, targetRotation, rotationSpeed);
        }

        public void OnPlayerDeath()
        {
            gameObject.layer = LayerMask.NameToLayer(PLAYER_DEAD_LAYER);
            targetRotation = Quaternion.Euler(0, 0, minAngle);
        }

        private void DeathMovement()
        {
            transform.position += _crashSpeed * Time.deltaTime * Vector3.down;
            _playerSprite.transform.rotation = Quaternion.RotateTowards(_playerSprite.transform.rotation, targetRotation, rotationSpeed);
        }

        private void GoToStartPos()
        {
            transform.position = _spawnPosition.position;      
            _playerSprite.transform.rotation = Quaternion.Euler(0, 0, 0);
            _input = new Vector2(0, 0);
        }

        private void RecieveInput(Vector2 _input)
        {
            this._input =_input;
        }

        public void EnableControls()
        {
            _controlsEnabled = true;
            //_inputManager.IsCursorVisible = false;
        }

        public void DisableControls()
        {
            _controlsEnabled = false;
            _inputManager.IsCursorVisible = true;
        }
    }
}