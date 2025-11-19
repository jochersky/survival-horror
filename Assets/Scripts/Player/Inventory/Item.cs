using System;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private Transform mainTransform;
    [SerializeField] private Transform rbFollowTransform;
    [SerializeField] private Transform handTransform;
    [SerializeField] private Transform backTransform;
    [SerializeField] private Transform attackTransform;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject draggableItemPrefab;
    [SerializeField] private ItemData itemData;
    
    [Header("Instance Values")]
    [SerializeField] private int count = 1;
    
    private Container _container;
    
    // Getters and Setters
    public int Count { get { return count; } set { count = value; } }

    public void FixedUpdate()
    {
        if (rbFollowTransform)
        {
            transform.position = rbFollowTransform.position;
            transform.rotation = rbFollowTransform.rotation;
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
        bool foundSpot = InventoryManager.instance.playerInventoryContainerManager.FindSpaceForItem(draggableItemPrefab, count);
        if (foundSpot)
        {
            Destroy(transform.parent.gameObject);
        }
        else
        {
            // TODO: tell player to make room and give them the dimensions
        }
    }

    public void Equip()
    {
        rb.detectCollisions = false;
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.None;
    }

    public void Unequip()
    {
        rb.detectCollisions = true;
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    public void MoveToHand()
    {
        mainTransform.localPosition = handTransform.localPosition;
        mainTransform.localRotation = handTransform.localRotation;
    }

    public void MoveToBack()
    {
        mainTransform.localPosition = backTransform.localPosition;
        mainTransform.localRotation = backTransform.localRotation;
    }

    public void MoveToAttack()
    {
        mainTransform.SetParent(attackTransform);
        mainTransform.localRotation = attackTransform.localRotation;
        mainTransform.localPosition = attackTransform.localPosition;
    }
}
