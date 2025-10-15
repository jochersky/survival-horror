using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContainerManager : MonoBehaviour
{
    private GridLayoutGroup _gridLayoutGroup;
    private Grid<GridItem> _grid;
    private int _cellSize;
    private InventorySlot[,] _slots;
    
    private void Start() {
        _gridLayoutGroup = GetComponent<GridLayoutGroup>();
        RectTransform rectTransform = GetComponent<RectTransform>();
        
        _cellSize = Mathf.FloorToInt(_gridLayoutGroup.cellSize.x);
        int gridHeight = Mathf.FloorToInt(rectTransform.sizeDelta.y / _cellSize);
        int gridWidth = Mathf.FloorToInt(rectTransform.sizeDelta.x / _cellSize);

        _grid = new Grid<GridItem>(gridHeight, gridWidth, _cellSize, Vector3.zero, 
            CreateTGridObject);
        
        GridItem gun = new GridItem(_grid, new Vector2(1,1), 0, 0, new Vector2(2,2), "gun");
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                _grid.SetGridObject(1 + i, 1 + j, gun);
            }
        }
        
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
                // _slots[slotIndex].Grid = _grid;
                // _slots[slotIndex].X = x;
                // _slots[slotIndex].Y = y;
            }
        }
    }

    private GridItem CreateTGridObject(Grid<GridItem> g, Vector2 origin, int x, int y, Vector2 dim, string n)
    {
        return new GridItem(g, origin, x, y, dim, n);
    }

    public void SetInventorySlotItem(int x, int y, GridItem item)
    {
        // TODO: 
        _slots[x, y].Image.enabled = false;
    }
}
