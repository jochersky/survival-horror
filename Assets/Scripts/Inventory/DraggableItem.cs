using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform)), RequireComponent(typeof(Image))]
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Grid<DraggableItem> _grid;
    private RectTransform _rectTransform;
    private Image _image;
    
    private Vector2 _dimensions;
    private string _name = "item";
    
    [HideInInspector] public Transform parentAfterDrag;
    
    public string Name { get => _name; set => _name = value; }
    
    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        _image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) - new Vector3(-1,1,0) * 16f;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        _image.raycastTarget = true;
        _rectTransform.anchoredPosition = Vector3.zero;
        // if (inventorySlot.TryGetComponent(out RectTransform rt) && inventorySlot.TryGetComponent(out Image img))
        // {
        //     if (IsOverlapping(_rectTransform, rt))
        //         img.enabled = false;
        // }
    }

    bool IsOverlapping(RectTransform rt1, RectTransform rt2)
    {
        Rect r1 = new Rect(rt1.position.x, rt1.position.y, rt1.rect.width, rt1.rect.height);
        Rect r2 = new Rect(rt2.position.x, rt2.position.y, rt2.rect.width, rt2.rect.height);
        return r1.Overlaps(r2);
    }
}
