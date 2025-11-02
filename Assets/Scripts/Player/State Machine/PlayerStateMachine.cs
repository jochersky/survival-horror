using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionAsset actions;
    [SerializeField] private Player player;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerAnimationEvents playerAnimEvents;
    [SerializeField] private GameObject orientation;
    [SerializeField] private GameObject playerMesh;
    [SerializeField] private Transform rotatedOrientation;
    [SerializeField] private Health health;
    private CharacterController _characterController;
    private InputActionMap _playerActions;

    [Header("Movement Properties")]
    [SerializeField] private float maxMoveSpeed = 1f;
    [SerializeField] private float moveAccel = 0.5f;
    [SerializeField] private float walkRotationSpeed = 2.0f;
    [SerializeField] private float stopDrag = 0.6f;
    [SerializeField] private float gravity = -9.8f;

    // Instance variables
    private Vector2 _moveInput;
    private bool _movePressed;
    private bool _zoomPressed;
    private bool _attackPressed;
    private Vector3 _moveVelocity;
    private Vector3 _verticalVelocity;
    private float _currentHorizontalSpeed;
    private bool _dead;
    private bool _meleeWeaponEquipped;
    private bool _gunWeaponEquipped;

    // State Variables
    private PlayerBaseState _currentState;
    private PlayerStateDictionary _states;
    
    // input actions
    private InputAction m_MoveAction;
    private InputAction m_ZoomAction;
    private InputAction m_AttackAction;
    
    // variables to store optimized setter/getter parameter IDs
    private int _isWalkingHash;
    private int _isZoomingHash;
    private int _startSwingHash;
    private int _endSwingHash;
    private int _isDeadHash;

    // Getters and Setters
    public Player Player => player;
    public GameObject PlayerMesh => playerMesh;
    public Animator Animator => animator;
    public PlayerAnimationEvents PlayerAnimationEvents => playerAnimEvents;
    public CharacterController CharacterController { get { return _characterController; } } 
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public PlayerStateDictionary States { get { return _states; } set { _states = value; } }
    public float MoveAccel => moveAccel;
    public float MaxMoveSpeed => maxMoveSpeed;
    public float WalkRotationSpeed => walkRotationSpeed;
    public float StopDrag => stopDrag;
    public float Gravity => gravity;
    public bool MovePressed { get { return _movePressed; } set { _movePressed = value; } }
    public bool ZoomPressed { get { return _zoomPressed; } set { _zoomPressed = value; } }
    public bool AttackPressed { get { return _attackPressed; } set { _attackPressed = value; } }
    public Vector2 MoveInput { get { return _moveInput; } }
    public float MoveVelocityX { get => _moveVelocity.x; set => _moveVelocity.x = value; }
    public float MoveVelocityY { get => _moveVelocity.y; set => _moveVelocity.y = value; }
    public float MoveVelocityZ { get => _moveVelocity.z; set => _moveVelocity.z = value; }
    public Vector3 MoveVelocity { get { return _moveVelocity; } set { _moveVelocity = value; } }
    public float VerticalVelocityY { get => _verticalVelocity.y; set => _verticalVelocity.y = value; }
    public Vector3 VerticalVelocity { get { return _verticalVelocity; } set { _verticalVelocity = value; } }
    public float CurrentHorizontalSpeed { get { return _currentHorizontalSpeed; } set { _currentHorizontalSpeed = value; } }
    public bool Dead => _dead;
    public bool MeleeWeaponEquipped => _meleeWeaponEquipped;
    public bool GunWeaponEquipped => _gunWeaponEquipped;
    public int IsWalkingHash => _isWalkingHash;
    public int IsZoomingHash => _isZoomingHash;
    public int StartSwingHash => _startSwingHash;
    public int EndSwingHash => _endSwingHash;
    public int IsDeadHash => _isDeadHash;
    public GameObject Orientation => orientation;
    public Transform RotatedOrientation => rotatedOrientation;
    public Vector3 ForwardDir => orientation.transform.forward;
    public Vector3 RightDir => orientation.transform.right;
    
    private void Awake()
    {
        // Initialize references
        _characterController = GetComponent<CharacterController>();

        _playerActions = actions.FindActionMap("Player");
        
        // assign input action callbacks
        m_MoveAction = _playerActions.FindAction("Move");
        m_ZoomAction = _playerActions.FindAction("Zoom");
        m_AttackAction = _playerActions.FindAction("Attack");
        m_MoveAction.started += OnMove;
        m_MoveAction.performed += OnMove;
        m_MoveAction.canceled += OnMove;
        m_ZoomAction.started += OnZoom;
        m_ZoomAction.performed += OnZoom;
        m_ZoomAction.canceled += OnZoom;
        m_AttackAction.started += OnAttack;
        m_AttackAction.performed += OnAttack;
        m_AttackAction.canceled += OnAttack;
        
        // connect health events
        health.OnDeath += () => _dead = true;
        
        // connect player events
        // Player.OnMeleeWeaponEquipped += () => { _meleeWeaponEquipped = true; _gunWeaponEquipped = false; };
        // Player.OnGunWeaponEquipped += () => { _meleeWeaponEquipped = false; _gunWeaponEquipped = true; };
        
        // set the parameter hash references
        _isWalkingHash = Animator.StringToHash("isWalking");
        _isZoomingHash = Animator.StringToHash("isZooming");
        _startSwingHash = Animator.StringToHash("StartSwing");
        _endSwingHash = Animator.StringToHash("EndSwing");
        _isDeadHash = Animator.StringToHash("isDead");
        
        // State machine + initial state setup
        _states = new PlayerStateDictionary(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();
    }

    private void Update()
    {
        _currentState.UpdateStates();
        _characterController.Move((MoveVelocity + _verticalVelocity) * Time.deltaTime);
    }

    private void OnEnable()
    {
        // enable the character controls action map
        _playerActions.Enable();
    }

    private void OnDisable()
    {
        // disable the character controls action map
        _playerActions.Disable();
    }

    // callback handler function to set the player input values
    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
        MovePressed = _moveInput != Vector2.zero;
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        ZoomPressed = context.ReadValueAsButton();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        AttackPressed = context.ReadValueAsButton();
    }

    public void ApplyStopDrag()
    {
        MoveVelocityX *= StopDrag;
        MoveVelocityY *= StopDrag;
    }
}
