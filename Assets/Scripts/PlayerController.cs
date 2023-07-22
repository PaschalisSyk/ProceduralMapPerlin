using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 5;
    Vector3 _input;
    Rigidbody rb;
    Map map;


    private void Awake()
    {
        map = FindObjectOfType<Map>();
        transform.position = new Vector3((map.size * map.tileSize) / 2, 2, (map.size * map.tileSize) / 3);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        GatherInput();
    }

    private void FixedUpdate()
    {
        Move();
    }

    void GatherInput()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Rotate the input direction by 45 degrees around the y-axis
        Vector3 rotatedInput = Quaternion.Euler(0, 45, 0) * new Vector3(horizontalInput, 0, verticalInput);

        // Normalize the resulting vector to avoid faster diagonal movement
        _input = rotatedInput.normalized;
    }

    void Move()
    {
        rb.MovePosition(transform.position + _input * speed * Time.deltaTime);

        if (_input.magnitude > 0.1f) // Only rotate if there is significant movement
        {
            // Rotate the player to look in the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(_input, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
}
