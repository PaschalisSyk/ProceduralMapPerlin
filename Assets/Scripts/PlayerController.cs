using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 5;
    Vector3 _input;
    Rigidbody rb;
    Map map;
    bool isSwimming = false;
    CapsuleCollider capsuleCollider;
    [SerializeField] GameObject swimParticles;
    Animator anim;
    bool isInPortal;
    public bool canPickUp = false;

    // Define a delegate for the event
    public delegate void TabPressedAction();
    //public delegate void ChangeEnviromentAction(EnvironmentType newEnvironment);
    public delegate void PickUpItemAction();
    public delegate void OnInteractWithPortalAction();

    // Define the event using the delegate
    public static event TabPressedAction OnTabPressed;
    //public static event ChangeEnviromentAction OnChangeEnviroment;
    public static event PickUpItemAction OnItemPickUp;
    public static event OnInteractWithPortalAction OnInteractWithPortal;

    //EnvironmentType nextEnviroment;

    public static int PosID = Shader.PropertyToID("_PlayerPos");
    Material waterMat;
    Camera mainCam;
    InventoryUI inventoryUI;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        inventoryUI = FindObjectOfType<InventoryUI>();
        //map = FindObjectOfType<Map>();
        //transform.position = new Vector3((map.size * map.tileSize) / 2, 2, (map.size * map.tileSize) / 3);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        waterMat = GameObject.FindGameObjectWithTag("Water").GetComponent<MeshRenderer>().material;
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    private void Update()
    {
        if(inventoryUI.IsInventoryOn())
        {
            return;
        }
        GatherInput();
        EllipseFollow();
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Invoke the event when Tab is pressed
            OnTabPressed?.Invoke();
        }
        if(isInPortal && Input.GetKeyDown(KeyCode.R))
        {
            //nextEnviroment = (EnvironmentType)Random.Range(0, System.Enum.GetValues(typeof(EnvironmentType)).Length);
            //OnChangeEnviroment?.Invoke(nextEnviroment);

            // Invoke the event when the player interacts with the portal
            OnInteractWithPortal?.Invoke();

        }
        if (canPickUp && Input.GetKeyDown(KeyCode.E))
        {
            OnItemPickUp?.Invoke();
            anim.SetTrigger("isPickingUp");
            canPickUp = false;
        }
    }

    private void FixedUpdate()
    {
        if(isSwimming)
        {
            gameObject.layer = LayerMask.NameToLayer("BlockingObject");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
        }
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
            anim.SetFloat("speed", _input.magnitude, 0.1f, Time.deltaTime);
        }
        else
        {
            anim.SetFloat("speed", 0);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if(collision.gameObject.CompareTag("Water"))
        {
            float colliderHeight = 1.5f;
            isSwimming = true;
            anim.SetBool("isSwimming", isSwimming);
            capsuleCollider.height = 1f;
            capsuleCollider.center = new Vector3(0, colliderHeight, 0);
            swimParticles.SetActive(true);
            swimParticles.GetComponentInChildren<ParticleSystem>().Play();
        }
        // Check if the player has collided with a ground tile (you can use a tag or any other identifier for ground tiles)
        if (collision.gameObject.CompareTag("Ground") && isSwimming)
        {
            isSwimming = false;
            anim.SetBool("isSwimming", isSwimming);
            capsuleCollider.height = 3.5f;
            capsuleCollider.center = new Vector3(0, 1.75f, 0);
            swimParticles.SetActive(false);
            swimParticles.GetComponentInChildren<ParticleSystem>().Stop();

            // Adjust the player's position to move on top of the ground tile
            Vector3 newPosition = new Vector3(transform.position.x, collision.transform.position.y + 0.8f, transform.position.z);
            transform.position = newPosition;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Portal"))
        {
            isInPortal = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Portal"))
        {
            isInPortal = false;
        }
    }

    void EllipseFollow()
    {
        var view = mainCam.WorldToViewportPoint(transform.position);
        waterMat.SetVector(PosID, view);
    }
}
