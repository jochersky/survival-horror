using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    public delegate void SwingBegin();
    public event SwingBegin OnSwingBegin;
    public delegate void SwingFinished();
    public event SwingFinished OnSwingFinished;
    public delegate void ShootFinished();
    public event SwingFinished OnShootFinished;
    public delegate void WeaponThrown();
    public event WeaponThrown OnWeaponThrown;

    public void SwingStarted()
    {
        OnSwingBegin?.Invoke();
    }
    
    public void SwingEnded()
    {
        OnSwingFinished?.Invoke();
    }

    public void ShootEnded()
    {
        OnShootFinished?.Invoke();
    }

    public void ThrowMeleeWeapon()
    {
        OnWeaponThrown?.Invoke();
    }
}
