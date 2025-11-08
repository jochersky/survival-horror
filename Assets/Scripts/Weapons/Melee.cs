using System.Collections;
using UnityEngine;

public class Melee : Weapon
{
    private bool _isThrowing;
    private float _throwRate = 1.0f;
    
    public override void SwingAttack()
    {
        
    }

    public override void AimAttack()
    {
        if (!_isThrowing) ThrowWeapon();
    }

    private void ThrowWeapon()
    {
        GameObject inst = Instantiate(this.gameObject, InventoryManager.instance.gameObject.transform);
        WeaponManager.instance.UnequipCurrent();
        StartCoroutine(Throw());
    }
    
    private IEnumerator Throw()
    {
        _isThrowing = true;
        
        float timer = 0;
        while (timer < _throwRate)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        
        _isThrowing = false;
    }
}
