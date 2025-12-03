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
        if (!Context.AimPressed) ApplyMeshRotation();
    }

    private void ReloadFinished()
    {
        Context.Animator.SetTrigger(Context.EndReloadHash);
        
        if (Context.AimPressed) SwitchState(Dictionary.Aim());
        else if (Context.MovePressed) SwitchState(Dictionary.Walk());
        else SwitchState(Dictionary.Idle());
    }
    
    private void ApplyMoveVelocity()
    {
        Vector3 moveDir = Context.ForwardDir * Context.MoveInput.y + Context.RightDir * Context.MoveInput.x;
        Context.CurrentHorizontalSpeed += Context.MoveAccel * Time.deltaTime;
        Context.CurrentHorizontalSpeed = Mathf.Min(Context.CurrentHorizontalSpeed, Context.MaxReloadMoveSpeed);
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
