using System;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform followTransform;
    [SerializeField] private GameObject draggableItemPrefab;
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

    public void Interact()
    {
        // Find a spot to put the item into the inventory
        if (InventoryManager.instance.playerInventoryContainerManager.FindSpaceForItem(draggableItemPrefab))
        {
            Destroy(transform.parent.gameObject);
        }
        else
        {
            // tell player to make room and give them the dimensions
        }
    }
}
