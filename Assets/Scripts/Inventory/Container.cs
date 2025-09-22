using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour, IInteractable
{
    [SerializeField] private int maxItems = 4;
    [SerializeField] private List<Item> items;
    
    private int _numItems = 0;

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
        Debug.Log("Opening container");
    }
}
