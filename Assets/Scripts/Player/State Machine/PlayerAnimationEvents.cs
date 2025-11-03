using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeField] private Damage rightHandWeaponDamage;
    
    public delegate void SwingFinished();
    public event SwingFinished OnSwingFinished;
    
    public void SwingEnded()
    {
        OnSwingFinished?.Invoke();
    }

    public void ActivateRightHandWeaponHitbox()
    {
        rightHandWeaponDamage.Activate();
    }

    public void DeactivateRightHandWeaponHitbox()
    {
        rightHandWeaponDamage.Deactivate();
    }

    public void ThrowMeleeWeapon()
    {
        // Debug.Log("Throwing weapon");
    }
}
