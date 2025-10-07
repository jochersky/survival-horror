using UnityEngine;

public class ZombieIdleState : ZombieBaseState
{
    public ZombieIdleState(ZombieStateMachine currentContext, ZombieStateDictionary zombieStateDictionary)
        : base(currentContext, zombieStateDictionary) { }

    public override void EnterState()
    {
        Context.Animator.SetBool(Context.IsChasingHash, false);
        Context.Animator.SetBool(Context.IsReturningHash, false);
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
        // TODO: add check to see if damaged to go into chase state
        
        if (Context.PlayerTransform)
            SwitchState(Dictionary.Chase());
        
        if (Context.Dead)
            SwitchState(Dictionary.Dead());
    }
}
