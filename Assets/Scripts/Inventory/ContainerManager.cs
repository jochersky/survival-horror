using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ContainerManager : MonoBehaviour
{
    private GridLayoutGroup _gridLayoutGroup;
    private Grid<GridItem> _grid;
    private int _cellSize;
    private InventorySlot[,] _slots;

    public int count = 0;
    
    private void Start() {
        _gridLayoutGroup = GetComponent<GridLayoutGroup>();
        RectTransform rectTransform = GetComponent<RectTransform>();
        
        _cellSize = Mathf.FloorToInt(_gridLayoutGroup.cellSize.x);
        int gridHeight = Mathf.FloorToInt(rectTransform.sizeDelta.y / _cellSize);
        int gridWidth = Mathf.FloorToInt(rectTransform.sizeDelta.x / _cellSize);

        _grid = new Grid<GridItem>(gridHeight, gridWidth, _cellSize, Vector3.zero, 
            CreateTGridObject);
        
        _slots = new InventorySlot[gridWidth, gridHeight];
        InventorySlot[] s = GetComponentsInChildren<InventorySlot>();
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                int slotIndex = y * gridWidth + x;
                _slots[x,y] = s[slotIndex];
                _slots[x,y].ContainerManager = this;
                _slots[x,y].Grid = _grid;
                _slots[x,y].X = x;
                _slots[x,y].Y = y;
            }
        }
    }

    private GridItem CreateTGridObject(Grid<GridItem> g, Vector2 origin, Vector2 dim, string n)
    {
        return new GridItem(g, origin, dim, n);
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
