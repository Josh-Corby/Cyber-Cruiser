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
        InputManager.OnControlsEnabled += ControlsEnabled;
        InputManager.OnControlsDisabled += ControlsDisabled;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        InputManager.OnMouseMove -= RecieveInput;
        InputManager.OnControlsEnabled -= ControlsEnabled;
        InputManager.OnControlsDisabled -= ControlsDisabled;
    }

    private void Start()
    {
        ControlsDisabled();
        transform.position = spawnPosition.position;
    }

    private void Update()
    {
        if (!controlsEnabled) return;

        mouseInput.transform.position = input;

        if (!lerpMovement)
        {
            transform.position = Vector2.MoveTowards(transform.position, input, moveSpeed * Time.deltaTime);
        }

        if (lerpMovement)
        {
            float distance = Vector2.Distance(transform.position, mouseInput.transform.position);
            float speedFactor = Mathf.Abs( 1f - (distance / 1f));
            float speed = baseSpeed * speedFactor;
            float t = Mathf.Clamp01(speed * Time.deltaTime / distance);

            transform.position = Vector2.Lerp(transform.position, mouseInput.transform.position, t);
        }  
    }

    private void RecieveInput(Vector2 _input)
    {
        input = _input;
    }

    private void ControlsEnabled()
    {
        controlsEnabled = true;
        mouseInput.SetActive(true);
    }

    private void ControlsDisabled()
    {
        controlsEnabled = false;
        mouseInput.SetActive(false);
    }
}
