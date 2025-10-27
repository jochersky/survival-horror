using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform)), RequireComponent(typeof(Image))]
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private ItemData _itemData;
    [SerializeField] private RectTransform followTransform;
    
    private Transform _prevParent;
    [HideInInspector] public Transform parentAfterDrag;
    private Transform _transformDuringDrag;
    private InventorySlot _prevSlot;
    [HideInInspector] public InventorySlot inventorySlot;
    [HideInInspector] public ContainerManager containerManager;
    private Canvas _canvas;
    
    private RectTransform _rectTransform;
    private Image _image;

    private Vector3 _rectPosition;
    
    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _image.sprite = _itemData.itemImage;
        _rectPosition = _rectTransform.anchoredPosition;
        _transformDuringDrag = InventoryManager.instance.InventoryUI.transform;
        _canvas = InventoryManager.instance.canvas;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (inventorySlot)
        {
            // remove the item from the grid as it's being dragged.
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
        transform.position = eventData.position + (-followTransform.anchoredPosition * _canvas.scaleFactor);
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
        _rectTransform.anchoredPosition = _rectPosition;
    }

    public GridItem CreateGridItem()
    {
        return new GridItem(null, _itemData.gridItemOrigin, _itemData.gridItemDimensions, _itemData.itemName);
    }
}
