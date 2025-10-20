using System;
using UnityEngine;

public class PlayerZoomState : PlayerBaseState
{
    public PlayerZoomState(PlayerStateMachine currentContext, PlayerStateDictionary playerStateDictionary)
        : base(currentContext, playerStateDictionary) { }

    public override void EnterState()
    {
        Context.Animator.SetBool(Context.IsWalkingHash, false);
        Context.Animator.SetBool(Context.IsZoomingHash, true);
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
        
        ApplyMoveVelocity();
        ApplyRotation();
        
        if (!Context.ZoomPressed) 
            SwitchState(Context.MovePressed ? Dictionary.Walk() : Dictionary.Idle());
    }

    private void ApplyMoveVelocity()
    {
        Vector3 moveDir = Context.ForwardDir * Context.MoveInput.y + Context.RightDir * Context.MoveInput.x;
        Context.CurrentHorizontalSpeed += Context.MoveAccel * Time.deltaTime;
        Context.CurrentHorizontalSpeed = Mathf.Min(Context.CurrentHorizontalSpeed, Context.MaxMoveSpeed);
        Context.MoveVelocity = moveDir * Context.CurrentHorizontalSpeed;
    }

    private void ApplyRotation()
    {
        Context.transform.rotation = Context.Orientation.transform.rotation;
    }
}