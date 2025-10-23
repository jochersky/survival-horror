using UnityEngine;

public class PlayerSwingState : PlayerBaseState
{
    public PlayerSwingState(PlayerStateMachine currentContext, PlayerStateDictionary playerStateDictionary)
        : base(currentContext, playerStateDictionary)
    {
        Context.PlayerAnimationEvents.OnSwingFinished += SwingEnded;
    }
    
    public override void EnterState()
    {
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
    }

    private void SwingEnded()
    {
        Context.Animator.SetTrigger(Context.EndSwingHash);
        SwitchState(Dictionary.Zoom());
    }
}
