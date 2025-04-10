using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; 

public class InventorySlot : MonoBehaviour, IDropHandler 
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag; 
        if (droppedObject == null) return;

        DraggableItem draggableItem = droppedObject.GetComponent<DraggableItem>();
        if (draggableItem == null) return; 

        
        if (transform.childCount == 0) 
        {
            Debug.Log($"Attempting to drop {draggableItem.item.itemName} onto empty slot {gameObject.name}");

            
            draggableItem.transform.SetParent(transform);
            
            draggableItem.transform.localPosition = Vector3.zero;

            draggableItem.successfullyDropped = true;

            
            draggableItem.originalParent = transform;

            
            CanvasGroup cg = draggableItem.GetComponent<CanvasGroup>();
            if (cg != null) cg.blocksRaycasts = true;

        }
        
        else if (transform.childCount > 0)
        {
            Debug.Log($"Attempting to swap with item in slot {gameObject.name}");
           
            DraggableItem itemInThisSlot = transform.GetChild(0).GetComponent<DraggableItem>();

            if (itemInThisSlot != null && itemInThisSlot != draggableItem) 
            {
               
                Transform originalSlot = draggableItem.originalParent;

                
                itemInThisSlot.transform.SetParent(originalSlot);
                itemInThisSlot.transform.localPosition = Vector3.zero; 
                itemInThisSlot.originalParent = originalSlot; 


                
                draggableItem.transform.SetParent(transform);
                draggableItem.transform.localPosition = Vector3.zero; 
                draggableItem.originalParent = transform; 

                
                draggableItem.successfullyDropped = true;

                
                CanvasGroup cgDragged = draggableItem.GetComponent<CanvasGroup>();
                if (cgDragged != null) cgDragged.blocksRaycasts = true;
                CanvasGroup cgExisting = itemInThisSlot.GetComponent<CanvasGroup>();
                if (cgExisting != null) cgExisting.blocksRaycasts = true;

                Debug.Log($"Swapped {draggableItem.item.itemName} with {itemInThisSlot.item.itemName}");

            }
            else
            {
                Debug.Log("Swap condition not met.");
                draggableItem.successfullyDropped = false;
            }
        }
    }
}