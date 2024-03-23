using UnityEngine;

public class Bird : MonoBehaviour
{
    public float walkingSpeed = 1f;
    public float flyingSpeed = 5f;
    public float flyingHeight = 5f;
    public float landingSpeed = 2f;
    public float detectionRange = 10f;

    private Vector3 startPosition;
    private bool isFlying = false;
    private bool isGrounded = true;
    private bool isPlayerNearby = false;

    Animator anim;

    void Start()
    {
        startPosition = transform.position;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (isFlying)
        {
            Fly();
            anim.SetBool("isFlying", true);
        }
        else if (isGrounded)
        {
            Walk();
            anim.SetBool("isFlying", false);
        }
    }

    void Walk()
    {
        // Walk forward along the ground
        transform.Translate(Vector3.forward * Time.deltaTime * walkingSpeed);
    }

    void Fly()
    {
        // Fly forward and oscillate vertically
        transform.Translate(Vector3.up * Mathf.Sin(Time.time * flyingSpeed) * flyingHeight * Time.deltaTime);
        transform.Translate(Vector3.forward * Time.deltaTime * flyingSpeed);
    }

    void Land()
    {
        // Land on the ground
        transform.Translate(Vector3.down * Time.deltaTime * landingSpeed);
        if(transform.position.y <= 0.4f)
        {
            transform.position = new Vector3(transform.position.x, 0.4f, transform.position.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (!isFlying)
            {
                isFlying = true;
                isGrounded = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (!isGrounded)
            {
                isFlying = false;
                isGrounded = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isPlayerNearby && Random.Range(0f, 1f) < 0.01f)
        {
            isFlying = !isFlying;
            isGrounded = !isGrounded;
        }
    }
}
