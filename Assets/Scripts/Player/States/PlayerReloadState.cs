using UnityEngine;

public class PlayerReloadState : PlayerBaseState
{
    private bool _reloadEnded = false;
    public PlayerReloadState(PlayerStateMachine currentContext, PlayerStateDictionary playerStateDictionary)
        : base(currentContext, playerStateDictionary)
    {
        Context.PlayerAnimationEvents.OnReloadFinished += ReloadFinished;
    }

    public override void EnterState()
    {
        Context.Animator.SetTrigger(Context.StartReloadHash);
        _reloadEnded = false;
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
        
        if (_reloadEnded)
            SwitchState(Dictionary.Idle());
    }

    private void ReloadFinished()
    {
        _reloadEnded = true;
        Context.Animator.SetTrigger(Context.EndReloadHash);
    }
    
    private void ApplyMeshRotation()
    {
        Context.PlayerMesh.transform.rotation = Context.RotatedOrientation.rotation;
    }
}
