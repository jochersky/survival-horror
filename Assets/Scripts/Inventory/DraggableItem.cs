using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform)), RequireComponent(typeof(Image))]
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform _prevParent;
    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public ContainerManager containerManager;
    private InventorySlot _prevSlot;
    [HideInInspector] public InventorySlot inventorySlot;
    
    // TODO: These eventually should just be held within a single item scriptable object
    [SerializeField] private string itemName;
    [SerializeField] private Vector2 dimensions;
    
    private RectTransform _rectTransform;
    private Image _image;
    
    public string Name { get => itemName; set => itemName = value; }
    
    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (inventorySlot)
        {
            // remove the item as it's being held from the grid
            // (set grid values back to default GridItem + enable surrounding InventorySlots)
            GridItem empty = new GridItem(
                inventorySlot.Grid,
                new Vector2(inventorySlot.X, inventorySlot.Y),
                dimensions,
                "empty");
            containerManager.SetItem(inventorySlot.X, inventorySlot.Y, empty);
        }

        _prevSlot = inventorySlot;
        _prevParent = transform.parent;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        _image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // TODO: get the item's origin and use that to pick up the item
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) - new Vector3(-1,1,0) * 16f;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // InventorySlot.OnDrop runs first
        // - inventorySlot is set by the InventorySlot this is dropped on -
        // - containerManager is set by the InventorySlot this is dropped on -
        
        GridItem item = new GridItem(
            inventorySlot.Grid,
            new Vector2(inventorySlot.X, inventorySlot.Y),
            dimensions,
            itemName);
        if (!containerManager.SetItem(inventorySlot.X, inventorySlot.Y, item))
        {
            parentAfterDrag = _prevParent;
            inventorySlot = _prevSlot;
            containerManager.SetItem(inventorySlot.X, inventorySlot.Y, item);
        }

        transform.SetParent(parentAfterDrag);
        _image.raycastTarget = true;
        _rectTransform.anchoredPosition = Vector3.zero;
    }
}
