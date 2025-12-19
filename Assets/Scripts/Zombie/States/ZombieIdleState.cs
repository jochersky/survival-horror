using UnityEngine;

public class ZombieIdleState : ZombieBaseState
{
    private float _time = Random.Range(0, 15);
    private float _groanTimer = 15;
    
    public ZombieIdleState(ZombieStateMachine currentContext, ZombieStateDictionary zombieStateDictionary)
        : base(currentContext, zombieStateDictionary) { }

    public override void EnterState()
    {
        Context.SignalAggroChange(false);
        
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
        
        if (_time >= _groanTimer)
        {
            AudioManager.Instance.PlaySFX(SfxType.ZombieGroan, Context.Source);
            _time = 0;
        }
        _time += Time.deltaTime;
    }
}
