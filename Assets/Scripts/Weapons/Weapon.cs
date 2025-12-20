using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Damage swingDamage;
    [SerializeField] private AudioSource swingSource;
    [SerializeField] private SfxType weaponSwingHitSfx;
    
    public Damage SwingDamage => swingDamage;

    protected virtual void Start()
    {
        swingDamage.OnDamageDone += () => { AudioManager.Instance.PlaySFX(weaponSwingHitSfx, swingSource); };
    }

    public virtual void SwingAttack()
    {
        swingDamage.Activate();
        AudioManager.Instance.PlaySFX(SfxType.WeaponSwing, swingSource);
    }
    public virtual void AimAttack() { }
}
