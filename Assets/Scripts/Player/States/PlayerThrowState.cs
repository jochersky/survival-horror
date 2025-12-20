using UnityEngine;

public class PlayerThrowState : PlayerBaseState
{
    private bool _throwEnded = false;
    
    public PlayerThrowState(PlayerStateMachine currentContext, PlayerStateDictionary playerStateDictionary)
        : base(currentContext, playerStateDictionary)
    {
        Context.PlayerAnimationEvents.OnSwingFinished += ThrowEnded;
    }
    
    public override void EnterState()
    {
        _throwEnded = false;
        Context.Animator.SetTrigger(Context.StartThrowHash);
        
        Context.MoveVelocity = Vector3.zero;
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
        if (Context.Dead) SwitchState(Dictionary.Dead());
        
        if (_throwEnded)
            SwitchState(Context.Animator.GetBool(Context.IsWalkingHash) ? Dictionary.Walk() : Dictionary.Idle());
    }

    private void ThrowEnded()
    {
        if (Context.CurrentSubState != this) return;
        _throwEnded = true;
        Context.Animator.SetTrigger(Context.EndThrowHash);
    }
}
