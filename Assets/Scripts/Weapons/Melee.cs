using System.Collections;
using UnityEngine;

public class Melee : Weapon
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Item item;
    [SerializeField] private Transform throwPoint;
    private Camera _cam;
    private LayerMask _mask;
    
    [Header("Melee Properties")]
    [SerializeField] private float maxThrowDistance = 20f;
    [SerializeField] private float maxThrowForce = 120f;
    [SerializeField] private float spinSpeed = 6f;
    
    private bool _isThrowing;
    private float _throwRate = 1.0f;

    private void Awake()
    {
        _cam = Camera.main;
        _mask = LayerMask.GetMask("EnemyHurtbox", "Environment");
    }
    
    public override void SwingAttack()
    {
        
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
