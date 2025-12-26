using Unity.VisualScripting;
using UnityEngine;

public class ZombieChaseState : ZombieBaseState
{
    private float _time = Random.Range(0, 5);
    private float _groanTimer = 5;
    
    public ZombieChaseState(ZombieStateMachine currentContext, ZombieStateDictionary zombieStateDictionary)
        : base(currentContext, zombieStateDictionary) { }

    public override void EnterState()
    {
        AudioManager.Instance.PlaySFX(SfxType.ZombieAggro, Context.Source);
        Context.SignalAggroChange(true);
        Context.IsLookingAround = false;
        
        Context.Animator.SetBool(Context.IsChasingHash, true);
        Context.Animator.SetBool(Context.IsSearchingHash, false);
        Context.Animator.SetBool(Context.IsReturningHash, false);
        // Context.Animator.Update(0);
        
        Context.Animator.CrossFade(Context.WalkHash, 0, 0);
        
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
        
        if (_time >= _groanTimer)
        {
            AudioManager.Instance.PlaySFX(SfxType.ZombieAggro, Context.Source);
            _time = 0;
        }
        _time += Time.deltaTime;
    }
}
