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

    public List<Item> items = new List<Item>();

    public int inventorySlots = 4;

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallBack;

    public bool Add(Item item)
    {
        if(items.Count >= inventorySlots)
        {
            return false;
        }

        items.Add(item);
        if(onItemChangedCallBack != null)
        {
            onItemChangedCallBack.Invoke();
        }

        return true;
    }

    public void Remove(Item item)
    {
        items.Remove(item);
    }
}
