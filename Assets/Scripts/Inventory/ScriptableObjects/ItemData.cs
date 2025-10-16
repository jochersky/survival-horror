using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemImage;
    public GameObject itemPrefab;
    public Vector2 gridItemDimensions;
    public Vector2 gridItemOrigin;
}
