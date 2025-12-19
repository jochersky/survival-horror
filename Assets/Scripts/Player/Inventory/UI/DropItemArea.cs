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
            AudioManager.Instance.PlaySFX(SfxType.DragItemDropped, null, 1, Random.Range(0.75f, 1.25f));
        }
    }
}
