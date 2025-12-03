using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Damage swingDamage;
    
    public Damage SwingDamage => swingDamage;
    
    public virtual void SwingAttack()
    {
        swingDamage.Activate();
    }
    public virtual void AimAttack() { }
}
