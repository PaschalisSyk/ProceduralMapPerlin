using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform inventoryTransform;

    Inventory inventory;

    InventorySlot[] slots;

    int selectedSlot = -1;

    private void Start()
    {
        inventory = Inventory.instance;
        inventory.onItemChangedCallBack += UpdateUI;

        slots = inventory.GetComponentsInChildren<InventorySlot>();

        //ChangeSelectedSlot(0);
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if(i < inventory.items.Count)
            {
                slots[i].AddItem(inventory.items[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }

    void ChangeSelectedSlot(int newValue)
    {
        if(selectedSlot >= 0)
        {
            slots[selectedSlot].Deselect();
        }

        slots[newValue].Select();
        selectedSlot = newValue;
    }
}
