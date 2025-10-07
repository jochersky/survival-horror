using System.Collections.Generic;
using UnityEngine;

enum ZombieStates
{
    // - Root States -
    Grounded,
    Fall,
    // - Sub States -
    Idle,
    Chase,
    Search,
    Return,
    Dead
}

public class ZombieStateDictionary
{
    private ZombieStateMachine _context;

    private readonly Dictionary<ZombieStates, ZombieBaseState> _states = new Dictionary<ZombieStates, ZombieBaseState>();

    public ZombieStateDictionary(ZombieStateMachine context)
    {
        _context = context;

        _states[ZombieStates.Grounded] = new ZombieGroundedState(_context, this);
        _states[ZombieStates.Fall] = new ZombieFallState(_context, this);

        _states[ZombieStates.Idle] = new ZombieIdleState(_context, this);
        _states[ZombieStates.Chase] = new ZombieChaseState(_context, this);
        _states[ZombieStates.Search] = new ZombieSearchState(_context, this);
        _states[ZombieStates.Return] = new ZombieReturnState(_context, this);
        _states[ZombieStates.Dead] = new ZombieDeathState(_context, this);
    }

    public ZombieBaseState Grounded()
    {
        return _states[ZombieStates.Grounded];
    }

    public ZombieBaseState Fall()
    {
        return _states[ZombieStates.Fall];
    }

    public ZombieBaseState Idle()
    {
        return _states[ZombieStates.Idle];
    }

    public ZombieBaseState Chase()
    {
        Debug.Log("Chasing");
        return _states[ZombieStates.Chase];
    }

    public ZombieBaseState Search()
    {
        Debug.Log("Searching");
        return _states[ZombieStates.Search];
    }

    public ZombieBaseState Return()
    {
        Debug.Log("Returning");
        return _states[ZombieStates.Return];
    }

    public ZombieBaseState Dead()
    {
        return _states[ZombieStates.Dead];
    }
}
