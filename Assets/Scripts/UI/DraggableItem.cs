//Copyright 2025 William Livingston
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool enableDragging = true;
    [HideInInspector] public InventoryItem item;
    [HideInInspector] public Image image;
    [HideInInspector] public Transform originalParent;
    [HideInInspector] public RectTransform inventoryPanelRect;

    private CanvasGroup canvasGroup;
    private Canvas parentCanvas;
    private Vector3 originalLocalPosition; // Store original position within the slot

    [HideInInspector] public bool successfullyDropped = false;

    void Awake()
    {
        image = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!enableDragging) return;
        if (item == null || image == null || image.sprite == null || !image.enabled)
        {
            eventData.pointerDrag = null;
            return;
        }

        Debug.Log("Begin Drag: " + item.itemName);

        originalParent = transform.parent;
        originalLocalPosition = transform.localPosition;

        transform.SetParent(parentCanvas.transform, true);
        transform.SetAsLastSibling();

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        successfullyDropped = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!enableDragging) return;
        if (canvasGroup.blocksRaycasts) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            eventData.position,
            parentCanvas.worldCamera,
            out Vector2 localPoint
        );
        transform.localPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!enableDragging) return;
        if (item == null) return;

        Debug.Log("End Drag: " + item.itemName);

        if (successfullyDropped)
        {
            Debug.Log($"{item.itemName} successfully dropped onto a new slot.");
            canvasGroup.alpha = 1.0f;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            bool outsideInventory = inventoryPanelRect == null || !RectTransformUtility.RectangleContainsScreenPoint(
                inventoryPanelRect, eventData.position, parentCanvas.worldCamera);

            if (outsideInventory)
            {
                ReturnToOriginalSlot();
            }
            else
            {
                Debug.Log("Dropped inside inventory panel but not on a valid slot, returning item.");
                ReturnToOriginalSlot();
            }
        }

        if (this != null && gameObject != null)
        {
            canvasGroup.alpha = 1.0f;
            canvasGroup.blocksRaycasts = true;
        }
    }

    private void ReturnToOriginalSlot()
    {
        if (originalParent != null)
        {
            transform.SetParent(originalParent);
            transform.localPosition = originalLocalPosition;
        }
        else
        {
            Debug.LogWarning($"Original parent for {item.itemName} is missing. Destroying UI element.");
            Destroy(gameObject);
        }

        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
    }

    private bool DropItemToWorld(InventoryItem itemToDrop)
    {
        if (itemToDrop == null || itemToDrop.itemPrefab == null) return false;

        bool removed = PlayerInventory.Instance.inventoryItems.Remove(itemToDrop);
        if (!removed)
        {
            Debug.LogError($"Failed to remove '{itemToDrop.itemName}' from inventory.");
            return false;
        }

        Debug.Log($"Dropping '{itemToDrop.itemName}' into world.");

        Vector3 dropPosition = PlayerInventory.Instance.transform.position +
                               (PlayerInventory.Instance.transform.forward * 1.5f) +
                               (Vector3.up * 0.2f); // Initial small offset

        float checkRadius = 0.5f;
        int maxAttempts = 10;
        int attempts = 0;

        // Prevent overlapping with other dropped items
        while (Physics.OverlapSphere(dropPosition, checkRadius, LayerMask.GetMask("DroppedItem")).Length > 0 && attempts < maxAttempts)
        {
            dropPosition += new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
            attempts++;
        }

        // Perform a raycast downward to ensure the item lands on the ground
        if (Physics.Raycast(dropPosition + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 5f))
        {
            dropPosition = hit.point + Vector3.up * 0.1f; // Ensure it's above the surface
        }

        GameObject droppedItem = Instantiate(itemToDrop.itemPrefab, dropPosition, Quaternion.identity);
        droppedItem.layer = LayerMask.NameToLayer("DroppedItem");

        Debug.Log($"Dropped '{itemToDrop.itemName}' at {dropPosition}.");

        return true;
    }
}