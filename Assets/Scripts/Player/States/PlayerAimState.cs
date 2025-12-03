using System;
using UnityEngine;

public class PlayerAimState : PlayerBaseState
{
    public PlayerAimState(PlayerStateMachine currentContext, PlayerStateDictionary playerStateDictionary)
        : base(currentContext, playerStateDictionary) { }

    public override void EnterState()
    {
        Context.Animator.SetBool(Context.IsWalkingHash, false);
        Context.Animator.SetBool(Context.IsAimingHash, true);
    }

    public override void ExitState()
    {
        Context.transform.rotation = Context.PlayerMesh.transform.rotation;
        Context.PlayerMesh.transform.rotation = Context.transform.rotation;
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
        if (Context.Dead) SwitchState(Dictionary.Dead());
        
        ApplyMeshRotation();
        ApplyMoveVelocity();

        if (Context.AttackPressed && Context.GunWeaponEquipped && Context.AimAttackRequested)
        {
            Context.AimAttackRequested = false;
            SwitchState(Dictionary.Shoot());
        }
        else if (Context.AttackPressed && Context.MeleeWeaponEquipped) SwitchState(Dictionary.Throw());
        else if (!Context.AimPressed) SwitchState(Context.MovePressed ? Dictionary.Walk() : Dictionary.Idle());
        else if (Context.GunWeaponEquipped && Context.ReloadRequested)
        {
            Context.ReloadRequested = false;
            SwitchState(Dictionary.Reload());
        }
    }

    private void ApplyMoveVelocity()
    {
        Vector3 rotatedForwardDir = Quaternion.AngleAxis(7.5f, Context.Orientation.transform.up) * Context.Orientation.transform.forward;
        Vector3 rotatedRightDir = Quaternion.AngleAxis(7.5f, Context.Orientation.transform.up) * Context.Orientation.transform.right;
        Vector3 moveDir = rotatedForwardDir * Context.MoveInput.y + rotatedRightDir * Context.MoveInput.x;
        Context.CurrentHorizontalSpeed += Context.MoveAccel * Time.deltaTime;
        Context.CurrentHorizontalSpeed = Mathf.Min(Context.CurrentHorizontalSpeed, Context.MaxAimMoveSpeed);
        Context.MoveVelocity = moveDir * Context.CurrentHorizontalSpeed;
    }

    private void ApplyMeshRotation()
    {
        Context.PlayerMesh.transform.rotation = Context.RotatedOrientation.rotation;
    }
}