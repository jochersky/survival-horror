using System;
using System.Collections.Generic;
using UnityEditor.Scripting;
using UnityEngine;

public class Container : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject containerUI;
    [SerializeField] private int maxItems = 4;
    [SerializeField] private List<Item> items;
    private GameObject _inventoryUIContainer;
    
    private int _numItems = 0;

    public GameObject ContainerUI => containerUI;
    public int MaxItems => _numItems;

    public void AddItem(Item newItem)
    {
        // TODO: allow for swapping between containers if not working
        if (_numItems >= maxItems) return;
        items.Add(newItem);
        _numItems++;
    }

    public void RemoveItem(Item removedItem)
    {
        items.Remove(removedItem);
        _numItems--;
    }

    public void Interact()
    {
        InventoryManager.instance.AddContainer(this);
    }
}
