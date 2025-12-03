using UnityEngine;

public class PlayerShootState : PlayerBaseState
{
    private bool _fireEnded = false;
    
    public PlayerShootState(PlayerStateMachine currentContext, PlayerStateDictionary playerStateDictionary)
        : base(currentContext, playerStateDictionary)
    {
        Context.PlayerAnimationEvents.OnFireFinished += FireEnded;
    }
    
    public override void EnterState()
    {
        _fireEnded = false;
        Context.Animator.SetTrigger(Context.StartedShootingHash);
        Context.MoveVelocityX = 0;
        Context.MoveVelocityZ = 0;
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
        
        ApplyMeshRotation();
        
        if (_fireEnded)
            SwitchState(Dictionary.Aim());
    }

    private void FireEnded()
    {
        _fireEnded = true;
        Context.Animator.SetTrigger(Context.EndedShootingHash);
    }
    
    private void ApplyMeshRotation()
    {
        Context.PlayerMesh.transform.rotation = Context.RotatedOrientation.rotation;
    }
}
