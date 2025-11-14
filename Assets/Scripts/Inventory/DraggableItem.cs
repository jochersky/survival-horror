using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

[RequireComponent(typeof(RectTransform)), RequireComponent(typeof(Image))]
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    public ItemData itemData;
    public GameObject itemPrefab;
    [SerializeField] private RectTransform followTransform;
    [SerializeField] private RectTransform inventorySlotTransform;
    [HideInInspector] public Transform parentAfterDrag;
    private Transform _prevParent;
    [HideInInspector] public InventorySlot inventorySlot;
    private InventorySlot _prevSlot;
    [HideInInspector] public WeaponSlot weaponSlot;
    private WeaponSlot _prevWeaponSlot;
    [HideInInspector] public ContainerManager containerManager;
    private Canvas _canvas;
    private RectTransform _rectTransform;
    private Vector2 _initialRectPos;
    private Image _image;
    [SerializeField] private TextMeshProUGUI _countLabel;

    private GameObject equippedItemInst;
    
    [Header("Instance Values")]
    [SerializeField] private int count = 1;
    
    // Getters and Setters
    public int Count
    {
        get => count;
        set
        {
            if (_countLabel)
            {
                _countLabel.text = value.ToString();
                count = value;
            }
        }
    }
    public RectTransform RectTransform => _rectTransform;
    public Vector2 InitialRectPos => _initialRectPos;
    
    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _image.sprite = itemData.itemImage;
        _initialRectPos = _rectTransform.anchoredPosition;
        _canvas = InventoryManager.instance.canvas;
        
        if (_countLabel) _countLabel.text = count.ToString();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        
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
        _prevWeaponSlot = weaponSlot;
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
                // containerManager.SetItem(inventorySlot.X, inventorySlot.Y, item);
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
            SetTransformToInventorySlot();
        }
        else
        {
            if (_prevWeaponSlot && _prevWeaponSlot != weaponSlot)
            {
                _prevWeaponSlot.UnequipWeaponFromSlot();
            }
            if (inventorySlot)
            {
                containerManager = null;
                inventorySlot = null;
            }
            SetTransformToWeaponSlot();
        }
    }

    public GridItem CreateGridItem()
    {
        return new GridItem(null, itemData.gridItemOrigin, itemData.gridItemDimensions, itemData.itemName);
    }

    public void DropItem()
    {
        InventoryManager.instance.SpawnItem(itemPrefab);
        containerManager.DropItemWithName(itemData.itemName);

        if (inventorySlot)
        {
            GridItem item = new GridItem(
                inventorySlot.Grid,
                new Vector2(inventorySlot.X, inventorySlot.Y),
                itemData.gridItemDimensions,
                "empty");
            containerManager.SetItem(inventorySlot.X, inventorySlot.Y, item);
        }

        if (weaponSlot)
        {
            weaponSlot.UnequipWeaponFromSlot();
        }
        
        Destroy(this.gameObject);
    }

    private void SetTransformToWeaponSlot()
    {
        _rectTransform.anchoredPosition = Vector2.zero;
        _rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        _rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
    }

    private void SetTransformToInventorySlot()
    {
        _rectTransform.anchoredPosition = inventorySlotTransform.anchoredPosition;
        _rectTransform.anchorMax = inventorySlotTransform.anchorMax;
        _rectTransform.anchorMin = inventorySlotTransform.anchorMin;
    }
}
