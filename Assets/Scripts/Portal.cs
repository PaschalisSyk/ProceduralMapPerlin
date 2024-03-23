using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class Combination
{
    public EnvironmentType environmentType;
    public List<Item> requiredItems;
}

public class Portal : MonoBehaviour
{
    //[Serializable]
    //public class RequiredItemsForEnvironment
    //{
    //    public EnvironmentType environment;
    //    public List<Inventory.InventoryItem> requiredItems;
    //}

    //public List<RequiredItemsForEnvironment> requiredItemCombinations;

    //public delegate void ChangeEnvironmentAction(EnvironmentType newEnvironment);
    //public static event ChangeEnvironmentAction OnChangeEnvironment;

    //private void OnEnable()
    //{
    //    PlayerController.OnInteractWithPortal += Interact;
    //}

    //private void OnDisable()
    //{
    //    PlayerController.OnInteractWithPortal -= Interact;
    //}

    //// Called when the player interacts with the portal
    //public void Interact()
    //{
    //    // Check each combination of required items
    //    foreach (var combination in requiredItemCombinations)
    //    {
    //        // If the player has all items in this combination, teleport them
    //        if (PlayerHasRequiredItems(combination.requiredItems))
    //        {
    //            TeleportPlayer(combination.environment);
    //            return;
    //        }
    //    }

    //    // If none of the combinations match, inform the player
    //    NotifyPlayerMissingItems();
    //}

    //// Check if the player has all the required items for a given combination
    //private bool PlayerHasRequiredItems(List<Inventory.InventoryItem> requiredItems)
    //{
    //    Inventory inventory = Inventory.instance;
    //    if (inventory == null)
    //    {
    //        Debug.LogWarning("Inventory not found.");
    //        return false;
    //    }

    //    foreach (var requiredItem in requiredItems)
    //    {
    //        bool foundItem = false;
    //        foreach (var inventoryItem in inventory.items)
    //        {
    //            if (inventoryItem.item == requiredItem.item)
    //            {
    //                foundItem = true;
    //                break;
    //            }
    //        }

    //        if (!foundItem)
    //        {
    //            return false;
    //        }
    //    }

    //    return true;
    //}

    //// Teleport the player to the new environment
    //private void TeleportPlayer(EnvironmentType environment)
    //{
    //    // Implement teleportation logic here
    //    OnChangeEnvironment?.Invoke(environment);
    //}

    //// Inform the player that they are missing required items
    //private void NotifyPlayerMissingItems()
    //{
    //    // Implement feedback to inform the player
    //    print("Missing Items");
    //}
    public List<Combination> possibleCombinations;
    public GameObject selectionUIPrefab;
    public Transform slotParent;
    public GameObject combinationSlotPrefab;

    private GameObject selectionUI;
    private List<CombinationSlot> combinationSlots = new List<CombinationSlot>();
    private int selectedSlotIndex = 0;

    public delegate void ChangeEnvironmentAction(EnvironmentType newEnvironment);
    public static event ChangeEnvironmentAction OnChangeEnvironment;

    private void OnEnable()
    {
        PlayerController.OnInteractWithPortal += Interact;
    }

    private void OnDisable()
    {
        PlayerController.OnInteractWithPortal -= Interact;
    }

    private void Update()
    {
        // Handle keyboard navigation
        if (selectionUI != null)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SelectSlot(selectedSlotIndex - 1);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                SelectSlot(selectedSlotIndex + 1);
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                // Teleport the player when Enter key is pressed
                TeleportPlayer(combinationSlots[selectedSlotIndex].GetCombination().environmentType);
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                DeactivateSelectionUI();
            }
        }
    }

    public void Interact()
    {
        // Check if the player has the required items for any combination
        List<Combination> validCombinations = GetValidCombinations();

        if (validCombinations.Count == 0)
        {
            NotifyPlayerMissingItems();
        }
        //else if (validCombinations.Count == 1)
        //{
        //    // If there's only one valid combination, teleport the player directly
        //    TeleportPlayer(validCombinations[0].environmentType);
        //}
        else
        {
            // If there are multiple valid combinations, display UI to let the player choose
            ShowSelectionUI(validCombinations);
        }
    }

    private List<Combination> GetValidCombinations()
    {
        List<Combination> validCombinations = new List<Combination>();
        Inventory inventory = Inventory.instance;

        foreach (Combination combination in possibleCombinations)
        {
            bool hasItems = true;

            foreach (Item requiredItem in combination.requiredItems)
            {
                if (!inventory.HasItem(requiredItem))
                {
                    hasItems = false;
                    break;
                }
            }

            if (hasItems)
            {
                validCombinations.Add(combination);
            }
        }

        return validCombinations;
    }

    private void TeleportPlayer(EnvironmentType environmentType)
    {
        // Teleport the player to the specified environment
        OnChangeEnvironment?.Invoke(environmentType);
    }

    private void NotifyPlayerMissingItems()
    {
        // Implement feedback to inform the player that they are missing required items
        Debug.Log("Missing Required Items");
    }

    private void ShowSelectionUI(List<Combination> validCombinations)
    {
        if(selectionUI == null)
        {
            // Instantiate the selection UI prefab
            selectionUI = Instantiate(selectionUIPrefab, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity) as GameObject;
            slotParent = GameObject.FindGameObjectWithTag("SlotParent").transform;

            // Clear existing combination slots
            ClearCombinationSlots();

            // Create a combination slot for each valid combination
            foreach (Combination combination in validCombinations)
            {
                // Instantiate the combination slot prefab
                GameObject slotObject = Instantiate(combinationSlotPrefab, slotParent) as GameObject;
                CombinationSlot slot = slotObject.GetComponent<CombinationSlot>();

                // Set the combination for the slot
                slot.SetCombination(combination);

                // Add the slot to the list of combination slots
                combinationSlots.Add(slot);
            }

            // Select the first slot by default
            SelectSlot(0);
        }
    }

    private void ClearCombinationSlots()
    {
        foreach (CombinationSlot slot in combinationSlots)
        {
            Destroy(slot.gameObject);
        }
        combinationSlots.Clear();
    }

    private void SelectSlot(int index)
    {
        // Deselect the previously selected slot
        if (selectedSlotIndex >= 0 && selectedSlotIndex < combinationSlots.Count)
        {
            combinationSlots[selectedSlotIndex].Deselect();
        }

        // Update the selected slot index
        selectedSlotIndex = Mathf.Clamp(index, 0, combinationSlots.Count - 1);

        // Select the newly selected slot
        combinationSlots[selectedSlotIndex].Select();
    }

    private void DeactivateSelectionUI()
    {
        // Deactivate the selection UI GameObject
        selectionUI.SetActive(false);

        ClearCombinationSlots();

        // Destroy the selection UI GameObject after a short delay
        Destroy(selectionUI, 0.1f); // Adjust the delay as needed
    }

}
