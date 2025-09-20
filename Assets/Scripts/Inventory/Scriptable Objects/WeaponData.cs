using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ItemData
{
    public int magazineSize;
    public float damage;
    public float fireRate;
    public float reloadTime;
}
