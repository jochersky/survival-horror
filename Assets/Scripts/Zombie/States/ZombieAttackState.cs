using UnityEngine;

public class ZombieAttackState : ZombieBaseState
{
    public ZombieAttackState(ZombieStateMachine currentContext, ZombieStateDictionary zombieStateDictionary)
        : base(currentContext, zombieStateDictionary)
    {
        Context.ZombieAnimationEvents.OnRightSwingAttackFinished += SwingEnded;
    }

    public override void EnterState()
    {
        Context.Agent.isStopped = true;
        PerformAttack();
    }

    public override void UpdateState()
    {
        if (Context.Dead)
            SwitchState(Dictionary.Dead());
    }

    public override void ExitState()
    {
        Context.Agent.isStopped = false;
    }

    public override void InitializeSubState()
    {
    }

    private void PerformAttack()
    {
        Context.Animator.SetTrigger(Context.AttackStartHash);
        Context.StartCoroutine(Context.AttackCooldown());
        AudioManager.Instance.PlaySFX(SfxType.ZombieAttack, Context.Source);
    }
    
    private void SwingEnded()
    {
        Context.Animator.SetTrigger(Context.AttackEndHash);
        SwitchState(Dictionary.Chase());
    }
}
