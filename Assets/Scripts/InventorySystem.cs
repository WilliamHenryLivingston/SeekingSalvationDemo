using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InventoryItem;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;
    public List<InventoryItem> inventoryItems = new List<InventoryItem>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void AddItem(InventoryItem item)
    {
        inventoryItems.Add(item);
        Debug.Log(item.itemName + " added to inventory.");
    }
}