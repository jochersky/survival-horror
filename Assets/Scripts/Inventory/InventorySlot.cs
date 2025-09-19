using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        // item dropped on the inventory slot
        GameObject dropped = eventData.pointerDrag;
        
        // there is already an item in this slot
        if (transform.childCount > 0)
        {
            DraggableItem droppedItem = dropped.GetComponent<DraggableItem>();
            Transform temp = droppedItem.parentAfterDrag;
            DraggableItem currentItem = transform.GetChild(0).GetComponent<DraggableItem>();
            currentItem.parentAfterDrag = temp;
            currentItem.transform.SetParent(temp);
            droppedItem.parentAfterDrag = transform;
        }
        else
        {
            DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
            draggableItem.parentAfterDrag = transform;
        }
    }
}
