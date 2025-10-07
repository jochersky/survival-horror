using UnityEngine;

public class ZombieGroundedState : ZombieBaseState
{
    public ZombieGroundedState(ZombieStateMachine currentContext, ZombieStateDictionary zombieStateDictionary)
        : base(currentContext, zombieStateDictionary)
    {
        IsRootState = true;
    }


    public override void EnterState()
    {
        InitializeSubState();
    }
    
    public override void ExitState()
    {
    }

    public override void UpdateState()
    {
    }
    
    public override void InitializeSubState()
    {
        if (Context.PlayerTransform)
        {
            SetSubState(Dictionary.Chase());
        }
        else
        {
            SetSubState(Dictionary.Idle());
        }
    }
}
