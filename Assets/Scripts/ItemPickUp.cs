using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickUp : MonoBehaviour
{
    public Item item;
    public GameObject pickUpText;
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
            pickUpMsg.SetActive(false);
            Destroy(pickUpMsg);
            this.canPickedUp = false;
        }
    }

}
