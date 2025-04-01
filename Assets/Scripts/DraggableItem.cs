using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public InventoryItem item;
    [HideInInspector] public Image image;
    [HideInInspector] public Transform originalParent;
    [HideInInspector] public RectTransform inventoryPanelRect;

    private CanvasGroup canvasGroup;
    private Canvas parentCanvas;
    private Vector3 originalLocalPosition; // Store original position within the slot

    // Flag to track if the item was successfully dropped onto a valid slot
    [HideInInspector] public bool successfullyDropped = false;

    void Awake()
    {
        image = GetComponent<Image>();
        // Ensure CanvasGroup exists or add it
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        parentCanvas = GetComponentInParent<Canvas>();
        // We'll set originalParent when adding the item in PlayerInventory
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item == null || image == null || image.sprite == null || !image.enabled)
        {
            eventData.pointerDrag = null; // Cancel drag for empty/invalid slots
            return;
        }

        Debug.Log("Begin Drag: " + item.itemName);

        // Store original state *before* reparenting
        originalParent = transform.parent;
        originalLocalPosition = transform.localPosition; // Store position relative to original slot

        // --- Prepare for drag ---
        transform.SetParent(parentCanvas.transform, true); // Move to top level of canvas
        transform.SetAsLastSibling(); // Render on top

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false; // Allow raycasts to hit things underneath (like other slots)

        successfullyDropped = false; // Reset flag for this drag operation
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvasGroup.blocksRaycasts) return; // Drag was cancelled or item is invalid

        // Move the item with the mouse using Canvas coordinates
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
        if (item == null) return; // Do nothing if item is somehow null

        Debug.Log("End Drag: " + item.itemName);

        // Crucial: Restore blockRaycasts *before* doing drop checks if needed elsewhere,
        // but often better to leave it false until we know the final destination.
        // canvasGroup.blocksRaycasts = true; // Re-enable raycasting

        // Check if OnDrop on a valid target (like InventorySlot) already handled the drop
        if (successfullyDropped)
        {
            Debug.Log($"{item.itemName} successfully dropped onto a new slot.");
            // The OnDrop handler should have already set the parent and position.
            // We just need to reset the visual alpha.
            canvasGroup.alpha = 1.0f;
            // The parent was set by the slot, so raycasts should be fine now.
            canvasGroup.blocksRaycasts = true;
            // We don't need to do anything else here, the InventorySlot handled it.
        }
        else // Item was NOT dropped onto a valid slot via OnDrop
        {
            // Check if it was dropped outside the main inventory panel
            bool outsideInventory = inventoryPanelRect == null || !RectTransformUtility.RectangleContainsScreenPoint(
                                        inventoryPanelRect,
                                        eventData.position,
                                        parentCanvas.worldCamera
                                    );

            if (outsideInventory)
            {
                Debug.Log("Dropped outside inventory!");
                // Attempt to drop the item into the world
                bool droppedToWorld = PlayerInventory.instance.DropItemToWorld(item);

                if (droppedToWorld)
                {
                    // Item was successfully dropped and removed from inventory logic,
                    // so destroy this UI element entirely.
                    Destroy(gameObject);
                    // No need to reset alpha/raycasts on a destroyed object.
                }
                else
                {
                    // Drop failed (e.g., no prefab), return UI element to original slot
                    Debug.Log("World drop failed or not possible, returning item.");
                    ReturnToOriginalSlot();
                }
            }
            else
            {
                // Dropped inside the inventory panel bounds, but not onto a valid receiving slot
                Debug.Log("Dropped inside inventory panel but not on a valid slot, returning item.");
                ReturnToOriginalSlot();
            }
        }

        // Ensure these are always reset if the object wasn't destroyed
        if (this != null && gameObject != null) // Check if object still exists
        {
            canvasGroup.alpha = 1.0f;
            canvasGroup.blocksRaycasts = true; // Make sure it can be interacted with again
        }
    }

    private void ReturnToOriginalSlot()
    {
        // Make sure original parent still exists (scene changes etc.)
        if (originalParent != null)
        {
            transform.SetParent(originalParent);
            transform.localPosition = originalLocalPosition; // Restore position within the original slot
        }
        else
        {
            // If original parent is gone, we might have to destroy item or handle differently
            Debug.LogWarning($"Original parent for {item.itemName} is missing. Destroying UI element.");
            Destroy(gameObject);
        }

        // Reset visual state fully here as well
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
    }
}