using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
  public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateDictionary playerStateDictionary)
  : base(currentContext, playerStateDictionary) { }

  public override void EnterState()
  {
    Context.Animator.SetBool(Context.IsWalkingHash, true);
    Context.Animator.SetBool(Context.IsZoomingHash, false);
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
    ApplyMeshRotation();

    bool weaponEquipped = WeaponManager.instance.weaponInHand;
    
    if (weaponEquipped && Context.ZoomPressed) SwitchState(Dictionary.Zoom());
    else if (weaponEquipped && Context.AttackPressed) SwitchState(Dictionary.Swing());
    else if (!Context.MovePressed) SwitchState(Dictionary.Idle());
    else if (Context.GunWeaponEquipped && Context.ReloadPressed) SwitchState(Dictionary.Reload());
  }

  private void ApplyMoveVelocity()
  {
    Vector3 moveDir = Context.ForwardDir * Context.MoveInput.y + Context.RightDir * Context.MoveInput.x;
    Context.CurrentHorizontalSpeed += Context.MoveAccel * Time.deltaTime;
    Context.CurrentHorizontalSpeed = Mathf.Min(Context.CurrentHorizontalSpeed, Context.MaxMoveSpeed);
    Context.MoveVelocity = moveDir * Context.CurrentHorizontalSpeed;
  }

  private void ApplyMeshRotation()
  {
    Vector3 moveDir = Context.ForwardDir * Context.MoveInput.y + Context.RightDir * Context.MoveInput.x;
    Vector3 targetVector = (moveDir + Context.PlayerMesh.transform.position) - Context.PlayerMesh.transform.position;
    float rotSpeed = Context.WalkRotationSpeed * Time.deltaTime;
    Vector3 newDir = Vector3.RotateTowards(Context.PlayerMesh.transform.forward, targetVector, rotSpeed, 0);
    Context.PlayerMesh.transform.rotation = Quaternion.LookRotation(newDir);
  }
}