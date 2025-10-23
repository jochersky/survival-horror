using UnityEngine;

public class ZombieAnimationEvents : MonoBehaviour
{
    [SerializeField] private Damage rightHandDamage;
    
    public delegate void RightSwingAttackFinished();
    public event RightSwingAttackFinished OnRightSwingAttackFinished;
    
    public void RightSwingAttackEnded()
    {
        OnRightSwingAttackFinished?.Invoke();
    }

    public void ActivateRightHandHitbox()
    {
        rightHandDamage.Activate();
    }

    public void DeactivateRightHandHitbox()
    {
        Debug.Log("Deactivating right hand hitbox");
        rightHandDamage.Deactivate();
    }
}
