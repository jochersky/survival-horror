using UnityEngine;
using System.Collections;

public class ZombieSearchState : ZombieBaseState
{
    public ZombieSearchState(ZombieStateMachine currentContext, ZombieStateDictionary zombieStateDictionary)
        : base(currentContext, zombieStateDictionary) { }
    
    public override void EnterState()
    {
        // Keep isChasing as true until we reach the last position of the player,
        // then play the "looking around" animation
        Context.LookedAround = false;
        Context.Agent.SetDestination(Context.LastSeenPlayerPosition);
    }

    public override void ExitState()
    {
        Context.LookedAround = false;
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
        if (!Context.IsLookingAround &&
            (Mathf.Approximately(Context.transform.position.x, Context.LastSeenPlayerPosition.x) &&
             Mathf.Approximately(Context.transform.position.z, Context.LastSeenPlayerPosition.z)))
            if (!Context.IsLookingAround && !Context.LookedAround) 
            {
                Context.Animator.SetBool(Context.IsSearchingHash, true);
                Context.Animator.SetBool(Context.IsChasingHash, false);
                Context.StartCoroutine(Context.LookingAround());
            }

        if (Context.DamagedAndPlayerInLineOfSight)
        {
            Context.JustDamaged = false;
            Context.DamagedAndPlayerInLineOfSight = false;
            SwitchState(Dictionary.Chase());
        }
        if (Context.PlayerTransform)
            SwitchState(Dictionary.Chase());
        if (Context.LookedAround)
            SwitchState(Dictionary.Return());
        
        if (Context.Dead)
            SwitchState(Dictionary.Dead());
    }
}
