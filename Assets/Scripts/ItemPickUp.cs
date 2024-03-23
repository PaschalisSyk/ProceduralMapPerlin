using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickUp : MonoBehaviour
{
    public Item item;
    public GameObject pickUpText;
    public Transform handTransform;
    [SerializeField] Vector3 toolPosition;
    [SerializeField] Vector3 toolRot;

    private GameObject pickUpMsg;
    private bool canPickedUp = false;


    private void OnEnable()
    {
        PlayerController.OnItemPickUp += PickUp;
    }

    private void OnDisable()
    {
        PlayerController.OnItemPickUp -= PickUp;
    }

    private void Start()
    {
        if(this.item.type == Item.ItemType.Tool)
        {
            handTransform = GameObject.FindGameObjectWithTag("PlayerPalm").transform;
        }
    }

    void PickUp()
    {
        if(canPickedUp)
        {
            bool wasPIckedUp = Inventory.instance.Add(this.item);
            if (pickUpMsg != null)
            {
                pickUpMsg.SetActive(false);
                Destroy(pickUpMsg);
            }

            if (wasPIckedUp)
            {
                Destroy(gameObject, 1);

                if(this.item.type == Item.ItemType.Tool)
                {
                    if(item.itemGO != null)
                    {
                        GameObject toolGO = Instantiate(item.itemGO, transform.position, Quaternion.identity) as GameObject;
                        PickUpTool(toolGO);
                    }
                }
            }
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().canPickUp = true;
            pickUpMsg = Instantiate(pickUpText, new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), Quaternion.identity) as GameObject;
            this.canPickedUp = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().canPickUp = false;
            if(pickUpMsg != null)
            {
                pickUpMsg.SetActive(false);
                Destroy(pickUpMsg);
            }
            this.canPickedUp = false;
        }
    }

    // Function to pick up an item
    public void PickUpTool(GameObject item)
    {
        // Check if the item is already held
        if (item.transform.parent == handTransform)
        {
            Debug.LogWarning("Item is already held!");
            return;
        }

        // Detach from current parent (if any)
        item.transform.SetParent(null);

        // Attach to the hand
        item.transform.SetParent(handTransform);

        // Move the item to the hand position
        item.transform.localPosition = toolPosition;
        item.transform.localEulerAngles = toolRot;


        // Optionally disable item physics or collision
        var rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Disable physics
            rb.detectCollisions = false; // Disable collisions
        }

        Debug.Log("Picked up item: " + item.name);
    }

    // Function to drop an item
    public void DropItem(GameObject item)
    {
        // Detach from hand
        item.transform.SetParent(null);

        // Optionally re-enable physics and collisions
        var rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false; // Enable physics
            rb.detectCollisions = true; // Enable collisions
        }

        Debug.Log("Dropped item: " + item.name);
    }
}
