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
            GridItem item = t.Item1.CreateGridItem();
            SetItem((int)t.Item2.x, (int)t.Item2.y, item);
            di.parentAfterDrag = _slots[(int)t.Item2.x, (int)t.Item2.y].transform;
            di.inventorySlot = _slots[(int)t.Item2.x, (int)t.Item2.y];
            di.containerManager = this;
        }
        
        OnStartFinished?.Invoke();
    }

    private GridItem CreateTGridObject(Grid<GridItem> g, Vector2 origin, Vector2 dim, string n)
    {
        return new GridItem(g, origin, dim, n);
    }

    public bool FindSpaceForItem(GameObject dragItemPrefab)
    {
        if (dragItemPrefab.TryGetComponent<DraggableItem>(out DraggableItem dragItem))
        {
            if (dragItem.itemData)
            { 
                Vector2 itemDim = dragItem.itemData.gridItemDimensions;
                float yRange = _gridHeight - itemDim.y;
                float xRange = _gridWidth - itemDim.x;
                Debug.Log(_gridHeight + " " + _gridWidth);
                for (int y = 0; y < yRange; y++)
                {
                    for (int x = 0; x < xRange; x++)
                    {
                        bool open = false;
                        // found an empty slot
                        if (!_slots[x,y].item)
                        {
                            open = true;
                            // check surrounding slots
                            for (int i = 0; i < itemDim.y; i++)
                            {
                                for (int j = 0; j < itemDim.x; j++)
                                {
                                    if (_slots[x + j, y + j].item) open = false;
                                    if (!open) break;
                                }
                                if (!open) break;
                            }
                        }

                        // found a place for the item to go
                        if (open)
                        {
                            Debug.Log(x + " " + y);
                            GameObject inst = Instantiate(dragItemPrefab, _slots[x,y].gameObject.transform);
                            return true;
                        }
                    }
                }
            }
        }
        
        return false;
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
}
