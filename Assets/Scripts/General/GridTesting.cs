using UnityEngine;

public class GridTesting : MonoBehaviour
{
    private Grid _grid;
    private void Start()
    {
        _grid = new Grid(4, 3, 16, new Vector2(16, 16));
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(_grid.GetValue(mousePos));
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _grid.SetValue(mousePos, 21);
        }
    }
}
