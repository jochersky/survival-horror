using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(RectTransform)), RequireComponent(typeof(Image))]
public class DraggableItem : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [InspectorName("References")]
    [SerializeField] private ItemData _itemData;
    public GameObject itemPrefab;
    [SerializeField] private RectTransform followTransform;
    public GameObject equippedIcon;
    [SerializeField] private GameObject itemOptionsButtons;
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

    private bool _itemOptionButtonsShowing;
    
    private void Start()
    {
        itemOptionsButtons.SetActive(false);
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _image.sprite = _itemData.itemImage;
        _rectPosition = _rectTransform.anchoredPosition;
        _transformDuringDrag = InventoryManager.instance.InventoryUI.transform;
        _canvas = InventoryManager.instance.canvas;
    }

    private void Update()
    {
        CheckMouseRangeForItemOptions();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            itemOptionsButtons.SetActive(false);
            _itemOptionButtonsShowing = false;
        }
        
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (itemOptionsButtons.TryGetComponent<RectTransform>(out RectTransform tf))
            {
                tf.position = Input.mousePosition;
            }
            itemOptionsButtons.SetActive(true);
            _itemOptionButtonsShowing = true;
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        
        itemOptionsButtons.SetActive(false);
        _itemOptionButtonsShowing = false;
        
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
        if (eventData.button != PointerEventData.InputButton.Left) return;
        
        transform.position = eventData.position + (-followTransform.anchoredPosition * _canvas.scaleFactor);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        
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

    private void CheckMouseRangeForItemOptions()
    {
        if (_itemOptionButtonsShowing && itemOptionsButtons.TryGetComponent<RectTransform>(out RectTransform tf))
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 itemOptionPos = new Vector2(tf.position.x, tf.position.y);
            Vector2 itemOptionsOriginOffset = (new Vector2(-tf.sizeDelta.x, -tf.sizeDelta.y) / 2) * _canvas.scaleFactor;
            Vector2 itemOptionsOrigin = itemOptionPos + itemOptionsOriginOffset;
            Vector2 itemOptionsExtentOffset = (new Vector2(tf.sizeDelta.x, tf.sizeDelta.y) / 2) * _canvas.scaleFactor;
            Vector2 itemOptionsExtent = itemOptionPos + itemOptionsExtentOffset;
            
            if (mousePos.x > itemOptionsExtent.x || mousePos.x < itemOptionsOrigin.x ||
                mousePos.y > itemOptionsExtent.y || mousePos.y < itemOptionsOrigin.y)
            {
                _itemOptionButtonsShowing = false;
                itemOptionsButtons.SetActive(false);
            }
        }
    }

    public void EquipItem()
    {
        equippedIcon.SetActive(true);
        Debug.Log("equip this item");
    }

    public void DropItem()
    {
        InventoryManager.instance.SpawnItem(itemPrefab);
        
        GridItem item = new GridItem(
            inventorySlot.Grid,
            new Vector2(inventorySlot.X, inventorySlot.Y),
            _itemData.gridItemDimensions,
            "empty");
        containerManager.SetItem(inventorySlot.X, inventorySlot.Y, item);
        Destroy(this.gameObject);
    }
}
