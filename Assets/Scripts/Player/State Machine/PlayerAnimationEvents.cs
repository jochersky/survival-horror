using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    
    public delegate void SwingBegin();
    public event SwingBegin OnSwingBegin;
    public delegate void SwingFinished();
    public event SwingFinished OnSwingFinished;

    public delegate void FireBegin();
    public event FireBegin OnFireBegin;
    public delegate void FireFinished();
    public event FireFinished OnFireFinished;
    public delegate void ReloadFinished();
    public event ReloadFinished OnReloadFinished;
    public delegate void WeaponThrown();
    public event WeaponThrown OnWeaponThrown;

    public void SwingStarted()
    {
        OnSwingBegin?.Invoke();
    }
    
    public void SwingEnded()
    {
        // TODO
        OnSwingFinished?.Invoke();
    }

    public void FireStarted()
    {
        OnFireBegin?.Invoke();
    } 

    public void FireEnded()
    {
        OnFireFinished?.Invoke();
    }

    public void ReloadEnded()
    {
        OnReloadFinished?.Invoke();
    }

    public void ThrowMeleeWeapon()
    {
        OnWeaponThrown?.Invoke();
    }

    public void PlayFootstepSound()
    {
        AudioManager.Instance.PlaySFX(SfxType.PlayerFootsteps, source);
    }

    public void OnPlayerDeathFall()
    {
        AudioManager.Instance.PlaySFX(SfxType.PlayerDeathFall, source);
    }
}
