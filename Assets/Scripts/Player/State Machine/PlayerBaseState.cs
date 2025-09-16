using UnityEngine;

public abstract class PlayerBaseState
{
  private bool _isRootState = false;
  private PlayerStateMachine _context;
  private PlayerStateDictionary _dictionary;
  private PlayerBaseState _currentSubState;
  private PlayerBaseState _currentSuperState;

  protected bool IsRootState { set => _isRootState = value; }
  protected PlayerStateMachine Context { get { return _context; } set { _context = value; } }
  protected PlayerStateDictionary Dictionary { get { return _dictionary; } set { _dictionary = value; }  }

  protected PlayerBaseState(PlayerStateMachine currentContext, PlayerStateDictionary playerStateDictionary)
  {
    _context = currentContext;
    _dictionary = playerStateDictionary;
  }

  // First method run after a state is entered
  public abstract void EnterState();

  // Method where state behavior is run. per frame state transitions checks done here
  public abstract void UpdateState();

  // Last method run after a state is exited
  public abstract void ExitState();

  // For root states that initialize substates
  public abstract void InitializeSubState();

  protected void SwitchState(PlayerBaseState newState)
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
    }
  }

  public void UpdateStates()
  {
    UpdateState();
    if (_currentSubState == null) return;
    _currentSubState.UpdateStates();
  }

  protected void SetSuperState(PlayerBaseState newSuperState)
  {
    _currentSuperState = newSuperState;
  }

  protected void SetSubState(PlayerBaseState newSubState){
      _currentSubState = newSubState;
      newSubState.SetSuperState(this);
  }
}