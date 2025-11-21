using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private Image image;
    
    [HideInInspector] public DraggableItem item;

    private ContainerManager _containerManager;
    private Grid<GridItem> _grid;
    private int _x;
    private int _y;
    
    public ContainerManager ContainerManager { get => _containerManager; set => _containerManager = value; }
    public Grid<GridItem> Grid { get => _grid; set => _grid = value; }
    public int X { get => _x; set => _x = value; }
    public int Y { get => _y; set => _y = value; }

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
        if (eventData.pointerDrag.TryGetComponent<DraggableItem>(out DraggableItem droppedItem))
        {
            // item already occupies this inventory slot
            if (item)
            {
                string curItemName = item.itemData.itemName;
                string dropItemName = droppedItem.itemData.itemName;
                int curItemCount = item.Count;
                int dropItemCount = droppedItem.Count;
                
                // combine the two stacks
                if (dropItemName == curItemName)
                {
                    int maxCount = item.itemData.maxCount;
                    int amt = curItemCount + dropItemCount;
                    if (amt <= maxCount)
                    {
                        item.Count += dropItemCount;
                        droppedItem.Count = 0;
                    }
                    else
                    {
                        int amtAdded = item.itemData.maxCount - item.Count;
                        item.Count = maxCount;
                        droppedItem.Count -= amtAdded;
                    }
                }
                // TODO: add item swapping between items of the same size but different names
            }
            // nothing in this slot, assign item to this inventory slot
            else
            {
                droppedItem.parentAfterDrag = transform;
                droppedItem.containerManager = ContainerManager;
                droppedItem.inventorySlot = this;
            }
        }
    }

    public void ToggleImage(bool visible)
    {
        image.enabled = visible;
    }
}
