using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// Remove 'using static InventoryItem;' if InventoryItem is a class/ScriptableObject now

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;
    public List<InventoryItem> inventoryItems = new List<InventoryItem>();

    public Transform inventoryUIParent;     // Assign your inventory panel (e.g., a Panel UI element)
    public GameObject inventorySlotPrefab;
    public float dropOffset = 1.5f;       // How far in front of the player to drop the item

    private RectTransform inventoryPanelRect; // Store the RectTransform for bounds checking

    public int maxInventorySize = 10;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject); // Optional: if your inventory persists between scenes
        }
        else
        {
            Destroy(gameObject);
            return; // Added return to prevent further execution in Awake if destroyed
        }

        // Get the RectTransform of the inventory panel
        inventoryPanelRect = inventoryUIParent.GetComponent<RectTransform>();
        if (inventoryPanelRect == null)
        {
            Debug.LogError("Inventory UI Parent does not have a RectTransform component!", inventoryUIParent);
        }
    }

    public bool AddItem(InventoryItem item)
    {
        if (item == null) return false;

        if (inventoryItems.Count >= maxInventorySize)
        {
            Debug.LogWarning("Inventory is full Cannot pick up" + item.itemName);
                return false;
        }
        inventoryItems.Add(item);
        Debug.Log(item.itemName + " added to inventory.");

        //UpdateUI
        AddItemToUI(item);

        return true;
    }

    private void AddItemToUI(InventoryItem item)
    {
        if (inventoryUIParent == null || inventorySlotPrefab == null)
        {
            Debug.LogError("Inventory UI Parent or Slot Prefab is not assigned in the Inspector!");
            return;
        }

        GameObject newSlot = Instantiate(inventorySlotPrefab, inventoryUIParent);
        newSlot.name = item.itemName + "_Slot"; // Helps with debugging

        Image itemImage = newSlot.GetComponentInChildren<Image>(); // More robust: find Image in children

        if (itemImage != null)
        {
            itemImage.sprite = item.itemIcon;
            itemImage.enabled = true; // Make sure image is enabled
        }
        else
        {
            Debug.LogWarning("Inventory Slot Prefab is missing an Image component in its children!", newSlot);
        }

        DraggableItem draggableItem = newSlot.GetComponent<DraggableItem>();
        if (draggableItem == null)
        {
            draggableItem = newSlot.AddComponent<DraggableItem>(); // Add Drag functionality
        }

        // --- Crucial Setup for DraggableItem ---
        draggableItem.item = item;                      // Link item data
        draggableItem.image = itemImage;                // Link the Image component
        draggableItem.originalParent = inventoryUIParent; // Set the parent
        draggableItem.inventoryPanelRect = this.inventoryPanelRect; // Pass the panel bounds
        // --- End Setup ---
    }

    // Called by DraggableItem when an item is dropped outside the inventory panel
    public bool DropItemToWorld(InventoryItem itemToDrop)
    {
        if (itemToDrop == null) return false;

        // 1. Check if the item has a prefab to instantiate
        if (itemToDrop.itemPrefab == null)
        {
            Debug.LogWarning($"Item '{itemToDrop.itemName}' cannot be dropped because it has no itemPrefab assigned.");
            return false; // Indicate failure
        }

        // 2. Remove the item from the core inventory list
        bool removed = inventoryItems.Remove(itemToDrop);
        if (!removed)
        {
            // This shouldn't happen if the DraggableItem's reference was correct, but good to check
            Debug.LogError($"Failed to remove item '{itemToDrop.itemName}' from inventory list during drop.");
            return false; // Indicate failure
        }

        Debug.Log($"Removed '{itemToDrop.itemName}' from inventory list.");

        // 3. Instantiate the item's prefab in the world
        // Calculate drop position (e.g., slightly in front of the player)
        Vector3 dropPosition = transform.position + (transform.forward * dropOffset);

        // Optional: Add a small vertical offset so it doesn't clip into the ground immediately
        dropPosition += Vector3.up * 0.2f;

        // Optional: Raycast down to find the actual ground position
        if (Physics.Raycast(dropPosition + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 10f))
        {
            dropPosition = hit.point + Vector3.up * 0.1f; // Place slightly above the hit point
        }

        Instantiate(itemToDrop.itemPrefab, dropPosition, Quaternion.identity); // Use default rotation or item specific rotation
        Debug.Log($"Instantiated '{itemToDrop.itemName}' prefab at {dropPosition}.");

        // 4. The DraggableItem script will handle destroying the UI GameObject

        return true; // Indicate success
    }

    // Optional: Method to remove item by reference (e.g., if picked up by another system)
    public void RemoveItem(InventoryItem itemToRemove)
    {
        if (inventoryItems.Remove(itemToRemove))
        {
            Debug.Log($"Removed {itemToRemove.itemName} from inventory.");
            // Find and destroy the corresponding UI slot
            foreach (Transform childSlot in inventoryUIParent)
            {
                DraggableItem dragItem = childSlot.GetComponent<DraggableItem>();
                if (dragItem != null && dragItem.item == itemToRemove)
                {
                    Destroy(childSlot.gameObject);
                    break; // Found and destroyed, exit loop
                }
            }
        }
    }
}