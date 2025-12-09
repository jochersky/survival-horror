using Unity.VisualScripting;
using UnityEngine;

public class ZombieChaseState : ZombieBaseState
{
    public ZombieChaseState(ZombieStateMachine currentContext, ZombieStateDictionary zombieStateDictionary)
        : base(currentContext, zombieStateDictionary) { }

    public override void EnterState()
    {
        // Context.StopAllCoroutines();
        Context.SignalAggroChange(true);
        Context.IsLookingAround = false;
        
        Context.Animator.SetBool(Context.IsChasingHash, true);
        Context.Animator.SetBool(Context.IsSearchingHash, false);
        Context.Animator.SetBool(Context.IsReturningHash, false);
        Context.Animator.Update(0);
        
        if (Context.PlayerTransform) 
            Context.Agent.SetDestination(Context.PlayerTransform.position);
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
        if (!Context.Animator.GetBool(Context.IsChasingHash))
            Context.Animator.SetBool(Context.IsChasingHash, true);
        
        if (Context.CanAttack && Context.PlayerInAttackRange) 
            SwitchState(Dictionary.Attack());
        
        // Stay in the chase state as long as there is a player transform
        if (Context.PlayerTransform)
            Context.Agent.SetDestination(Context.PlayerTransform.position);
        else
            SwitchState(Dictionary.Search());
        
        if (Context.Dead)
            SwitchState(Dictionary.Dead());
    }
}
