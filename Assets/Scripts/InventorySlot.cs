using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public Text quantityText; // Reference to the UI text field
    public Inventory.InventorySlotType slotType;

    Item item;
    InventoryItem inventoryItem;

    public Color selectedColor, notSelectedColor;

    private void Awake()
    {
        inventoryItem = GetComponentInChildren<InventoryItem>();
        Deselect();
    }

    public Item GetItem()
    {
        return item;
    }

    public void AddItem(Item newItem, int quantity)
    {
        item = newItem;

        icon.sprite = item.icon;
        if (inventoryItem != null)
        {
            inventoryItem.image.sprite = icon.sprite;
            inventoryItem.image.color = item.iconColor;
            inventoryItem.image.enabled = true;
        }

        // Update the quantity text field
        quantityText.text = quantity.ToString();
        bool textActive = quantity > 1;
        quantityText.gameObject.SetActive(textActive);
    }

    public void ClearSlot()
    {
        item = null;

        inventoryItem.image.sprite = null;
        inventoryItem.enabled = false;

        // Clear the quantity text field
        quantityText.text = "";
    }

    public void Select()
    {
        inventoryItem.image.color = selectedColor;
        GetComponent<Image>().color = selectedColor;
    }

    public void Deselect()
    {
        if (inventoryItem.image != null)
        {
            inventoryItem.image.color = notSelectedColor;
            GetComponent<Image>().color = notSelectedColor;
        }
    }

    public bool IsEmpty()
    {
        return item == null; // The slot is empty if there is no item assigned to it
    }
}
