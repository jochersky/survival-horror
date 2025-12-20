using UnityEngine;

public abstract class ZombieBaseState
{
    private bool _isRootState = false;
    private ZombieStateMachine _context;
    private ZombieStateDictionary _dictionary;
    private ZombieBaseState _currentSubState;
    private ZombieBaseState _currentSuperState;

    protected bool IsRootState { set => _isRootState = value; }
    protected ZombieStateMachine Context { get { return _context; } set { _context = value; } }
    protected ZombieStateDictionary Dictionary { get { return _dictionary; } set { _dictionary = value; }  }

    protected ZombieBaseState(ZombieStateMachine currentContext, ZombieStateDictionary zombieStateDictionary)
    {
        _context = currentContext;
        _dictionary = zombieStateDictionary;
    }

    // First method run after a state is entered
    public abstract void EnterState();

    // Method where state behavior is run. per frame state transitions checks done here
    public abstract void UpdateState();

    // Last method run after a state is exited
    public abstract void ExitState();

    // For root states that initialize substates
    public abstract void InitializeSubState();

    protected void SwitchState(ZombieBaseState newState)
    {
        ExitState();
        newState.EnterState();

        if (_isRootState)
        {
            _context.CurrentState = newState;
        }
        else if (_currentSuperState != null)
        {
            _currentSuperState.SetSubState(newState);
            _context.CurrentSubState = newState;
        }
    }

    public void UpdateStates()
    {
        UpdateState();
        if (_currentSubState == null) return;
        _currentSubState.UpdateStates();
    }

    protected void SetSuperState(ZombieBaseState newSuperState)
    {
        _currentSuperState = newSuperState;
    }

    protected void SetSubState(ZombieBaseState newSubState){
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
}
