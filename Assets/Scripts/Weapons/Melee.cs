using System.Collections;
using UnityEngine;

public class Melee : Weapon
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Item item;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private Damage throwDamage;
    private Camera _cam;
    private LayerMask _mask;
    
    [Header("Melee Properties")]
    [SerializeField] private float maxThrowDistance = 20f;
    [SerializeField] private float maxThrowForce = 120f;
    [SerializeField] private float spinSpeed = 6f;
    
    private bool _isThrowing;

    private void Awake()
    {
        _cam = Camera.main;
        _mask = LayerMask.GetMask("EnemyHurtbox", "Environment");
    }

    public override void AimAttack()
    {
        if (!_isThrowing) ThrowWeapon();
    }
    
    private void ThrowWeapon()
    {
        transform.SetParent(InventoryManager.instance.transform);
        item.Unequip();
        WeaponManager.instance.UnequipThrownWeapon();
        
        Vector3 throwingDirection = (_cam.transform.forward * maxThrowDistance - throwPoint.forward).normalized;
        // point the direction of the vector slightly up before applying throw force
        Vector3 throwVector = (throwingDirection + (throwPoint.up * 0.1f)).normalized * maxThrowForce;
        
        rb.AddForce(throwVector, ForceMode.Impulse);
        rb.AddRelativeTorque(-throwPoint.right * spinSpeed, ForceMode.Impulse);
        
        throwDamage.Activate();
    }
}
