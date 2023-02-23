using UnityEngine;

public class PlayerShipController : MonoBehaviour
{
    [SerializeField] private Vector2 input;
    [SerializeField] private GameObject mousePosition;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private float moveSpeed;

    private void OnEnable()
    {
        InputManager.OnMouseMove += RecieveInput;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        InputManager.OnMouseMove -= RecieveInput;
    }

    private void Start()
    {
        transform.position = spawnPosition.position;
    }

    private void Update()
    {
        mousePosition.transform.position = input;
        transform.position = Vector2.MoveTowards(transform.position, input, moveSpeed * Time.deltaTime);
        //transform.position = input;
    }

    private void RecieveInput(Vector2 _input)
    {
        input = _input;
    }


}
