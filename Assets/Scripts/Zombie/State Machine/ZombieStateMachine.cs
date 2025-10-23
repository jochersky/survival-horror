using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieStateMachine : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private ZombieAnimationEvents zombieAnimEvents;
    [SerializeField] private PlayerSensor playerSightedSensor;
    [SerializeField] private PlayerSensor playerInAttackRangeSensor;
    [SerializeField] private Transform headTransform;
    [SerializeField] private Health health;
    [SerializeField] GameObject player;
    private CharacterController _characterController;
    private NavMeshAgent _agent;

    [Header("Zombie Properties")] 
    [SerializeField] private float zombieSearchTime;
    [SerializeField] private float zombieLineOfSightDistance;
    [SerializeField] private float attackCooldownTime;
    
    // State Variables
    private ZombieBaseState _currentState;
    private ZombieStateDictionary _states;
    
    // variables to store optimized setter/getter parameter IDs
    private int _isChasingHash;
    private int _isAttackingHash;
    private int _attackStartHash;
    private int _attackEndHash;
    private int _isSearchingHash;
    private int _isReturningHash;
    private int _isDeadHash;
    
    // State transition variables
    private bool _playerLineOfSight;
    private Vector3 _startingPosition;
    private Transform _playerTransform;
    private Vector3 _lastSeenPlayerPosition;
    private bool _playerInAttackRange;
    private bool _canAttack = true;
    private bool _isLookingAround;
    private bool _lookedAround;
    private bool _justDamaged;
    private bool _damagedAndPlayerInLineOfSight;
    private bool _dead;

    // Getters and Setters
    public ZombieBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public ZombieStateDictionary States { get { return _states; } set { _states = value; } }
    public NavMeshAgent Agent { get { return _agent; } set { _agent = value; } }
    public Animator Animator { get { return animator; } set { animator = value; } }
    public ZombieAnimationEvents ZombieAnimationEvents => zombieAnimEvents;
    public PlayerSensor PlayerSightedSensor => playerSightedSensor;
    public PlayerSensor PlayerInAttackRangeSensor => playerInAttackRangeSensor;
    
    public float ZombieSearchTime => zombieSearchTime;
    public float ZombieLineOfSightDistance => zombieLineOfSightDistance;
    
    public Vector3 StartingPosition { get { return _startingPosition; } set { _startingPosition = value; } }
    public Transform PlayerTransform { get { return _playerTransform; } set { _playerTransform = value; } }
    public Vector3 LastSeenPlayerPosition { get { return _lastSeenPlayerPosition; } set { _lastSeenPlayerPosition = value; } }
    public bool PlayerInAttackRange { get { return _playerInAttackRange; } set { _playerInAttackRange = value; } }
    public bool CanAttack { get { return _canAttack; } set { _canAttack = value; } }
    
    public bool IsLookingAround { get { return _isLookingAround; } set { _isLookingAround = value; } }
    public bool LookedAround { get { return _lookedAround; } set { _lookedAround = value; } }
    public bool JustDamaged { get { return _justDamaged; } set { _justDamaged = value; } }
    public bool DamagedAndPlayerInLineOfSight { get { return _damagedAndPlayerInLineOfSight; } set { _damagedAndPlayerInLineOfSight = value; } }
    public bool Dead { get { return _dead; } set { _dead = value; } }
    
    public int IsChasingHash => _isChasingHash;
    public int IsAttackingHash => _isAttackingHash;
    public int AttackStartHash => _attackStartHash;
    public int AttackEndHash => _attackEndHash;
    public int IsSearchingHash => _isSearchingHash;
    public int IsReturningHash => _isReturningHash;
    public int IsDeadHash => _isDeadHash;
    
    private void Awake()
    {
        // Setup references
        _characterController = GetComponent<CharacterController>();
        _agent = GetComponent<NavMeshAgent>();

        StartingPosition = transform.position;

        // Subscribe events
        playerSightedSensor.OnPlayerEnter += PlayerSpotted;
        playerSightedSensor.OnPlayerExit += PlayerLost;
        playerInAttackRangeSensor.OnPlayerEnter += EnteredAttackRange;
        playerInAttackRangeSensor.OnPlayerExit += ExitedAttackRange;
        health.OnHealthChanged += SetJustDamaged;
        health.OnDeath += SetDead;
        
        // Set the parameter hash references
        _isChasingHash = Animator.StringToHash("isChasing");
        _isAttackingHash = Animator.StringToHash("isAttacking");
        _attackStartHash = Animator.StringToHash("attackStart");
        _attackEndHash = Animator.StringToHash("attackEnd");
        _isSearchingHash = Animator.StringToHash("isSearching");
        _isReturningHash = Animator.StringToHash("isReturning");
        _isDeadHash = Animator.StringToHash("isDead");
        
        // State machine + initial state setup
        _states = new ZombieStateDictionary(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();
    }

    private void Update()
    {
        _currentState.UpdateStates();
    }

    private void PlayerSpotted(Transform enteredPlayerTransform)
    {
        PlayerTransform = enteredPlayerTransform;
    }

    private void PlayerLost(Vector3 position)
    {
        PlayerTransform = null;
        LastSeenPlayerPosition = position;
    }

    private void EnteredAttackRange(Transform playerTransform)
    {
        PlayerInAttackRange = true;
    }
    
    private void ExitedAttackRange(Vector3 playerTransform)
    {
        PlayerInAttackRange = false;
    }

    private void SetJustDamaged(float oldHealth, float newHealth)
    {
        JustDamaged = true;
        DamagedAndPlayerInLineOfSight = JustDamaged && IsPlayerInLineOfSight();
        if (DamagedAndPlayerInLineOfSight)
        {
            PlayerTransform = player.transform;
            LastSeenPlayerPosition = player.transform.position;
        }
    }
    
    private bool IsPlayerInLineOfSight()
    { 
        // Debug.DrawRay(headTransform.position, (player.transform.position + Vector3.up * 0.5f - headTransform.position) * ZombieLineOfSightDistance, Color.pink, 100f);
        Physics.Raycast(headTransform.position, 
            player.transform.position + Vector3.up * 0.5f - headTransform.position, 
            out RaycastHit hit, ZombieLineOfSightDistance);

        if (hit.collider)
            return hit.collider.gameObject == player;
            
        return false;
    }

    private void SetDead()
    {
        Dead = true;
    }

    public IEnumerator LookingAround()
    {
        _isLookingAround = true;

        float timer = 0;
        while (timer < ZombieSearchTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        _isLookingAround = false;
        _lookedAround = true;
    }

    public IEnumerator AttackCooldown()
    {
        CanAttack = false;

        float timer = 0;
        while (timer < attackCooldownTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        CanAttack = true;
    }
}
