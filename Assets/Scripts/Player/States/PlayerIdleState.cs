using System;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
  public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateDictionary playerStateDictionary)
  : base(currentContext, playerStateDictionary) { }

  public override void EnterState()
  {
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
    ApplyStopDrag();

    if (Context.MovePressed)
    {
      SwitchState(Dictionary.Walk());
    }
  }

  private void ApplyStopDrag()
  {
    Context.MoveVelocityX *= Context.StopDrag;
    Context.MoveVelocityZ *= Context.StopDrag;
  }
}