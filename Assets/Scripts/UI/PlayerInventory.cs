//Copyright 2025 William Livingston
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    [Header("Inventory Data")]
    public List<InventoryItem> inventoryItems = new List<InventoryItem>();
    public int currency = 0;
    public int coins = 0;
    public int maxInventorySize = 10;

    [Header("UI References")]
    public Transform inventoryUIParent;
    public GameObject inventorySlotPrefab;
    private RectTransform inventoryPanelRect;

    [Header("Drop Settings")]
    public float dropOffset = 1.5f;

    // New Properties (Step Two)
    public float TotalWeight { get; private set; }
    public int TotalSpiritualSignificance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
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
            Debug.LogWarning("Inventory is full. Cannot pick up " + item.itemName);
            return false;
        }

        inventoryItems.Add(item);
        Debug.Log(item.itemName + " added to inventory.");

        RecalculateStats(); // Update stats when items are added

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
        newSlot.name = item.itemName + "_Slot";

        Image itemImage = newSlot.GetComponentInChildren<Image>();

        if (itemImage != null)
        {
            itemImage.sprite = item.itemIcon;
            itemImage.enabled = true;
        }
        else
        {
            Debug.LogWarning("Inventory Slot Prefab is missing an Image component in its children!", newSlot);
        }

        DraggableItem draggableItem = newSlot.GetComponent<DraggableItem>();
        if (draggableItem == null)
        {
            draggableItem = newSlot.AddComponent<DraggableItem>();
        }

        draggableItem.item = item;
        draggableItem.image = itemImage;
        draggableItem.originalParent = inventoryUIParent;
        draggableItem.inventoryPanelRect = this.inventoryPanelRect;
    }

    public bool DropItemToWorld(InventoryItem itemToDrop)
    {
        if (itemToDrop == null) return false;

        if (itemToDrop.itemPrefab == null)
        {
            Debug.LogWarning($"Item '{itemToDrop.itemName}' cannot be dropped because it has no itemPrefab assigned.");
            return false;
        }

        bool removed = inventoryItems.Remove(itemToDrop);
        if (!removed)
        {
            Debug.LogError($"Failed to remove item '{itemToDrop.itemName}' from inventory list during drop.");
            return false;
        }

        Debug.Log($"Removed '{itemToDrop.itemName}' from inventory list.");

        RecalculateStats(); // Update stats when items are dropped

        Vector3 dropPosition = transform.position + (transform.forward * dropOffset);
        dropPosition += Vector3.up * 0.2f;

        if (Physics.Raycast(dropPosition + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 10f))
        {
            dropPosition = hit.point + Vector3.up * 0.1f;
        }

        Instantiate(itemToDrop.itemPrefab, dropPosition, Quaternion.identity);
        Debug.Log($"Instantiated '{itemToDrop.itemName}' prefab at {dropPosition}.");

        return true;
    }

    public void RemoveItem(InventoryItem itemToRemove)
    {
        if (inventoryItems.Remove(itemToRemove))
        {
            Debug.Log($"Removed {itemToRemove.itemName} from inventory.");
            RecalculateStats(); // Update stats when items are removed

            foreach (Transform childSlot in inventoryUIParent)
            {
                DraggableItem dragItem = childSlot.GetComponent<DraggableItem>();
                if (dragItem != null && dragItem.item == itemToRemove)
                {
                    Destroy(childSlot.gameObject);
                    break;
                }
            }
        }
    }

    public bool HasItem(string itemName)
    {
        return inventoryItems.Any(item => item.itemName == itemName);
    }

    public bool RemoveItemByName(string itemName)
    {
        var itemToRemove = inventoryItems.FirstOrDefault(item => item.itemName == itemName);
        if (itemToRemove != null)
        {
            RemoveItem(itemToRemove);
            return true;
        }

        Debug.LogWarning($"Tried to remove item '{itemName}' but it wasn't found in inventory.");
        return false;
    }

    // New Method for Step Two
    private void RecalculateStats()
    {
        TotalWeight = 0f;
        TotalSpiritualSignificance = 0;

        foreach (var item in inventoryItems)
        {
            TotalWeight += item.weight;
            TotalSpiritualSignificance += item.spiritualSignificance;
        }

        Debug.Log($"[Inventory Stats] Weight: {TotalWeight}, Spiritual: {TotalSpiritualSignificance}");
    }

    // --- SHRINE SUPPORT METHODS ---
    public InventoryItem GetFirstItem()
    {
        if (inventoryItems.Count == 0)
            return null;
        return inventoryItems[0];
    }

    public void RemoveFirstItem()
    {
        if (inventoryItems.Count == 0) return;

        InventoryItem itemToRemove = inventoryItems[0];
        inventoryItems.RemoveAt(0);

        Debug.Log($"Removed {itemToRemove.itemName} from inventory (first slot).");
        RecalculateStats();

        // Also remove from UI
        foreach (Transform childSlot in inventoryUIParent)
        {
            DraggableItem dragItem = childSlot.GetComponent<DraggableItem>();
            if (dragItem != null && dragItem.item == itemToRemove)
            {
                Destroy(childSlot.gameObject);
                break;
            }
        }
    }

}
