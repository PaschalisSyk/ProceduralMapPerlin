using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class InventoryUI : MonoBehaviour
{
    public Transform inventoryTransform;
    public Transform toolsTransform;
    public GameObject inventoryUI;

    Inventory inventory;

    InventorySlot[] itemSlots;
    InventorySlot[] toolSlots;

    int selectedSlot = -1;

    public InventorySlot[] GetSlots()
    {
        return itemSlots;
    }

    public bool IsInventoryOn()
    {
        return inventoryUI.activeInHierarchy;
    }


    private void OnEnable()
    {
        Inventory.OnItemChangedCallBack += UpdateUI;
    }

    private void OnDisable()
    {
        Inventory.OnItemChangedCallBack -= UpdateUI;
    }

    private void Start()
    {
        inventory = Inventory.instance;
        inventoryUI.SetActive(false);

        itemSlots = inventoryTransform.GetComponentsInChildren<InventorySlot>();
        toolSlots = toolsTransform.GetComponentsInChildren<InventorySlot>();

        UpdateUI();
        //ChangeSelectedSlot(0);
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
        }
        HandleKeyboardNavigation();
    }

    void UpdateUI()
    {
        if(itemSlots != null)
        {
            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (i < inventory.items.Count)
                {
                    // Check if the item is a tool and place it in the appropriate tool slot
                    if (inventory.items[i].item.slotType == Inventory.InventorySlotType.Tool)
                    {
                        int toolSlotIndex = GetToolSlotIndex(inventory.items[i].item);
                        if (toolSlotIndex != -1)
                        {
                            // Use the GetItem method to access the item field
                            Item toolItem = toolSlots[toolSlotIndex].GetItem();
                            if (toolItem == null || toolItem == inventory.items[i].item)
                            {
                                toolSlots[toolSlotIndex].AddItem(inventory.items[i].item, inventory.items[i].quantity);
                            }
                        }
                    }
                    else
                    {
                        // Place non-tool items in regular item slots
                        itemSlots[i].AddItem(inventory.items[i].item, inventory.items[i].quantity);
                    }
                }
                else
                {
                    itemSlots[i].ClearSlot();
                }
            }
        }
        else
        {
            print("ItemSlotsNull");
        }
        
    }

    // Get the index of the tool slot that corresponds to the specified tool item
    private int GetToolSlotIndex(Item item)
    {
        for (int i = 0; i < toolSlots.Length; i++)
        {
            // Check if the tool slot is empty or contains the same item
            if (toolSlots[i].GetItem() == null || toolSlots[i].GetItem() == item)
            {
                return i; // Return the index of the empty tool slot or slot with the same item
            }
        }
        return -1; // Return -1 if no empty tool slot or slot with the same item is found
    }

    void HandleKeyboardNavigation()
    {
        int columns = Mathf.CeilToInt(Mathf.Sqrt(itemSlots.Length));
        int rows = Mathf.CeilToInt(itemSlots.Length / (float)columns);

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // Navigate up
            int newIndex = selectedSlot - columns;
            if (newIndex >= 0)
                ChangeSelectedSlot(newIndex);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            // Navigate down
            int newIndex = selectedSlot + columns;
            if (newIndex < itemSlots.Length)
                ChangeSelectedSlot(newIndex);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // Navigate left
            int newIndex = selectedSlot - 1;
            if (newIndex >= 0 && newIndex / columns == selectedSlot / columns)
                ChangeSelectedSlot(newIndex);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            // Navigate right
            int newIndex = selectedSlot + 1;
            if (newIndex < itemSlots.Length && newIndex / columns == selectedSlot / columns)
                ChangeSelectedSlot(newIndex);
        }
    }

    void ChangeSelectedSlot(int newValue)
    {
        // Ensure the new index is within bounds
        newValue = Mathf.Clamp(newValue, 0, itemSlots.Length - 1);

        // Deselect the currently selected slot
        if (selectedSlot >= 0 && selectedSlot < itemSlots.Length)
        {
            itemSlots[selectedSlot].Deselect();
        }

        // Select the new slot
        itemSlots[newValue].Select();
        selectedSlot = newValue;
    }
    int GetHorizontalSlotsCount()
    {
        // Calculate the number of horizontal slots in a row
        // Assuming the inventory is laid out in a grid pattern
        // You might need to adjust this based on your actual layout
        int horizontalSlotsCount = 1;

        if (itemSlots.Length > 0)
        {
            int siblingIndex = itemSlots[0].transform.GetSiblingIndex();
            for (int i = 1; i < itemSlots.Length; i++)
            {
                if (itemSlots[i].transform.GetSiblingIndex() != siblingIndex + i)
                {
                    break;
                }
                horizontalSlotsCount++;
            }
        }

        return horizontalSlotsCount;
    }
}
