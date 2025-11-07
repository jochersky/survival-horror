using UnityEngine;
using UnityEngine.EventSystems;

public class DropItemArea : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        
        GameObject dropped = eventData.pointerDrag;
        if (dropped.TryGetComponent(out DraggableItem item))
        {
            item.DropItem();
        }
    }
}
