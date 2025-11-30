using UnityEngine;

public class PlayerShootState : PlayerBaseState
{
    private bool _shootEnded = false;
    
    public PlayerShootState(PlayerStateMachine currentContext, PlayerStateDictionary playerStateDictionary)
        : base(currentContext, playerStateDictionary)
    {
        Context.PlayerAnimationEvents.OnShootFinished += ShootEnded;
    }
    
    public override void EnterState()
    {
        _shootEnded = false;
        Context.Animator.SetTrigger(Context.StartedShootingHash);
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
        
        ApplyMeshRotation();
        Context.ApplyStopDrag();
        
        if (_shootEnded)
            SwitchState(Dictionary.Zoom());
    }

    private void ShootEnded()
    {
        _shootEnded = true;
        Context.Animator.SetTrigger(Context.EndedShootingHash);
    }
    
    private void ApplyMeshRotation()
    {
        Context.PlayerMesh.transform.rotation = Context.RotatedOrientation.rotation;
    }
}
