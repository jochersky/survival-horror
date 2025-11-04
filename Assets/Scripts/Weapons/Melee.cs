using UnityEngine;

public class Melee : Weapon
{
    public override void SwingAttack()
    {
        
    }

    public override void AimAttack()
    {
        Debug.Log("run");
        GameObject inst = Instantiate(this.gameObject, InventoryManager.instance.gameObject.transform);
    }
}
