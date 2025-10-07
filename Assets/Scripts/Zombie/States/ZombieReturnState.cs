using UnityEngine;

public class ZombieReturnState : ZombieBaseState
{
    public ZombieReturnState(ZombieStateMachine currentContext, ZombieStateDictionary zombieStateDictionary)
        : base(currentContext, zombieStateDictionary) { }

    public override void EnterState()
    {
        Context.Animator.SetBool(Context.IsReturningHash, true);
        Context.Animator.SetBool(Context.IsSearchingHash, false);
        Context.Animator.SetBool(Context.IsChasingHash, false);
        
        Context.Agent.SetDestination(Context.StartingPosition);
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
        if (Context.DamagedAndPlayerInLineOfSight)
        {
            Context.JustDamaged = false;
            Context.DamagedAndPlayerInLineOfSight = false;
            SwitchState(Dictionary.Chase());
        }
        
        // Stay in the return state as long as there is no player transform
        if (Context.PlayerTransform) 
            SwitchState(Dictionary.Chase());
        // Go to idle once the starting position is reached
        else if (Mathf.Approximately(Context.transform.position.x, Context.StartingPosition.x) && 
            Mathf.Approximately(Context.transform.position.z, Context.StartingPosition.z)) 
            SwitchState(Dictionary.Idle());
        
        if (Context.Dead)
            SwitchState(Dictionary.Dead());
    }
}