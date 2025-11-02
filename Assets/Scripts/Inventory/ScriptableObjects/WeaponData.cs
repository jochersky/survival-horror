using UnityEngine;

public enum WeaponType
{
    Gun,
    Melee
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ItemData
{
    public WeaponType weaponType;
    public int magazineSize;
    public float damage;
    public float fireRate;
    public float reloadTime;
}
