using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static InventoryItem;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;
    public List<InventoryItem> inventoryItems = new List<InventoryItem>();

    public Transform inventoryUIParent;
    public GameObject inventorySlotPrefab;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void AddItem(InventoryItem item)
    {
        if(item == null) return;    


        inventoryItems.Add(item);
        Debug.Log(item.itemName + " added to inventory.");

        //UpdateUI
        AddItemToUI(item);
    }

    private void AddItemToUI(InventoryItem item)
    {
        GameObject newSlot = Instantiate(inventorySlotPrefab, inventoryUIParent);
        Image itemImage = newSlot.GetComponent<Image>();

        if (itemImage != null )
        {
            itemImage.sprite = item.itemIcon;
        }
       
        DraggableItem draggableItem = newSlot.GetComponent<DraggableItem>();
        if (draggableItem == null)
        {
            draggableItem = newSlot.AddComponent<DraggableItem>(); // Add Drag functionality
        }

        // Link the Image to the Drag script
        draggableItem.image = itemImage;

    }
}