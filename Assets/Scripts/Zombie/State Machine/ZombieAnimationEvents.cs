using UnityEngine;

public class ZombieAnimationEvents : MonoBehaviour
{
    [SerializeField] private Damage rightHandDamage;
    
    public delegate void RightSwingAttackFinished();
    public event RightSwingAttackFinished OnRightSwingAttackFinished;
    public delegate void ReviveFinished();
    public event ReviveFinished OnReviveFinished;
    
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
        rightHandDamage.Deactivate();
    }

    public void OnReviveEnd()
    {
        OnReviveFinished?.Invoke();
    }
}
