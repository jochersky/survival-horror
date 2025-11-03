using UnityEngine;

public class PlayerSwingState : PlayerBaseState
{
    private bool _swingEnded = false;
    
    public PlayerSwingState(PlayerStateMachine currentContext, PlayerStateDictionary playerStateDictionary)
        : base(currentContext, playerStateDictionary)
    {
        Context.PlayerAnimationEvents.OnSwingFinished += SwingEnded;
    }
    
    public override void EnterState()
    {
        _swingEnded = false;
        Context.Animator.SetTrigger(Context.StartSwingHash);
        // Add forward charge at start of the state
        Context.MoveVelocity = Vector2.zero;
        Context.MoveVelocity += Context.ForwardDir * 0.5f;
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
        
        if (_swingEnded)
            SwitchState(Context.Animator.GetBool(Context.IsWalkingHash) ? Dictionary.Walk() : Dictionary.Idle());
    }

    private void SwingEnded()
    {
        _swingEnded = true;
        Context.Animator.SetTrigger(Context.EndSwingHash);
    }
}
