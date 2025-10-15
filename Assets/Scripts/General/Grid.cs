using System;
using UnityEngine;

public class Grid<TGridObject>
{
    // Event that is fired whenever grid object is changed
    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }
    
    private int _width;
    private int _height;
    private float _cellSize;
    private Vector3 _origin;
    
    public int Width => _width;
    public int Height => _height;

    private TGridObject[,] _gridArray;
    private TextMesh[,] _debugTextArray;
    
    public Grid(int height, int width, float cellSize, Vector3 origin, Func<Grid<TGridObject>, Vector2, int, int, Vector2, string, TGridObject> createTGridObject)
    {
        this._width = width;
        this._height = height;
        this._cellSize = cellSize;
        this._origin = origin;
        _gridArray = new TGridObject[width, height];
        _debugTextArray = new TextMesh[width, height];

        // initialize each object in the grid with TGridObject creation function.
        // Func<> is a delegate which returns the TGridObject when passed
        // "(Grid<ObjectName> grid, Vector2 origin, int x, int y, Vector2 dimensions, string name) => new ObjectName(...)".
        for (int x = 0; x < _gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < _gridArray.GetLength(1); y++)
            {
                _gridArray[x, y] = createTGridObject(this, new Vector2(x,y), x, y, new Vector2(1,1), "empty");
            }
        }

        // Debug line drawing and string representation
        bool debug = true;
        if (!debug) return;
        
        for (int x = 0; x < _gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < _gridArray.GetLength(1); y++)
            {
                _debugTextArray[x, y] = CreateWorldText(_gridArray[x, y]?.ToString(), null, GetWorldPosition(x, y) + new Vector3(_cellSize, _cellSize) * 0.5f, 30, Color.white, TextAnchor.MiddleCenter);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
            }
        }
        Debug.DrawLine(GetWorldPosition(_width, 0), GetWorldPosition(width, _height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(0, _height), GetWorldPosition(_width, _height), Color.white, 100f);
    }

    public TGridObject GetGridObject(int x, int y)
    {
        // ignore invalid indices
        if (x < 0 || y < 0 || x >= _width || y >= _height) return default(TGridObject);
        
        return _gridArray[x, y];
    }

    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }

    public void SetGridObject(int x, int y, TGridObject gridObject)
    {
        // ignore invalid indices
        if (x < 0 || y < 0 || x >= _width || y >= _height) return;

        _gridArray[x, y] = gridObject;
        _debugTextArray[x, y].text = _gridArray[x, y]?.ToString();
        TriggerGridObjectChanged(x, y);
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject gridObject)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetGridObject(x, y, gridObject);
    }

    // returns position of grid in world space from grid space
    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * _cellSize + _origin;
    }

    // returns position of grid in grid space from world space
    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - _origin).x / _cellSize);
        y = Mathf.FloorToInt((worldPosition - _origin).y / _cellSize);
    }
    
    // - grid object must have a line which calls this function when changing its values -
    public void TriggerGridObjectChanged(int x, int y)
    {
        if (OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, y = y });
    }

    public static TextMesh CreateWorldText(string text, Transform parent = null,
        Vector3 localPosition = default(Vector3), int fontSize = 40, Color color = default(Color), TextAnchor anchor = TextAnchor.MiddleCenter)
    {
        GameObject gameObject = new GameObject("WorldText", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = anchor;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        return textMesh;
    }
}
