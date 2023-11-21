using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public Item item;

    void PickUp()
    {
        bool wasPIckedUp = Inventory.instance.Add(item);
        //GetComponent<MeshRenderer>().material.color = item.iconColor;

        if(wasPIckedUp)
        {
            Destroy(gameObject);

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            PickUp();
        }
    }
}
