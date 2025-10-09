using UnityEngine;
using UnityEngine.UI;

public class ContainerUIBuilder : MonoBehaviour
{
    private GridLayoutGroup _gridLayoutGroup;
    private Grid<GridItemObject> _grid;
    private int _cellSize;
    
    private void Start() {
        _gridLayoutGroup = GetComponent<GridLayoutGroup>();
        RectTransform rectTransform = GetComponent<RectTransform>();
        
        _cellSize = Mathf.FloorToInt(_gridLayoutGroup.cellSize.x);
        int gridHeight = Mathf.FloorToInt(rectTransform.sizeDelta.y / _cellSize);
        int gridWidth = Mathf.FloorToInt(rectTransform.sizeDelta.x / _cellSize);

        _grid = new Grid<GridItemObject>(gridHeight, gridWidth, _cellSize, Vector3.zero, (Grid<GridItemObject> g, int x, int y) => new GridItemObject(g, x, y));
    }
}

public class GridItemObject
{
    private Grid<GridItemObject> _grid;
    private int _x;
    private int _y;
    
    public GridItemObject(Grid<GridItemObject> grid, int x, int y)
    {
        _grid = grid;
        _x = x;
        _y = y;
    }
}
