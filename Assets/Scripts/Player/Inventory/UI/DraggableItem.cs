using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image;

    [SerializeField] private string itemName;
    [SerializeField] private int itemCount = 1;
    [SerializeField] private int maxItemCount = 3;
    [SerializeField] private GameObject itemCounterImage;
    [SerializeField] private TextMeshProUGUI itemCounterText;
    
    [HideInInspector] public Transform parentAfterDrag;

    public string ItemName => itemName;
    public int ItemCount { get => itemCount; set { itemCount = value; itemCounterText.text = value.ToString(); } }
    public int MaxItemCount => maxItemCount;

    private void Start()
    {
        // reveal any items with item counts 1 or more
        if (itemCount >= 1)
        {
            itemCounterImage.SetActive(true);
            itemCounterText.text = itemCount.ToString();
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
    }
}
