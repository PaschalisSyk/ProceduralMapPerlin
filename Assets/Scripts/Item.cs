using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName ="ScriptableObject/Item")]
public class Item : ScriptableObject
{
    new public string name = "New Item";
    public Sprite icon = null;
    public Color iconColor;
    public bool stackable = false;
    public ItemType type;

    public GameObject itemGO;

    public Inventory.InventorySlotType slotType
    {
        get
        {
            // Determine the appropriate slot type based on the item type
            switch (type)
            {
                case ItemType.Tool:
                    return Inventory.InventorySlotType.Tool;
                case ItemType.Collectible:
                    return Inventory.InventorySlotType.Item;
                case ItemType.Artifact:
                    return Inventory.InventorySlotType.Artifact;
                default:
                    return Inventory.InventorySlotType.Item; // Default to Item slot type
            }
        }
    }

    public enum ItemType
    {
        Tool,
        Collectible,
        Artifact
    }

}
