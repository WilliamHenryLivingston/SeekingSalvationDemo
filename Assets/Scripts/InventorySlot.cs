using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // Add this if you need Image/UI components from the slot itself

public class InventorySlot : MonoBehaviour, IDropHandler // This script goes on the SLOT prefab
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag; // The GameObject being dragged (which has DraggableItem)
        if (droppedObject == null) return;

        DraggableItem draggableItem = droppedObject.GetComponent<DraggableItem>();
        if (draggableItem == null) return; // Only accept DraggableItems

        // --- Logic for dropping onto an EMPTY slot ---
        if (transform.childCount == 0) // Is this slot empty?
        {
            Debug.Log($"Attempting to drop {draggableItem.item.itemName} onto empty slot {gameObject.name}");

            // Parent the item to this slot's transform
            draggableItem.transform.SetParent(transform);
            // Reset local position to center it in the slot
            draggableItem.transform.localPosition = Vector3.zero;

            // IMPORTANT: Tell the DraggableItem that the drop was successful
            draggableItem.successfullyDropped = true;

            // Optional: Update the originalParent reference in case it's dragged again from this new slot
            draggableItem.originalParent = transform;

            // Re-enable raycasts on the item now that it's settled
            CanvasGroup cg = draggableItem.GetComponent<CanvasGroup>();
            if (cg != null) cg.blocksRaycasts = true;

        }
        // --- Logic for SWAPPING items (Optional) ---
        else if (transform.childCount > 0)
        {
            Debug.Log($"Attempting to swap with item in slot {gameObject.name}");
            // Get the item currently in this slot
            DraggableItem itemInThisSlot = transform.GetChild(0).GetComponent<DraggableItem>();

            if (itemInThisSlot != null && itemInThisSlot != draggableItem) // Ensure there is an item and it's not the one being dragged
            {
                // Get the original slot of the item being dragged
                Transform originalSlot = draggableItem.originalParent;

                // Move the item currently in this slot to the original slot of the dragged item
                itemInThisSlot.transform.SetParent(originalSlot);
                itemInThisSlot.transform.localPosition = Vector3.zero; // Center in the original slot
                itemInThisSlot.originalParent = originalSlot; // Update its original parent ref


                // Move the dragged item into this slot
                draggableItem.transform.SetParent(transform);
                draggableItem.transform.localPosition = Vector3.zero; // Center in this slot
                draggableItem.originalParent = transform; // Update its original parent ref

                // Mark the drop as successful for the dragged item
                draggableItem.successfullyDropped = true;

                // Re-enable raycasts on both items
                CanvasGroup cgDragged = draggableItem.GetComponent<CanvasGroup>();
                if (cgDragged != null) cgDragged.blocksRaycasts = true;
                CanvasGroup cgExisting = itemInThisSlot.GetComponent<CanvasGroup>();
                if (cgExisting != null) cgExisting.blocksRaycasts = true;

                Debug.Log($"Swapped {draggableItem.item.itemName} with {itemInThisSlot.item.itemName}");

            }
            else
            {
                // Cannot swap (e.g., trying to drop onto itself or child isn't a DraggableItem)
                // Let OnEndDrag handle returning the item to original slot
                Debug.Log("Swap condition not met.");
                draggableItem.successfullyDropped = false;
            }
        }
    }
}