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
        // Context.Agent.isStopped = true;
        // Context.CanAttack = true;
        Context.Animator.SetTrigger(Context.AttackStartHash);
        // Context.Animator.SetBool(Context.IsAttackingHash, true);
        // Context.Animator.SetBool(Context.IsChasingHash, false);
        // PerformAttack();
    }

    public override void UpdateState()
    {
        // if (Context.CanAttack && Context.PlayerInAttackRange)
        //     PerformAttack();
        //
        // if (!Context.PlayerInAttackRange)
        //     SwitchState(Dictionary.Chase());
        //
        // if (Context.Dead)
        //     SwitchState(Dictionary.Dead());
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
        // if (Context.Animator.GetBool(Context.PerformedRightSwingHash))
        // {
        //     Context.Animator.SetBool(Context.PerformedRightSwingHash, false);
        //     Context.Animator.SetBool(Context.PerformedLeftSwingHash, true);
        // }
        // else
        // {
        //     Context.Animator.SetBool(Context.PerformedLeftSwingHash, false);
        //     Context.Animator.SetBool(Context.PerformedRightSwingHash, true);
        // }
        
        // Context.StartCoroutine(Context.AttackCooldown());
    }
    
    private void SwingEnded()
    {
        Context.Animator.SetTrigger(Context.AttackEndHash);
        SwitchState(Dictionary.Chase());
    }
}
