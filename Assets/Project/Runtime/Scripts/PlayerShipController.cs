using UnityEngine;

public class PlayerShipController : GameBehaviour
{
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
        InputManager.OnControlsEnabled += EnableControls;
        InputManager.OnControlsDisabled += DisableControls;
        GameManager.OnMissionStart += StartLevelPosition;
        GameManager.OnIsGamePaused += ToggleControls;
        CyberKrakenGrappleTentacle.OnGrappleEnd += EnableControls;
        Cursor.lockState = CursorLockMode.Confined;

        PlayerManager.OnPlayerDeath += PlayerDead;
    }

    private void OnDisable()
    {
        InputManager.OnMouseMove -= RecieveInput;
        InputManager.OnControlsEnabled -= EnableControls;
        InputManager.OnControlsDisabled -= DisableControls;
        GameManager.OnMissionStart -= StartLevelPosition;
        GameManager.OnIsGamePaused -= ToggleControls;
        CyberKrakenGrappleTentacle.OnGrappleEnd -= EnableControls;
        PlayerManager.OnPlayerDeath -= PlayerDead;
    }



    private void Update()
    {
        if (ControlsEnabled)
        {
            PlayerMovement();
        }

        else if (_isPlayerDead)
        {
            DeathMovement();
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
        _isPlayerDead = true;
    }
    private void DeathMovement()
    {
        transform.position += _crashSpeed * Time.deltaTime * Vector3.down;
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

    public void ToggleControls(bool value)
    {
        if (value)
        {
            DisableControls();
        }
        else
        {
            EnableControls();
        }
    }

    private void EnableControls()
    {
        _controlsEnabled = true;
        mouseInput.SetActive(true);
        IM.IsCursorVisible = false;
    }

    private void DisableControls()
    {
        _controlsEnabled = false;
        mouseInput.SetActive(false);
        IM.IsCursorVisible = true;
    }
}
