using System;
using UnityEngine;

public class ContainerUI : MonoBehaviour
{
    [SerializeField] private Container container;
    private GameObject _grid;

    private void Awake()
    {
        _grid = GameObject.Find("Grid");
    }
    private void Start()
    {
        // deactivate all slots in the container that can't be used
        int numSlots = _grid.transform.childCount;
        for (int i = container.MaxItems - 1; i < numSlots; i++)
        {
            _grid.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
