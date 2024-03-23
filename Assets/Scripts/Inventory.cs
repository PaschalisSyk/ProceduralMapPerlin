using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [System.Serializable]
    public class InventoryItem
    {
        public Item item;
        public int quantity;
        public InventorySlotType slotType;
    }

    [System.Serializable]
    public enum InventorySlotType
    {
        Item,
        Tool,
        Artifact
    }

    public List<InventoryItem> items = new List<InventoryItem>();

    public int inventorySlots = 4;

    public delegate void OnItemChanged();
    public static event OnItemChanged OnItemChangedCallBack;

    //private void Start()
    //{
    //    if (!GameManager.Instance.NewGame())
    //    {
    //        LoadInventory();
    //    }
    //}

    public bool Add(Item item)
    {
        if (item.stackable)
        {
            foreach (InventoryItem inventoryItem in items)
            {
                if (inventoryItem.item == item)
                {
                    inventoryItem.quantity++;
                    if (OnItemChangedCallBack != null)
                    {
                        OnItemChangedCallBack.Invoke();
                    }
                    return true;
                }
            }
        }

        //if(item.type == Item.ItemType.Tool)
        //{
        //    slotType = InventorySlotType.Tool;
        //}

        if (items.Count >= inventorySlots)
        {
            return false;
        }

        items.Add(new InventoryItem { item = item, quantity = 1 });
        if (OnItemChangedCallBack != null)
        {
            OnItemChangedCallBack.Invoke();
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
        if (OnItemChangedCallBack != null)
        {
            OnItemChangedCallBack.Invoke();
        }
    }

    public bool HasItem(Item item)
    {
        foreach (InventoryItem inventoryItem in items)
        {
            if (inventoryItem.item == item)
            {
                return true;
            }
        }
        return false;
    }

    public void SaveInventory()
    {
        // Serialize inventory data to JSON
        string inventoryData = JsonUtility.ToJson(instance);

        // Save serialized inventory data to PlayerPrefs
        PlayerPrefs.SetString("InventoryData", inventoryData);
        PlayerPrefs.Save();
    }

    public void LoadInventory()
    {
        if (PlayerPrefs.HasKey("InventoryData"))
        {
            // Retrieve serialized inventory data from PlayerPrefs
            string inventoryData = PlayerPrefs.GetString("InventoryData");

            // Deserialize inventory data from JSON
            JsonUtility.FromJsonOverwrite(inventoryData, this);

            // Update current inventory with saved inventory data
            instance.items = this.items;

            //Update UI
            if(OnItemChangedCallBack != null)
            {
                OnItemChangedCallBack?.Invoke();
            }
        }
    }
}