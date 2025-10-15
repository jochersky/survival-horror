using UnityEngine;

public class GridItem
{
    private Grid<GridItem> _grid;
    private Vector2 _origin;
    private int _x;
    private int _y;
    private Vector2 _dimensions;
    private string _name;
    
    public Grid<GridItem> Grid { get { return _grid; } }
    public Vector2 Origin { get { return _origin; } }
    public int OriginX { get { return (int)Origin.x; } }
    public int OriginY { get { return (int)Origin.y; } }
    public int X { get { return _x; } }
    public int Y { get { return _y; } }
    public Vector2 Dimensions { get { return _dimensions; } }
    public string Name { get { return _name; } }
    
    public GridItem(Grid<GridItem> grid, Vector2 origin, int x, int y, Vector2 dimensions, string name)
    {
        _grid = grid;
        _origin = origin;
        _x = x;
        _y = y;
        _dimensions = dimensions;
        _name = name;
    }

    public override string ToString()
    {
        return Name;
    }
}
