using UnityEngine;

public class PlayerDeadState : PlayerBaseState
{
    public PlayerDeadState(PlayerStateMachine currentContext, PlayerStateDictionary playerStateDictionary) 
    : base(currentContext, playerStateDictionary) { }
    
    public override void EnterState()
    {
        Context.Animator.SetBool(Context.IsDeadHash, true);
        Context.CurrentHorizontalSpeed = 0;
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
        Context.ApplyStopDrag();
    }
}
