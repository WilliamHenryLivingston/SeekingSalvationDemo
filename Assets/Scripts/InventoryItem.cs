using System.Collections;
using System.Collections.Generic;
using UnityEngine;



    [CreateAssetMenu(fileName = "NewInventoryItem", menuName = "Inventory/Item")] 

    public class InventoryItem: ScriptableObject
    {
    public string itemName = "New Item";
    public Sprite itemIcon = null;
    public GameObject itemPrefab = null;
    }

