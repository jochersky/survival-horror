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
        // item dropped on the inventory slot
        GameObject dropped = eventData.pointerDrag;
        MoveToEmptySlot(dropped);
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


