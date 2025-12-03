using System;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.Android;

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
        // Look around for player if we reach their last known position
        if (!Context.IsLookingAround && Context.Agent.stoppingDistance >= Context.Agent.remainingDistance)
        {
            if (!Context.IsLookingAround && !Context.LookedAround) 
            {
                Context.Animator.SetBool(Context.IsSearchingHash, true);
                Context.Animator.SetBool(Context.IsChasingHash, false);
                Context.StartCoroutine(Context.LookingAround());
            }
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

    private bool WithinDistance(float i, float j, float distance)
    {
        return Math.Abs(i - j) <= distance;
    }
}