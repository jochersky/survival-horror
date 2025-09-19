using System;
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
            DraggableItem currentItem = transform.GetChild(0).GetComponent<DraggableItem>();
            if (!CombineItemStacks(droppedItem, currentItem, dropped))
            {
                SwapItems(droppedItem, currentItem);
            }
        }
        // empty inventory slot
        else
        {
            MoveToEmptySlot(dropped);
        }
    }

    private bool CombineItemStacks(DraggableItem droppedItem, DraggableItem currentItem, GameObject dropped)
    {
        // don't attempt to stack items with different names
        if (currentItem.ItemName != droppedItem.ItemName) return false;
        
        int newCurrentItemCount = currentItem.ItemCount + droppedItem.ItemCount;
        // return false to swap stacks instead of combining
        if (newCurrentItemCount > currentItem.MaxItemCount) return false;
        
        // stack combined
        currentItem.ItemCount = newCurrentItemCount;
        Destroy(dropped);
        return true;
    }
    
    private void SwapItems(DraggableItem droppedItem, DraggableItem currentItem)
    {
        Transform temp = droppedItem.parentAfterDrag;
        currentItem.parentAfterDrag = temp;
        currentItem.transform.SetParent(temp);
        droppedItem.parentAfterDrag = transform;
    }

    private void MoveToEmptySlot(GameObject dropped)
    {
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
        draggableItem.parentAfterDrag = transform;
    }
}


