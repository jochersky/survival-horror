using UnityEngine;

public class ZombieIdleState : ZombieBaseState
{
    public ZombieIdleState(ZombieStateMachine currentContext, ZombieStateDictionary zombieStateDictionary)
        : base(currentContext, zombieStateDictionary) { }

    public override void EnterState()
    {
        Context.Animator.SetBool(Context.IsChasingHash, false);
        Context.Animator.SetBool(Context.IsReturningHash, false);
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
        
        if (Context.PlayerTransform)
            SwitchState(Dictionary.Chase());
        
        if (Context.Dead)
            SwitchState(Dictionary.Dead());
    }
}
