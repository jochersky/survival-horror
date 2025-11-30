using UnityEngine;

public class PlayerReloadState : PlayerBaseState
{
    public PlayerReloadState(PlayerStateMachine currentContext, PlayerStateDictionary playerStateDictionary)
        : base(currentContext, playerStateDictionary)
    {
        Context.PlayerAnimationEvents.OnReloadFinished += ReloadFinished;
    }

    public override void EnterState()
    {
        Context.Animator.SetTrigger(Context.StartReloadHash);
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
        if (Context.Dead) SwitchState(Dictionary.Dead());

        Context.MoveVelocity *= Context.StopDrag;
    }

    private void ReloadFinished()
    {
        Context.Animator.SetTrigger(Context.EndReloadHash);
        
        if (Context.ZoomPressed) SwitchState(Dictionary.Zoom());
        else if (Context.MovePressed) SwitchState(Dictionary.Walk());
        else SwitchState(Dictionary.Idle());
    }
}
