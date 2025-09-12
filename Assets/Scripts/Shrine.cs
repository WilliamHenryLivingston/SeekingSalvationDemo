//Copyright 2025 William Livingston 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrineTable : MonoBehaviour
{
    public Transform tableSpot; // point where items are visually placed (optional)
    public float interactRange = 3f;

    private PlayerInventory playerInventory;
    private Camera playerCam;

    private int storedSpiritualSignificance = 0;

    void Start()
    {
        // You can grab the player's camera at runtime
        playerCam = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPlaceItem();
        }
    }

    void TryPlaceItem()
    {
        // Raycast from center of screen
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            if (hit.collider.gameObject == this.gameObject) // we hit the table
            {
                if (playerInventory == null)
                    playerInventory = FindObjectOfType<PlayerInventory>();

                if (playerInventory == null) return;

                InventoryItem firstItem = playerInventory.GetFirstItem();
                if (firstItem == null)
                {
                    Debug.Log("No items to place on the shrine.");
                    return;
                }

                // Remove from inventory
                playerInventory.RemoveFirstItem();

                // Add its spiritual significance
                storedSpiritualSignificance += firstItem.spiritualSignificance;

                Debug.Log($"Placed {firstItem.itemName} on the shrine. Total Spiritual Value: {storedSpiritualSignificance}");

                // OPTIONAL: spawn a dummy prefab on the table visually
                if (tableSpot != null)
                {
                    GameObject placed = GameObject.CreatePrimitive(PrimitiveType.Cube); 
                    placed.transform.position = tableSpot.position + new Vector3(0, 0.2f, 0);
                    placed.transform.localScale = Vector3.one * 0.3f;
                    Destroy(placed, 5f); // remove later
                }
            }
        }
    }

    public int CollectStoredValue()
    {
        int value = storedSpiritualSignificance;
        storedSpiritualSignificance = 0; // reset after offering
        return value;
    }
}
