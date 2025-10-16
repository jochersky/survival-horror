using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform)), RequireComponent(typeof(Image))]
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private ItemData _itemData;
    
    private Transform _prevParent;
    [HideInInspector] public Transform parentAfterDrag;
    private InventorySlot _prevSlot;
    [HideInInspector] public InventorySlot inventorySlot;
    [HideInInspector] public ContainerManager containerManager;
    
    private RectTransform _rectTransform;
    private Image _image;
    
    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _image.sprite = _itemData.itemImage;
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
                _itemData.gridItemDimensions,
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
            _itemData.gridItemDimensions,
            _itemData.itemName);
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
