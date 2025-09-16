using System;
using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
  public PlayerFallState(PlayerStateMachine currentContext, PlayerStateDictionary playerStateDictionary)
  : base(currentContext, playerStateDictionary)
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

  public override void InitializeSubState()
  {
    if (Context.MovePressed)
    {
      SetSubState(Dictionary.Walk());
    }
    else
    {
      SetSubState(Dictionary.Idle());
    }
  }

  public override void UpdateState()
  {
    HandleGravity();

    if (Context.CharacterController.isGrounded)
    {
      SwitchState(Dictionary.Grounded());
    }
  }

  public void HandleGravity()
  {
    Context.VerticalVelocityY += Context.Gravity * Time.deltaTime;
  }
}