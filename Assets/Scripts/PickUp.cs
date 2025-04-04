using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InventoryItem;

public class PickUp : MonoBehaviour
{
    public InventoryItem inventoryItem;
    private bool isPlayerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered item trigger."); // Debug log
            isPlayerInRange = true;

        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited item trigger."); // Debug log
            isPlayerInRange = false;
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E key pressed while near item."); // Debug log
            PickUpItem();
        }
    }

    void PickUpItem()
    {
        if (inventoryItem == null || PlayerInventory.instance == null)
        {
            Debug.LogError("InventoryItem or PlayerInventory instance is missing.");
            return;
        }

        Debug.Log("Attempting to pick up: " + inventoryItem.itemName);

        // Try to add the item to the inventory
        bool added = PlayerInventory.instance.AddItem(inventoryItem);

        if (added)
        {
            Debug.Log("Successfully picked up: " + inventoryItem.itemName);
            Destroy(gameObject); // Only destroy if successfully added
        }
        else
        {
            Debug.LogWarning("Cannot pick up " + inventoryItem.itemName + " - Inventory is full!");
        }
    }
}
