using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private float swingDmg;
    
    public virtual void SwingAttack() { }
    public virtual void AimAttack() { }
}
