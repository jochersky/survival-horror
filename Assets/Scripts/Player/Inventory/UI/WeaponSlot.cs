using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class WeaponSlot : MonoBehaviour, IDropHandler
{
    [HideInInspector] public DraggableItem item;
    
    public delegate void WeaponEquipped(DraggableItem di, WeaponSlot w);
    public event WeaponEquipped OnWeaponEquipped;

    public delegate void WeaponUnequipped(WeaponSlot w);
    public event WeaponUnequipped OnWeaponUnequipped;
    
    private void Awake()
    {
        DraggableItem[] d = GetComponentsInChildren<DraggableItem>();
        if (d.Length > 0)
        {
            item = d[0];
        }
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        if (item) return;
        
        // item dropped on the inventory slot
        GameObject dropped = eventData.pointerDrag;
        MoveToEmptySlot(dropped);
    }
    
    private void MoveToEmptySlot(GameObject dropped)
    {
        item = dropped.GetComponent<DraggableItem>();
        
        // don't allow non-weapon items to be equipped to weapon slots
        if (item.itemData.itemType != ItemType.Weapon) return;
        
        item.parentAfterDrag = transform;
        item.weaponSlot = this;
        item.inventorySlot = null;
        OnWeaponEquipped?.Invoke(item,this);
    }

    public void UnequipWeaponFromSlot()
    {
        OnWeaponUnequipped?.Invoke(this);
    }
}
