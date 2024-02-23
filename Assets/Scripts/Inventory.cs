using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
    }

    [System.Serializable]
    public class InventoryItem
    {
        public Item item;
        public int quantity;
    }

    public List<InventoryItem> items = new List<InventoryItem>();

    public int inventorySlots = 4;

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallBack;

    public bool Add(Item item)
    {
        if (item.stackable)
        {
            foreach (InventoryItem inventoryItem in items)
            {
                if (inventoryItem.item == item)
                {
                    inventoryItem.quantity++;
                    if (onItemChangedCallBack != null)
                    {
                        onItemChangedCallBack.Invoke();
                    }
                    return true;
                }
            }
        }

        if (items.Count >= inventorySlots)
        {
            return false;
        }

        items.Add(new InventoryItem { item = item, quantity = 1 });
        if (onItemChangedCallBack != null)
        {
            onItemChangedCallBack.Invoke();
        }

        return true;
    }

    public void Remove(Item item)
    {
        foreach (InventoryItem inventoryItem in items)
        {
            if (inventoryItem.item == item)
            {
                inventoryItem.quantity--;
                if (inventoryItem.quantity <= 0)
                {
                    items.Remove(inventoryItem);
                }
                break;
            }
        }
        if (onItemChangedCallBack != null)
        {
            onItemChangedCallBack.Invoke();
        }
    }
}
