using System;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateDictionary playerStateDictionary)
    : base(currentContext, playerStateDictionary) { }

    public override void EnterState()
    {
        Context.Animator.SetBool(Context.IsWalkingHash, false);
        Context.Animator.SetBool(Context.IsAimingHash, false);
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
        
        Context.ApplyStopDrag();
        
        bool weaponEquipped = WeaponManager.Instance.weaponInHand;
        if (weaponEquipped && Context.AimPressed) SwitchState(Dictionary.Aim());
        else if (weaponEquipped && Context.AttackPressed) SwitchState(Dictionary.Swing());
        else if (Context.MovePressed) SwitchState(Dictionary.Walk());
        else if (Context.GunWeaponEquipped && Context.ReloadRequested)
        {
            Context.ReloadRequested = false;
            SwitchState(Dictionary.Reload());
        }
    }
}