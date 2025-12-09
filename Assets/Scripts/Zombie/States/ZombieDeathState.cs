using UnityEngine;

public class ZombieDeathState : ZombieBaseState {
    public ZombieDeathState(ZombieStateMachine currentContext, ZombieStateDictionary zombieStateDictionary)
        : base(currentContext, zombieStateDictionary)
    {
        Context.OnStartRevive += SwitchToRevive;
    }

    public override void EnterState()
    {
        Context.SignalAggroChange(false);
        
        Context.Animator.SetBool(Context.IsDeadHash, true);
        Context.Animator.SetBool(Context.IsChasingHash, false);
        Context.Animator.SetBool(Context.IsReturningHash, false);
        
        // Stop the zombie from chasing player when playing death animation
        Context.Agent.isStopped = true;

        Context.StartCoroutine(Context.TimerToRevive());
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
    }

    private void SwitchToRevive()
    {
        Context.ReviveReset();
        SwitchState(Dictionary.Revive());
    }
}
