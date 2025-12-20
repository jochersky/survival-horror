using UnityEngine;

public class ZombieAnimationEvents : MonoBehaviour
{
    [SerializeField] private Damage rightHandDamage;
    [SerializeField] private AudioSource source;
    
    public delegate void RightSwingAttackFinished();
    public event RightSwingAttackFinished OnRightSwingAttackFinished;
    public delegate void ReviveFinished();
    public event ReviveFinished OnReviveFinished;

    public void RightSwingAttackStarted()
    {
        AudioManager.Instance.PlaySFX(SfxType.ZombieAttack, source);
    }
    
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

    public void OnZombieDeathFall()
    {
        AudioManager.Instance.PlaySFX(SfxType.ZombieDeathFall);
    }

    public void OnReviveEnd()
    {
        OnReviveFinished?.Invoke();
    }
}
