using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;

    Item item;

    InventoryItem inventoryItem;

    public Color selectedColor, notSelectedColor;

    private void Awake()
    {
        inventoryItem = GetComponentInChildren<InventoryItem>();
        Deselect();
    }

    public void AddItem(Item newItem)
    {
        item = newItem;

        icon.sprite = item.icon;
        //icon.enabled = true;
        if(inventoryItem != null)
        {
            inventoryItem.image.sprite = icon.sprite;
            inventoryItem.image.color = item.iconColor;
            inventoryItem.image.enabled = true;
        }
    }

    public void ClearSlot()
    {
        item = null;

        inventoryItem.image.sprite = null;
        inventoryItem.enabled = false;
    }

    public void Select()
    {
        inventoryItem.image.color = selectedColor;
    }

    public void Deselect()
    {
        if(inventoryItem.image != null)
        {
            inventoryItem.image.color = notSelectedColor;
        }
    }
}
