using System.Collections.Generic;
using UnityEngine;

enum PlayerStates
{
  // - Root States -
  Grounded,
  Fall,
  // - Sub States -
  Idle,
  Walk,
  Aim,
  Swing,
  Throw,
  Shoot,
  Reload,
  Dead
}

public class PlayerStateDictionary
{
  private PlayerStateMachine _context;
  private readonly Dictionary<PlayerStates, PlayerBaseState> _states = new Dictionary<PlayerStates, PlayerBaseState>();

  public PlayerStateDictionary(PlayerStateMachine currentContext)
  {
    _context = currentContext;
    
    _states[PlayerStates.Grounded] = new PlayerGroundedState(_context, this);
    _states[PlayerStates.Fall] = new PlayerFallState(_context, this);

    _states[PlayerStates.Idle] = new PlayerIdleState(_context, this);
    _states[PlayerStates.Walk] = new PlayerWalkState(_context, this);
    _states[PlayerStates.Aim] = new PlayerAimState(_context, this);
    _states[PlayerStates.Swing] = new PlayerSwingState(_context, this);
    _states[PlayerStates.Throw] = new PlayerThrowState(_context, this);
    _states[PlayerStates.Shoot] = new PlayerShootState(_context, this);
    _states[PlayerStates.Reload] = new PlayerReloadState(_context, this);
    _states[PlayerStates.Dead] = new PlayerDeadState(_context, this);
  }

  // - Root States -
  public PlayerBaseState Grounded()
  {
    return _states[PlayerStates.Grounded];
  }

  public PlayerBaseState Fall()
  {
    return _states[PlayerStates.Fall];
  }

  // - Sub States -
  public PlayerBaseState Idle()
  {
    return _states[PlayerStates.Idle];
  }

  public PlayerBaseState Walk()
  {
    return _states[PlayerStates.Walk];
  }

  public PlayerBaseState Aim()
  {
    return _states[PlayerStates.Aim];
  }

  public PlayerBaseState Swing()
  {
    return _states[PlayerStates.Swing];
  }

  public PlayerBaseState Throw()
  {
    return _states[PlayerStates.Throw];
  }

  public PlayerBaseState Shoot()
  {
    return _states[PlayerStates.Shoot];
  }

  public PlayerBaseState Reload()
  {
    return _states[PlayerStates.Reload];
  }

  public PlayerBaseState Dead()
  {
    return _states[PlayerStates.Dead];
  }
}