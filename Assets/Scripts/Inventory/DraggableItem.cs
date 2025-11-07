using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(RectTransform)), RequireComponent(typeof(Image))]
public class DraggableItem : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [InspectorName("References")]
    public ItemData itemData;
    public GameObject itemPrefab;
    [SerializeField] private RectTransform followTransform;
    public GameObject equippedIcon;
    [SerializeField] private GameObject itemOptionsButtons;
    private Transform _prevParent;
    [HideInInspector] public Transform parentAfterDrag;
    private Transform _transformDuringDrag;
    private InventorySlot _prevSlot;
    [HideInInspector] public InventorySlot inventorySlot;
    [HideInInspector] public WeaponSlot weaponSlot;
    [HideInInspector] public ContainerManager containerManager;
    private Canvas _canvas;
    private RectTransform _rectTransform;
    private Vector2 _initialRectPos;
    private Vector2 _initialAnchorMin;
    private Vector2 _initialAnchorMax;
    private Image _image;

    private GameObject equippedItemInst;

    private Vector3 _rectPosition;

    private bool _itemOptionButtonsShowing;
    
    private void Start()
    {
        itemOptionsButtons.SetActive(false);
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _image.sprite = itemData.itemImage;
        _rectPosition = _rectTransform.anchoredPosition;
        _initialRectPos = _rectTransform.anchoredPosition;
        _initialAnchorMin = _rectTransform.anchorMin;
        _initialAnchorMax = _rectTransform.anchorMax;
        _transformDuringDrag = InventoryManager.instance.inventoryUI.transform;
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
                itemData.gridItemDimensions,
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
        // - inventorySlot is set by the InventorySlot this is dropped on
        // - containerManager is set by the InventorySlot this is dropped on

        if (inventorySlot)
        {
            GridItem item = new GridItem(
                inventorySlot.Grid,
                new Vector2(inventorySlot.X, inventorySlot.Y),
                itemData.gridItemDimensions,
                itemData.itemName);
            if (!containerManager.SetItem(inventorySlot.X, inventorySlot.Y, item))
            {
                parentAfterDrag = _prevParent;
                inventorySlot = _prevSlot;
                containerManager.SetItem(inventorySlot.X, inventorySlot.Y, item);
            }
        }

        transform.SetParent(parentAfterDrag);
        _image.raycastTarget = true;
        
        // Assign transform values based on what kind of slot the item is in
        if (inventorySlot)
        {
            if (weaponSlot)
            {
                weaponSlot.UnequipWeaponFromSlot();
                weaponSlot = null;
            }
            _rectTransform.anchoredPosition = _initialRectPos;
            _rectTransform.anchorMin = _initialAnchorMin;
            _rectTransform.anchorMax = _initialAnchorMax;
        }
        else
        {
            inventorySlot = null;
            _rectTransform.anchoredPosition= Vector2.zero;
            _rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            _rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        }
    }

    public GridItem CreateGridItem()
    {
        return new GridItem(null, itemData.gridItemOrigin, itemData.gridItemDimensions, itemData.itemName);
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

    public void DropItem()
    {
        InventoryManager.instance.SpawnItem(itemPrefab);
        
        GridItem item = new GridItem(
            inventorySlot.Grid,
            new Vector2(inventorySlot.X, inventorySlot.Y),
            itemData.gridItemDimensions,
            "empty");
        containerManager.SetItem(inventorySlot.X, inventorySlot.Y, item);
        
        Destroy(this.gameObject);
    }
}
