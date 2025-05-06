using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InventoryItem;

[RequireComponent(typeof(Collider))]
public class PickUp : MonoBehaviour
{
    public InventoryItem inventoryItem;
    private bool isPlayerInRange = false;
    private bool isPickedUp = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Player entered item trigger.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Player exited item trigger.");
        }
    }

    private void Update()
    {
        if (isPlayerInRange && !isPickedUp && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E key pressed while near item.");
            PickUpItem();
        }
    }

    private void PickUpItem()
    {
        if (inventoryItem == null || PlayerInventory.Instance == null)
        {
            Debug.LogError("InventoryItem or PlayerInventory instance is missing.");
            return;
        }

        bool added = PlayerInventory.Instance.AddItem(inventoryItem);

        if (added)
        {
            isPickedUp = true;
            Debug.Log("Successfully picked up: " + inventoryItem.itemName);

            // Optional: disable collider first to avoid repeat triggers before destruction
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            // Optional: play pickup sound here
            // AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            Destroy(gameObject); // Remove item from the scene
        }
        else
        {
            Debug.LogWarning("Cannot pick up " + inventoryItem.itemName + " - Inventory is full!");
        }
    }
}
