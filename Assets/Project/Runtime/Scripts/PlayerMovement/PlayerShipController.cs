using UnityEngine;

namespace CyberCruiser
{
    public class PlayerShipController : GameBehaviour
    {
        private const string PLAYER_ALIVE_LAYER = "Player";
        private const string PLAYER_DEAD_LAYER = "DeadPlayer";

        #region References
        [SerializeField] private GameObject playerSprite;
        [SerializeField] private GameObject mouseInput;
        [SerializeField] private Transform spawnPosition;
        #endregion

        #region Fields
        [SerializeField] private Vector2 input;
        [SerializeField] private float baseSpeed;
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float distanceToStopRotation = 5f;
        [SerializeField] private bool lerpMovement;
        [SerializeField] private bool _controlsEnabled;
        private bool _isPlayerDead;
        [SerializeField] private float _crashSpeed;

        private readonly float minAngle = -20;
        private readonly float maxAngle = 20;
        private Quaternion targetRotation;
        #endregion

        #region Properties
        public bool ControlsEnabled { get => _controlsEnabled; set => _controlsEnabled = value; }
        #endregion

        private void OnEnable()
        {
            _isPlayerDead = false;
            InputManager.OnMouseMove += RecieveInput;
            GameManager.OnMissionStart += StartLevelPosition;

            CyberKrakenGrappleTentacle.OnGrappleEnd += EnableControls;
            PlayerManager.OnPlayerDeath += PlayerDead;

            Cursor.lockState = CursorLockMode.Confined;
            gameObject.layer = LayerMask.NameToLayer(PLAYER_ALIVE_LAYER);
        }

        private void OnDisable()
        {
            InputManager.OnMouseMove -= RecieveInput;
            GameManager.OnMissionStart -= StartLevelPosition;

            CyberKrakenGrappleTentacle.OnGrappleEnd -= EnableControls;
            PlayerManager.OnPlayerDeath -= PlayerDead;
        }

        private void Update()
        {
            if (_isPlayerDead)
            {
                DeathMovement();
            }

            else if (_controlsEnabled)
            {
                PlayerMovement();
            }
        }

        private void PlayerMovement()
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(input);
            mousePosition.z = 0f;
            mouseInput.transform.position = mousePosition;

            if (!lerpMovement)
            {
                transform.position = Vector2.MoveTowards(transform.position, mousePosition, baseSpeed * Time.deltaTime);
            }

            if (lerpMovement)
            {
                transform.position = Vector2.Lerp(transform.position, mousePosition, baseSpeed);
            }

            float yDiff = Mathf.Abs(mousePosition.y - transform.position.y);
            if (yDiff > distanceToStopRotation)
            {

                if (mousePosition.y > transform.position.y)
                {
                    targetRotation = Quaternion.Euler(0, 0, maxAngle);
                }

                else if (mousePosition.y < transform.position.y)
                {
                    targetRotation = Quaternion.Euler(0, 0, minAngle);
                }
            }

            else
            {
                targetRotation = Quaternion.Euler(0, 0, 0);
            }
            playerSprite.transform.rotation = Quaternion.RotateTowards(playerSprite.transform.rotation, targetRotation, rotationSpeed);
        }

        private void PlayerDead()
        {
            gameObject.layer = LayerMask.NameToLayer(PLAYER_DEAD_LAYER);
            _isPlayerDead = true;
            targetRotation = Quaternion.Euler(0, 0, minAngle);
        }

        private void DeathMovement()
        {
            transform.position += _crashSpeed * Time.deltaTime * Vector3.down;
            playerSprite.transform.rotation = Quaternion.RotateTowards(playerSprite.transform.rotation, targetRotation, rotationSpeed);
        }

        private void StartLevelPosition()
        {
            transform.position = spawnPosition.position;
            playerSprite.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        private void RecieveInput(Vector2 _input)
        {
            input = _input;
        }

        public void EnableControls()
        {
            _controlsEnabled = true;
            mouseInput.SetActive(true);
            InputManagerInstance.IsCursorVisible = false;
        }

        public void DisableControls()
        {
            _controlsEnabled = false;
            mouseInput.SetActive(false);
            InputManagerInstance.IsCursorVisible = true;
        }
    }
}