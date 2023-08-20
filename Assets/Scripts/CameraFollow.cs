using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour
{
    Transform _player;
    [SerializeField] float followSpeed = 5f;

    RaycastHit[] _hits = new RaycastHit[10];
    [SerializeField] Vector3 targetOffset = Vector3.up;
    Transform mainCamera;
    LayerMask layerMask;
    List<GameObject> blockingObjects = new List<GameObject>();

    private Dictionary<GameObject, int> originalLayers = new Dictionary<GameObject, int>();


    void Start()
    {
        //_player = GameManager.Instance.player.transform;
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;

        //StartCoroutine(CheckForObjects());
        

        if (_player == null)
        {
            Debug.LogError("Player transform reference is missing! Please assign the player's transform to the 'Player' variable in the inspector.");
        }
    }

    void FixedUpdate()
    {
        if (_player != null)
        {
            // Calculate target position
            Vector3 targetPosition = _player.position;

            // Smoothly move the camera's parent GameObject to follow the player
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }

    IEnumerator CheckForObjects()
    {
        while (true)
        {
            int hits = Physics.RaycastNonAlloc(
                mainCamera.position,
                (_player.position + targetOffset - mainCamera.position).normalized,
                _hits,
                Vector3.Distance(mainCamera.position, _player.position + targetOffset)
            );

            List<GameObject> objectsToReset = new List<GameObject>(blockingObjects);

            // Iterate through the hits and update the objectsToReset list
            for (int i = 0; i < hits; i++)
            {
                if (_hits[i].collider != null)
                {
                    GameObject hitObject = _hits[i].collider.gameObject;
                    if (blockingObjects.Contains(hitObject))
                    {
                        objectsToReset.Remove(hitObject);
                    }
                }
            }

            // Reset the layer of objects that are no longer blocking the view
            foreach (GameObject obj in objectsToReset)
            {
                obj.layer = LayerMask.NameToLayer("Default"); // Change "Default" to your desired layer
                blockingObjects.Remove(obj);
            }

            if (hits > 0)
            {
                for (int i = 0; i < hits; i++)
                {
                    if (_hits[i].collider != null && _hits[i].collider.gameObject.layer != LayerMask.NameToLayer("BlockingObject"))
                    {
                        GameObject hitObject = _hits[i].collider.gameObject;

                        if (!blockingObjects.Contains(hitObject))
                        {
                            blockingObjects.Add(hitObject);

                            // Store the original layer if not already stored
                            if (!originalLayers.ContainsKey(hitObject))
                            {
                                originalLayers.Add(hitObject, hitObject.layer);
                            }

                            // Change the layer to BlockingObject
                            hitObject.layer = LayerMask.NameToLayer("BlockingObject");
                        }
                    }
                }
            }
            else
            {
                // Reset the layers of objects that were previously blocking
                foreach (GameObject blockedObject in blockingObjects)
                {
                    if (originalLayers.TryGetValue(blockedObject, out int originalLayer))
                    {
                        blockedObject.layer = originalLayer;
                    }
                }

                blockingObjects.Clear();
            }

            System.Array.Clear(_hits, 0, _hits.Length);

            yield return null;
        }
    }

}
