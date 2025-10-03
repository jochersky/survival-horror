using UnityEngine;

public class ZombieIdleState : ZombieBaseState
{
    public ZombieIdleState(ZombieStateMachine currentContext, ZombieStateDictionary zombieStateDictionary)
        : base(currentContext, zombieStateDictionary) { }

    public override void EnterState()
    {
        Context.Agent.SetDestination(Context.StartingPosition);
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
            SwitchState(Dictionary.Chase());
    }
}
