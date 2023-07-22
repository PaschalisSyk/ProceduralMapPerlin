using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 5;
    Vector3 _input;
    Rigidbody rb;
    Map map;
    bool isSwimming = false;
    CapsuleCollider capsuleCollider;
    [SerializeField] GameObject swimParticles;


    private void Awake()
    {
        //map = FindObjectOfType<Map>();
        //transform.position = new Vector3((map.size * map.tileSize) / 2, 2, (map.size * map.tileSize) / 3);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
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

    private void OnCollisionEnter(Collision collision)
    {

        if(collision.gameObject.CompareTag("Water"))
        {
            isSwimming = true;
            capsuleCollider.height = 1;
            capsuleCollider.center = new Vector3(0, 0.5f, 0);
            swimParticles.SetActive(true);
            swimParticles.GetComponentInChildren<ParticleSystem>().Play();
        }
        // Check if the player has collided with a ground tile (you can use a tag or any other identifier for ground tiles)
        if (collision.gameObject.CompareTag("Ground") && isSwimming)
        {
            isSwimming = false;
            capsuleCollider.height = 2;
            capsuleCollider.center = new Vector3(0, 0, 0);
            swimParticles.SetActive(false);
            swimParticles.GetComponentInChildren<ParticleSystem>().Stop();

            // Adjust the player's position to move on top of the ground tile
            Vector3 newPosition = new Vector3(transform.position.x, collision.transform.position.y + 1f, transform.position.z);
            transform.position = newPosition;
        }
    }
}
