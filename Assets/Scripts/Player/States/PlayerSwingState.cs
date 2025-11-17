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
        Context.Animator.SetBool(Context.IsWalkingHash, false);
        _swingEnded = false;
        Context.Animator.SetTrigger(Context.StartSwingHash);
        
        // turn player in direction of cam
        Context.Player.transform.forward = Context.Orientation.transform.forward;
        Context.Orientation.transform.forward = Context.Player.transform.forward;
        
        // TODO: Add forward charge when animation steps forward
        Context.MoveVelocity = Vector3.zero; // leave this line here in enter
        Context.MoveVelocity += Context.ForwardDir * 0.5f;
    }

    public override void ExitState()
    {
        Context.MoveVelocity = Vector2.zero;
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
        if (Context.Dead) SwitchState(Dictionary.Dead());
        
        if (_swingEnded)
            SwitchState(Dictionary.Idle());
    }

    private void SwingEnded()
    {
        _swingEnded = true;
        Context.Animator.SetTrigger(Context.EndSwingHash);
    }
}
