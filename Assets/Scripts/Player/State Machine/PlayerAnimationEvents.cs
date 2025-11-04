using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeField] private Damage rightHandWeaponDamage;
    
    public delegate void SwingFinished();
    public event SwingFinished OnSwingFinished;
    
    public delegate void ShootFinished();
    public event SwingFinished OnShootFinished;
    
    public void SwingEnded()
    {
        OnSwingFinished?.Invoke();
    }

    public void ShootEnded()
    {
        OnShootFinished?.Invoke();
    }

    public void ActivateRightHandWeaponHitbox()
    {
        if (!rightHandWeaponDamage) return;
        
        rightHandWeaponDamage.Activate();
    }

    public void DeactivateRightHandWeaponHitbox()
    {
        if (!rightHandWeaponDamage) return;
        
        rightHandWeaponDamage.Deactivate();
    }

    public void ThrowMeleeWeapon()
    {
        // Debug.Log("Throwing weapon");
    }
}
