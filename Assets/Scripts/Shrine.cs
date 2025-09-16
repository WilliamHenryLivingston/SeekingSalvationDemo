//Copyright 2025 William Livingston 
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShrineTable : MonoBehaviour
{
    [Header("Shrine Settings")]
    public List<Transform> tableSlots = new List<Transform>();
    public float interactRange = 3f;

    [Header("UI")]
    public TextMeshProUGUI shrineUIText;

    private PlayerInventory playerInventory;
    private Camera playerCam;

    private List<InventoryItem> placedItems = new List<InventoryItem>();
    private List<GameObject> placedObjects = new List<GameObject>();
    private int totalSpiritualSignificance = 0;

    void Start()
    {
        playerCam = Camera.main;
        playerInventory = FindObjectOfType<PlayerInventory>();

        if (playerInventory == null)
            Debug.LogError("No PlayerInventory found in scene!");

        UpdateShrineUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPlaceItem();
        }
    }

    public void TryPlaceItem()
    {
        if (playerInventory == null || playerCam == null) return;

        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            if (hit.collider.gameObject != this.gameObject) return;

            InventoryItem firstItem = playerInventory.GetFirstItem();
            if (firstItem == null)
            {
                Debug.Log("No items to place on the shrine.");
                return;
            }

            Transform freeSlot = tableSlots.Find(slot => slot.childCount == 0);
            if (freeSlot == null)
            {
                Debug.Log("No free slots on the shrine.");
                return;
            }

            // Spawn the visual representation
            GameObject placedObj = firstItem.itemPrefab != null
                ? Instantiate(firstItem.itemPrefab, freeSlot.position, freeSlot.rotation, freeSlot)
                : GameObject.CreatePrimitive(PrimitiveType.Cube);

            placedObj.transform.SetParent(freeSlot);
            placedObj.transform.localPosition = Vector3.zero;
            placedObj.transform.localRotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
            placedObj.transform.localScale = Vector3.one * 0.3f;

            placedItems.Add(firstItem);
            placedObjects.Add(placedObj);

            totalSpiritualSignificance += firstItem.spiritualSignificance;

            // Remove from inventory
            playerInventory.RemoveItem(firstItem);

            UpdateShrineUI();

            Debug.Log($"Placed {firstItem.itemName} on shrine. Total Spiritual Value: {totalSpiritualSignificance}");
        }
    }

    public int OfferItems()
    {
        if (placedItems.Count == 0) return 0;

        int offeredEnergy = totalSpiritualSignificance;

        foreach (GameObject obj in placedObjects)
            Destroy(obj);

        placedItems.Clear();
        placedObjects.Clear();
        totalSpiritualSignificance = 0;

        UpdateShrineUI();

        Debug.Log($"Offering {offeredEnergy} spiritual energy to the tree!");
        return offeredEnergy;
    }

    private void UpdateShrineUI()
    {
        if (shrineUIText != null)
            shrineUIText.text = $"Shrine Energy: {totalSpiritualSignificance}";
    }
}