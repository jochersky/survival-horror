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

    private Vector3 _rectPosition;
    
    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _image.sprite = _itemData.itemImage;
        _rectPosition = _rectTransform.anchoredPosition;
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
        int xOffset = _itemData.gridItemDimensions.x > 1 ? (int) -_rectTransform.sizeDelta.x / 4 : 0;
        int yOffset = _itemData.gridItemDimensions.y > 1 ? (int) _rectTransform.sizeDelta.y / 4 : 0;
        Vector3 offset = new Vector3(xOffset, yOffset, 0);
            
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset;
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
}
