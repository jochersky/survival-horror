using UnityEngine;
using UnityEngine.EventSystems;

public class HealthPack : MonoBehaviour, IConsumable, IPointerClickHandler
{
    [SerializeField] private DraggableItem dragItem;
    [SerializeField] private float healthAmt = 50;
    
    public void Use()
    {
        InventoryManager.instance.PlayerHealth.Heal(healthAmt); 
        dragItem.Count--;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right) return;
        Use();
    }
}
