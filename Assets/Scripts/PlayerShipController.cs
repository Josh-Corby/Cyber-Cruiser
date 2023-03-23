using UnityEngine;

public class PlayerShipController : MonoBehaviour
{
    [SerializeField] private GameObject playerSprite;

    [SerializeField] private Vector2 input;
    [SerializeField] private GameObject mouseInput;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private float baseSpeed;

    [SerializeField] private bool lerpMovement;

    [SerializeField] private bool controlsEnabled;

    private float minAngle = -20;
    private float maxAngle = 20;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float distanceToStopRotation = 5f;
    private Quaternion targetRotation;

    private void OnEnable()
    {
        InputManager.OnMouseMove += RecieveInput;
        InputManager.OnControlsEnabled += EnableControls;
        InputManager.OnControlsDisabled += DisableControls;

        GameManager.OnLevelCountDownStart += StartLevelPosition;
        GameManager.OnGamePaused += DisableControls;
        GameManager.OnGameResumed += EnableControls;

        Cursor.lockState = CursorLockMode.Confined;

        //EnableControls();
    }

    private void OnDisable()
    {
        InputManager.OnMouseMove -= RecieveInput;
        InputManager.OnControlsEnabled -= EnableControls;
        InputManager.OnControlsDisabled -= DisableControls;

        GameManager.OnLevelCountDownStart -= StartLevelPosition;
        GameManager.OnGamePaused -= DisableControls;
        GameManager.OnGameResumed -= EnableControls;
    }


    private void Update()
    {

        if (!controlsEnabled) return;  

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

    private void StartLevelPosition()
    {
        transform.position = spawnPosition.position;
        playerSprite.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void RecieveInput(Vector2 _input)
    {
        input = _input;
    }

    private void EnableControls()
    {
        controlsEnabled = true;
        mouseInput.SetActive(true);
        Cursor.visible = false;
    }

    private void DisableControls()
    {
        controlsEnabled = false;
        mouseInput.SetActive(false);
        Cursor.visible = true;
    }
}
