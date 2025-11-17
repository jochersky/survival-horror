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
    
    private bool _isThrowing;
    private float _throwRate = 1.0f;

    private void Awake()
    {
        _cam = Camera.main;
        _mask = LayerMask.GetMask("EnemyHurtbox", "Environment");
    }

    private void Update()
    {
        if (WeaponManager.instance._isZooming)
        {
            Vector3 throwingDirection = (_cam.transform.forward - throwPoint.forward).normalized;
            
            Debug.DrawRay(throwPoint.position, throwingDirection, Color.red);
            Debug.DrawRay(_cam.transform.position, _cam.transform.forward, Color.green);
        }
    }
    
    public override void SwingAttack()
    {
        
    }

    public override void AimAttack()
    {
        if (!_isThrowing) CalculateThrow();
    }

    private void CalculateThrow()
    {
        Vector3 throwingDirection = (_cam.transform.forward - throwPoint.forward).normalized;
            
        Debug.DrawRay(throwPoint.position, throwingDirection, Color.red);
        Debug.DrawRay(_cam.transform.position, _cam.transform.forward, Color.green);
            
        Physics.Raycast(_cam.transform.position, _cam.transform.forward, out RaycastHit camHit, 100f, _mask);

        // adjust where the bullet will hit if something collides with a ray going out of the camera
        throwingDirection = (camHit.transform ? 
            camHit.point - throwPoint.position : 
            (_cam.transform.forward - throwPoint.forward).normalized);
            
        ThrowWeapon(throwingDirection);
    }
    
    private void ThrowWeapon(Vector3 throwingDirection)
    {
        transform.SetParent(InventoryManager.instance.transform);
        item.Unequip();
        WeaponManager.instance.UnequipThrownWeapon();
        
        Vector3 throwVector = (throwingDirection + (throwPoint.up * 0.2f)) * 20;
        rb.AddForce(throwVector, ForceMode.Impulse);
        rb.AddRelativeTorque(-throwPoint.right * 8, ForceMode.Impulse);
        
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
