using UnityEngine;
using UnityEngine.AI;

public class ZombieStateMachine : MonoBehaviour
{
    private CharacterController _characterController;
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    
    [Header("Movement Properties")]
    [SerializeField] private float maxMoveSpeed = 1f;
    [SerializeField] private float moveAccel = 0.5f;
    [SerializeField] private float walkRotationSpeed = 2.0f;
    [SerializeField] private float stopDrag = 0.6f;
    [SerializeField] private float gravity = -9.8f;
    
    // State Variables
    private ZombieBaseState _currentState;
    private ZombieStateDictionary _states;

    // Getters and Setters
    public ZombieBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public ZombieStateDictionary States { get { return _states; } set { _states = value; } }
    
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        
        // State machine + initial state setup
        _states = new ZombieStateDictionary(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();
    }

    private void Update()
    {
        _currentState.UpdateStates();
    }
}
