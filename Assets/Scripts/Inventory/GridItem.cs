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
    public Vector2 Dimensions { get { return _dimensions; } }
    public string Name { get { return _name; } }
    
    public GridItem(Grid<GridItem> grid, Vector2 origin, Vector2 dimensions, string name)
    {
        _grid = grid;
        _origin = origin;
        _dimensions = dimensions;
        _name = name;
    }

    public override string ToString()
    {
        return Name;
    }
}
