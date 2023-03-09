using UnityEngine;

public class PlayerShipController : MonoBehaviour
{
    [SerializeField] private Vector2 input;
    [SerializeField] private GameObject mouseInput;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private float baseSpeed;
    [SerializeField] private float moveSpeed;


    #region lerpmovement
    [SerializeField] private float maxSpeed;
    [SerializeField] private bool lerpMovement;
    #endregion

    private bool controlsEnabled;

    private void OnEnable()
    {
        InputManager.OnMouseMove += RecieveInput;
        InputManager.OnControlsEnabled += EnableControls;
        InputManager.OnControlsDisabled += DisableControls;

        GameManager.OnLevelCountDownStart += StartLevelPosition;
        GameManager.OnGamePaused += DisableControls;
        GameManager.OnGameResumed += EnableControls;

        Cursor.lockState = CursorLockMode.Confined;
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
        Vector3 mouseWorldPosition = mousePosition;
        mouseInput.transform.position = mouseWorldPosition;

        if (!lerpMovement)
        {
            transform.position = Vector2.MoveTowards(transform.position, mouseWorldPosition, moveSpeed * Time.deltaTime);
        }

        if (lerpMovement)
        {
            float distance = Vector2.Distance(transform.position, mouseWorldPosition);
            float speedFactor = Mathf.Abs( 1f - (distance / 1f));
            float speed = baseSpeed * speedFactor;
            float t = Mathf.Clamp01(speed * Time.deltaTime / distance);

            transform.position = Vector2.Lerp(transform.position, mouseWorldPosition, t);
        }  
    }

    private void StartLevelPosition()
    {
        transform.position = spawnPosition.position;
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
