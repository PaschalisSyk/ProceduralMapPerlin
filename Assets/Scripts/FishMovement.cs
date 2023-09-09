using UnityEngine;

public class FishMovement : MonoBehaviour
{
    [SerializeField] private float swimSpeed = 3f;
    [SerializeField] private float changeDirectionChance = 0.05f;

    private Rigidbody rb;
    private Vector3 swimDirection;
    MeshCollider waterCol;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        swimDirection = GetRandomSwimDirection();
        waterCol = GameObject.FindWithTag("Water").GetComponent<MeshCollider>();
        swimSpeed = Random.Range(3, 5);
    }

    private void FixedUpdate()
    {
        MoveFish();
    }

    private void MoveFish()
    {
        Quaternion targetRotation = Quaternion.LookRotation(swimDirection, Vector3.up);
        rb.MoveRotation(targetRotation);

        Vector3 newPosition = transform.position + swimDirection * swimSpeed * Time.fixedDeltaTime;
        newPosition = new Vector3(
            Mathf.Clamp(newPosition.x, waterCol.bounds.min.x, waterCol.bounds.max.x),
            Mathf.Clamp(newPosition.y, -2, waterCol.bounds.max.y -0.6f),
            Mathf.Clamp(newPosition.z, waterCol.bounds.min.z, waterCol.bounds.max.z)
        );

        rb.MovePosition(newPosition);

        if (Random.Range(0f, 1f) <= changeDirectionChance)
        {
            swimDirection = GetRandomSwimDirection();
        }
    }

    private Vector3 GetRandomSwimDirection()
    {
        return new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
    }
}

