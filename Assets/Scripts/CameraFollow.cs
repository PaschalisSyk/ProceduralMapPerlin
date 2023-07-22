using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 5f;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player transform reference is missing! Please assign the player's transform to the 'Player' variable in the inspector.");
        }
    }

    private void Update()
    {
        if (player != null)
        {
            // Calculate target position
            Vector3 targetPosition = player.position;

            // Smoothly move the camera's parent GameObject to follow the player
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }
}
