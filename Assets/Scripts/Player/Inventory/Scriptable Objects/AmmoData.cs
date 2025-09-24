using UnityEngine;

[CreateAssetMenu(fileName = "AmmoData", menuName = "Scriptable Objects/AmmoData")]
public class AmmoData : ItemData
{
    public int minCount;
    public int maxCount;
    public string gunType;
}
