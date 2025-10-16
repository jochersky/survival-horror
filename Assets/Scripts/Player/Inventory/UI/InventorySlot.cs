using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private Image image;

    private ContainerManager _containerManager;
    private Grid<GridItem> _grid;
    private int _x;
    private int _y;

    private GridItem _emptyItem;
    
    public ContainerManager ContainerManager { get => _containerManager; set => _containerManager = value; }
    public Grid<GridItem> Grid { get => _grid; set => _grid = value; }
    public int X { get => _x; set => _x = value; }
    public int Y { get => _y; set => _y = value; }

    private void Start()
    {
        _emptyItem = new GridItem(_grid, new Vector2(_x, _y), new Vector2(2, 2), "empty");
    }

    public void OnDrop(PointerEventData eventData)
    {
        // item dropped on the inventory slot
        GameObject dropped = eventData.pointerDrag;

        MoveToEmptySlot(dropped);

        // there is already an item in this slot
        // if (transform.childCount > 0)
        // {
        //     DraggableItem droppedItem = dropped.GetComponent<DraggableItem>();
        //     DraggableItem currentItem = transform.GetChild(0).GetComponent<DraggableItem>();
        //     SwapItems(droppedItem, currentItem);
        //     // if (!CombineItemStacks(droppedItem, currentItem, dropped))
        //     // {
        //     //     SwapItems(droppedItem, currentItem);
        //     // }
        // }
        // // empty inventory slot
        // else
        // {
        //     MoveToEmptySlot(dropped);
        // }
    }

    private bool CombineItemStacks(DraggableItem droppedItem, DraggableItem currentItem, GameObject dropped)
    {
        // don't stack items witnout names
        // if (currentItem.ItemName == "") return false;
        
        // don't attempt to stack items with different names
        // if (currentItem.ItemName != droppedItem.ItemName) return false;
        
        // int newCurrentItemCount = currentItem.ItemCount + droppedItem.ItemCount;
        // return false to swap stacks instead of combining
        // if (newCurrentItemCount > currentItem.MaxItemCount) return false;
        
        // stack combined
        // currentItem.ItemCount = newCurrentItemCount;
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
        draggableItem.containerManager = ContainerManager;
        draggableItem.inventorySlot = this;
    }

    public void ToggleImage(bool visible)
    {
        image.enabled = visible;
    }
}


