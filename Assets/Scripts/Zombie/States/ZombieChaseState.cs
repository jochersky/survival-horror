using UnityEngine;

public class ZombieChaseState : ZombieBaseState
{
    public ZombieChaseState(ZombieStateMachine currentContext, ZombieStateDictionary zombieStateDictionary)
        : base(currentContext, zombieStateDictionary) { }

    public override void EnterState()
    {
        Context.Animator.SetBool(Context.IsChasingHash, true);
        Context.Animator.SetBool(Context.IsReturningHash, false);
        
        if (Context.PlayerTransform) 
            Context.Agent.SetDestination(Context.PlayerTransform.position);
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
        // Stay in the chase state as long as there is a player transform
        if (Context.PlayerTransform)
            Context.Agent.SetDestination(Context.PlayerTransform.position);
        else
            SwitchState(Dictionary.Return());
        
        if (Context.Dead)
            SwitchState(Dictionary.Dead());
    }
}
