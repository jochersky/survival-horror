using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    Weapon,
    Ammo,
    Consumable,
    Key
}

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public Sprite itemImage;
    public GameObject itemPrefab;
    public Vector2 gridItemDimensions;
    public Vector2 gridItemOrigin;
    public int maxCount = 0;
}
