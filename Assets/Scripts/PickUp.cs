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
        if(other.CompareTag("Player"))
        {
            Debug.Log("Player entered item trigger."); // Debug log
            isPlayerInRange = true;
            
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("Player exited item trigger."); // Debug log
            isPlayerInRange = false;
        }
    }

    private void Update()
    {
        if(isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E key pressed while near item."); // Debug log
            PickUpItem();
        }
    }

    void PickUpItem()
    {
        Debug.Log("Picked Up:" + inventoryItem.itemName);
        PlayerInventory.instance.AddItem(inventoryItem);

        Debug.Log("Destroying item: " + gameObject.name);

        Destroy(gameObject);
    }
}
