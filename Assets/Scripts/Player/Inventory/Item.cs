using System;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform followTransform;
    [SerializeField] private ItemData itemData;
    private Container _container;

    public void FixedUpdate()
    {
        if (followTransform)
        {
            transform.position = followTransform.position;
            transform.rotation = followTransform.rotation;
        }
    }

    public void ChangeContainer(Container newContainer)
    {
        if (_container) _container.RemoveItem(this);
        _container = newContainer;
        _container.AddItem(this);
    }

    public void DropIntoWorld()
    {
        _container.RemoveItem(this);
        // TODO: spawn the prefab into the game
    }

    public void Interact()
    {
        // InventoryManager.instance.PlayerInventoryContainer.AddItem(this);
        
        // TODO: determine if there is an open spot in the inventory for the item and put it there.
        // if not, tell player to make room and give them the dimensions
        Destroy(transform.parent.gameObject);
    }
}
