using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class WeaponSlot : MonoBehaviour, IDropHandler
{
    [HideInInspector] public DraggableItem item;
    
    public void OnDrop(PointerEventData eventData)
    {
        // item dropped on the inventory slot
        GameObject dropped = eventData.pointerDrag;
        MoveToEmptySlot(dropped);
    }
    
    private void MoveToEmptySlot(GameObject dropped)
    {
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
        draggableItem.parentAfterDrag = transform;
        draggableItem.inventorySlot = null;
        
        RectTransform rt = dropped.GetComponent<RectTransform>();
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition= Vector2.zero;
    }
}
