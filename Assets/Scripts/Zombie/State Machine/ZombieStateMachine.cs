using UnityEngine;
using UnityEngine.AI;

public class ZombieStateMachine : MonoBehaviour
{
    private CharacterController _characterController;
    private NavMeshAgent _agent;
    private Animator _animator;
    [SerializeField] private PlayerSensor _playerSightedSensor;
    
    [Header("Movement Properties")]
    [SerializeField] private float maxMoveSpeed = 1f;
    [SerializeField] private float moveAccel = 0.5f;
    [SerializeField] private float walkRotationSpeed = 2.0f;
    [SerializeField] private float stopDrag = 0.6f;
    [SerializeField] private float gravity = -9.8f;
    
    // State Variables
    private ZombieBaseState _currentState;
    private ZombieStateDictionary _states;

    private Vector3 _startingPosition;
    private Transform _playerTransform;
    private Vector3 _lastSeenPlayerPosition;

    // Getters and Setters
    public ZombieBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public ZombieStateDictionary States { get { return _states; } set { _states = value; } }
    public NavMeshAgent Agent { get { return _agent; } set { _agent = value; } }
    public Animator Animator { get { return _animator; } set { _animator = value; } }
    public PlayerSensor PlayerSightedSensor => _playerSightedSensor;
    public Vector3 StartingPosition { get { return _startingPosition; } set { _startingPosition = value; } }
    public Transform PlayerTransform { get { return _playerTransform; } set { _playerTransform = value; } }
    public Vector3 LastSeenPlayerPosition { get { return _lastSeenPlayerPosition; } set { _lastSeenPlayerPosition = value; } }

    
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        StartingPosition = transform.position;

        // Subscribe events
        _playerSightedSensor.OnPlayerEnter += PlayerSpotted;
        _playerSightedSensor.OnPlayerExit += PlayerLost;
        
        // State machine + initial state setup
        _states = new ZombieStateDictionary(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();
    }

    private void Update()
    {
        _currentState.UpdateStates();
    }

    private void PlayerSpotted(Transform player)
    {
        PlayerTransform = player;
    }

    private void PlayerLost(Vector3 position)
    {
        PlayerTransform = null;
        LastSeenPlayerPosition = position;
    }
}
