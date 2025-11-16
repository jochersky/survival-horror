using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ContainerManager : MonoBehaviour
{
    private GridLayoutGroup _gridLayoutGroup;
    private Grid<GridItem> _grid;
    private int _cellSize;
    private InventorySlot[,] _slots;

    private int _gridHeight;
    private int _gridWidth;
    private InventorySlot[] _s;
    
    public int CellSize => _cellSize;
    
    public delegate void StartFinished();
    public event StartFinished OnStartFinished;

    public delegate void StackableItemCountsUpdated(string itemName);
    public event StackableItemCountsUpdated OnStackableItemCountsUpdated;
    
    private void Start() {
        _gridLayoutGroup = GetComponent<GridLayoutGroup>();
        RectTransform rectTransform = GetComponent<RectTransform>();
        
        _cellSize = Mathf.FloorToInt(_gridLayoutGroup.cellSize.x);
        _gridHeight = Mathf.FloorToInt(rectTransform.sizeDelta.y / _cellSize);
        _gridWidth = Mathf.FloorToInt(rectTransform.sizeDelta.x / _cellSize);
        
        _grid = new Grid<GridItem>(_gridHeight, _gridWidth, _cellSize, Vector3.zero, CreateTGridObject);
        
        _slots = new InventorySlot[_gridWidth, _gridHeight];
        _s = GetComponentsInChildren<InventorySlot>();
        List<Tuple<DraggableItem, Vector2>> draggableLocations = new List<Tuple<DraggableItem, Vector2>>();
        for (int y = 0; y < _gridHeight; y++)
        {
            for (int x = 0; x < _gridWidth; x++)
            {
                int slotIndex = y * _gridWidth + x;
                _slots[x,y] = _s[slotIndex];
                if (_s[slotIndex].item)
                {
                    Tuple<DraggableItem, Vector2> i =
                        new Tuple<DraggableItem, Vector2>(_s[slotIndex].item, new Vector2(x, y));
                    draggableLocations.Add(i);
                }
                _slots[x,y].ContainerManager = this;
                _slots[x,y].Grid = _grid;
                _slots[x,y].X = x;
                _slots[x,y].Y = y;
            }
        }

        // Add all parented game objects with DraggableItem on them to their InventorySlots.
        // (mimics the way DraggableItems and InventorySlots interact)
        foreach (Tuple<DraggableItem, Vector2> t in draggableLocations)
        {
            DraggableItem di = t.Item1;
            int x = (int) t.Item2.x;
            int y = (int) t.Item2.y;
            SetDraggableItemToGrid(di, x, y);
        }
        
        OnStartFinished?.Invoke();
    }

    private GridItem CreateTGridObject(Grid<GridItem> g, Vector2 origin, Vector2 dim, string n)
    {
        return new GridItem(g, origin, dim, n);
    }

    public bool FindSpaceForItem(GameObject dragItemPrefab, int count)
    {
        if (dragItemPrefab.TryGetComponent<DraggableItem>(out DraggableItem dragItem) && dragItem.itemData)
        {
            bool stackable = dragItem.itemData.maxCount > 1;
            
            // Thrown weapon re-equip handling
            bool matchingWeapon = dragItem.itemData.itemName == WeaponManager.instance.lastThrownWeaponName;
            if (matchingWeapon && WeaponManager.instance.EquipThrownWeaponOnPickup(dragItemPrefab))
            {
                return true;
            }

            // stackable and non-stackable item handling
            if (!stackable) return AddNonStackableItem(dragItem, dragItemPrefab);
            else return AddStackableItem(dragItem, dragItemPrefab, count);
        }
        
        return false;
    }

    private bool AddStackableItem(DraggableItem dragItem, GameObject dragItemPrefab, int count)
    {
        Vector2 itemDim = dragItem.itemData.gridItemDimensions;
        float yRange = _gridHeight - itemDim.y + 1;
        float xRange = _gridWidth - itemDim.x + 1;
        String itemName = dragItem.itemData.itemName;
        int maxCount = dragItem.itemData.maxCount;
        
        // Find all matching stackable draggable items in inventory slots
        List<DraggableItem> draggableItems = new List<DraggableItem>();
        for (int y = 0; y < _gridHeight; y++)
        {
            for (int x = 0; x < _gridWidth; x++)
            {
                DraggableItem di = _slots[x, y].item;
                if (!di) continue;
                
                String diName = di.itemData.itemName;
                int c = di.Count;
                
                // Only add matching items which have a count lower than the max
                if (diName == itemName && c < maxCount) draggableItems.Add(di);
            }
        }
        
        // go through all items found
        int countLeft = count;
        if (draggableItems.Count > 0)
        {
            foreach (DraggableItem di in draggableItems)
            {
                // stop if the count ever reaches 0
                if (countLeft == 0) break;
                
                int newCount = di.Count + countLeft;
                if (newCount > maxCount)
                {
                    countLeft = newCount - maxCount;
                    di.Count = maxCount;
                }
                else
                {
                    countLeft = 0;
                    di.Count = newCount;
                }
            }
        }
        
        // look for an empty spot to add the item
        if (countLeft > 0)
        {
            for (int y = 0; y < yRange; y++)
            {
                for (int x = 0; x < xRange; x++)
                {
                    GridItem gridObj = _grid.GetGridObject(x, y);
                
                    // check whether the item can be placed here
                    if (gridObj.Name == "empty" && DroppedItemPlaceable(x, y, itemDim.x, itemDim.y))
                    {
                        GameObject inst = Instantiate(dragItemPrefab, _slots[x, y].gameObject.transform);
                        DraggableItem di = inst.GetComponent<DraggableItem>();
                        di.Count = countLeft;
                        SetDraggableItemToGrid(di, x, y);
                        _slots[x, y].item = di;
                        OnStackableItemCountsUpdated?.Invoke(itemName);
                        return true;
                    }
                }
            }
            
            // TODO: no empty place was found, place an overflow amount in the world
        }
        // item successfully stacked into existing stacks
        else
        {
            OnStackableItemCountsUpdated?.Invoke(itemName);
            return true;
        }
        
        return false;
    }

    private bool AddNonStackableItem(DraggableItem dragItem, GameObject dragItemPrefab)
    {
        Vector2 itemDim = dragItem.itemData.gridItemDimensions;
        float yRange = _gridHeight - itemDim.y + 1;
        float xRange = _gridWidth - itemDim.x + 1;
        
        for (int y = 0; y < yRange; y++)
        {
            for (int x = 0; x < xRange; x++)
            {
                GridItem gridObj = _grid.GetGridObject(x, y);
                
                // check whether the item can be placed here
                if (gridObj.Name == "empty" && DroppedItemPlaceable(x, y, itemDim.x, itemDim.y))
                {
                    GameObject inst = Instantiate(dragItemPrefab, _slots[x, y].gameObject.transform);
                    DraggableItem di = inst.GetComponent<DraggableItem>();
                    SetDraggableItemToGrid(di, x, y);
                    _slots[x, y].item = di;
                    return true;
                }
            }
        }
        
        return false;
    }

    private bool DroppedItemPlaceable(int x, int y, float itemX, float itemY)
    {
        // check surrounding slots
        for (int i = 0; i < itemY; i++)
        {
            for (int j = 0; j < itemX; j++)
            {
                // item can't be placed here
                if (_grid.GetGridObject(x + j, y + i).Name != "empty") return false;
            }
        }
        // item can be placed here
        return true;
    }

    public bool SetItem(int x, int y, GridItem item)
    {
        // validate that the item can be placed here
        if (!ValidateItemPlacement(x, y, item)) return false;
        
        // set item to the grid cells within the item dimensions
        for (int i = 0; i < item.Dimensions.x; i++)
        {
            for (int j = 0; j < item.Dimensions.y; j++)
            {
                _grid.SetGridObject(x + i, y + j, item);
                _slots[x + i, y + j].ToggleImage(item.Name == "empty");
            }
        }
        return true;
    }
    
    private bool ValidateItemPlacement(int x, int y, GridItem item)
    {
        // check the item can be placed here 
        for (int i = 0; i < item.Dimensions.x; i++)
        {
            for (int j = 0; j < item.Dimensions.y; j++)
            {
                int dx = x + i;
                int dy = y + j;
                // out of bounds check
                if (dx >= _grid.Width || dx < 0 || dy >= _grid.Height || dy < 0) return false;
                // occupied space check
                if (item.Name == "empty") continue;
                GridItem temp = _grid.GetGridObject(dx, dy);
                if (temp.Name != "empty") return false;
            }
        }
        return true;
    }

    private void SetDraggableItemToGrid(DraggableItem dragItem, int x, int y)
    {
        GridItem item = dragItem.CreateGridItem();
        SetItem(x, y, item);
        dragItem.parentAfterDrag = _slots[x, y].transform;
        dragItem.inventorySlot = _slots[x, y];
        dragItem.containerManager = this;
    }

    public List<DraggableItem> GetAllDraggableItems(string itemName)
    {
        // Find all matching stackable draggable items in inventory slots
        List<DraggableItem> draggableItems = new List<DraggableItem>();
        for (int y = 0; y < _gridHeight; y++)
        {
            for (int x = 0; x < _gridWidth; x++)
            {
                DraggableItem di = _slots[x, y].item;
                if (!di) continue;
                
                String diName = di.itemData.itemName;
                int c = di.Count;
                
                if (diName == itemName) draggableItems.Add(di);
            }
        }
        
        return draggableItems;
    }

    public void DropItemWithName(string itemName)
    {
        OnStackableItemCountsUpdated?.Invoke(itemName);
    }
}
