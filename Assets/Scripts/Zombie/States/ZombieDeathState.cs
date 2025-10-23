using UnityEngine;

public class ZombieDeathState : ZombieBaseState {
    public ZombieDeathState(ZombieStateMachine currentContext, ZombieStateDictionary zombieStateDictionary)
        : base(currentContext, zombieStateDictionary) { }

    public override void EnterState()
    {
        Context.Animator.SetBool(Context.IsDeadHash, true);
        Context.Animator.SetBool(Context.IsChasingHash, false);
        Context.Animator.SetBool(Context.IsReturningHash, false);
        Context.Animator.SetTrigger(Context.AttackEndHash);
        
        // Stop the zombie from chasing player when playing death animation
        Context.Agent.isStopped = true;
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
    }
}
