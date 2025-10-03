using UnityEngine;

public class ZombieChaseState : ZombieBaseState
{
    public ZombieChaseState(ZombieStateMachine currentContext, ZombieStateDictionary zombieStateDictionary)
        : base(currentContext, zombieStateDictionary) { }

    public override void EnterState()
    {
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
        if (Context.PlayerTransform) 
            Context.Agent.SetDestination(Context.PlayerTransform.position);
        else
            SwitchState(Dictionary.Idle());
    }
}
