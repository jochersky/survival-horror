using UnityEngine;

public class ZombieReviveState : ZombieBaseState
{
    public ZombieReviveState(ZombieStateMachine currentContext, ZombieStateDictionary zombieStateDictionary)
        : base(currentContext, zombieStateDictionary)
    {
        Context.OnEndRevive += SwitchToReturn;
    }

    public override void EnterState()
    {
        Context.Animator.SetBool(Context.IsDeadHash, false);
        Context.Animator.SetTrigger(Context.InitiateReviveHash);
        Context.Dead = false;
    }

    public override void UpdateState()
    {
    }

    public override void ExitState()
    {
        // Reenable the agent to follow the player upon revive
        Context.Agent.isStopped = false;
    }

    public override void InitializeSubState()
    {
    }

    private void SwitchToReturn()
    {
        Debug.Log("end revive");
        Context.Animator.SetTrigger(Context.EndReviveHash);
        SwitchState(Dictionary.Return());
    } 
}
