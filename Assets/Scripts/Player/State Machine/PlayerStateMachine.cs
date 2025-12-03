using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionAsset actions;
    [SerializeField] private Player player;
    [SerializeField] private WeaponManager weaponManager;
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
    [SerializeField] private float maxAimMoveSpeed = 1f;
    [SerializeField] private float maxReloadMoveSpeed = 1f;
    [SerializeField] private float moveAccel = 0.5f;
    [SerializeField] private float walkRotationSpeed = 8.0f;
    [SerializeField] private float stopDrag = 0.6f;
    [SerializeField] private float gravity = -9.8f;

    // Instance variables
    private Vector2 _moveInput;
    private bool _movePressed;
    private bool _aimPressed;
    private bool _attackPressed;
    private bool _reloadPressed;
    private Vector3 _moveVelocity;
    private Vector3 _verticalVelocity;
    private float _currentHorizontalSpeed;
    private bool _dead;
    private bool _meleeWeaponEquipped = false;
    private bool _gunWeaponEquipped = false;
    private bool _aimAttackRequested = false;
    private bool _reloadRequested = false;

    // State Variables
    private PlayerBaseState _currentState;
    private PlayerStateDictionary _states;
    
    // input actions
    private InputAction m_MoveAction;
    private InputAction m_AimAction;
    private InputAction m_AttackAction;
    private InputAction m_ReloadAction;
    
    // variables to store optimized setter/getter parameter IDs
    private int _isWalkingHash;
    private int _isAimingHash;
    private int _startSwingHash;
    private int _endSwingHash;
    private int _startedShootingHash;
    private int _endedShootingHash;
    private int _startReloadHash;
    private int _endReloadHash;
    private int _startThrowHash;
    private int _endThrowHash;
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
    public float MaxAimMoveSpeed => maxAimMoveSpeed;
    public float MaxReloadMoveSpeed => maxReloadMoveSpeed;
    public float WalkRotationSpeed => walkRotationSpeed;
    public float StopDrag => stopDrag;
    public float Gravity => gravity;
    public bool MovePressed { get { return _movePressed; } set { _movePressed = value; } }
    public bool AimPressed { get { return _aimPressed; } set { _aimPressed = value; } }
    public bool AttackPressed { get { return _attackPressed; } set { _attackPressed = value; } }
    public bool ReloadPressed { get { return _reloadPressed; } set { _reloadPressed = value; } }
    public Vector2 MoveInput { get { return _moveInput; } }
    public float MoveVelocityX { get => _moveVelocity.x; set => _moveVelocity.x = value; }
    public float MoveVelocityY { get => _moveVelocity.y; set => _moveVelocity.y = value; }
    public float MoveVelocityZ { get => _moveVelocity.z; set => _moveVelocity.z = value; }
    public Vector3 MoveVelocity { get { return _moveVelocity; } set { _moveVelocity = value; } }
    public float VerticalVelocityY { get => _verticalVelocity.y; set => _verticalVelocity.y = value; }
    public Vector3 VerticalVelocity { get { return _verticalVelocity; } set { _verticalVelocity = value; } }
    public float CurrentHorizontalSpeed { get { return _currentHorizontalSpeed; } set { _currentHorizontalSpeed = value; } }
    public bool Dead => _dead;
    public bool MeleeWeaponEquipped { get { return _meleeWeaponEquipped; } set { _meleeWeaponEquipped = value; } }
    public bool GunWeaponEquipped { get { return _gunWeaponEquipped; } set { _gunWeaponEquipped = value; } }
    public bool AimAttackRequested { get { return _aimAttackRequested; } set { _aimAttackRequested = value; } }
    public bool ReloadRequested { get { return _reloadRequested; } set { _reloadRequested = value; } }
    public int IsWalkingHash => _isWalkingHash;
    public int IsAimingHash => _isAimingHash;
    public int StartSwingHash => _startSwingHash;
    public int EndSwingHash => _endSwingHash;
    public int StartedShootingHash => _startedShootingHash;
    public int EndedShootingHash => _endedShootingHash;
    public int StartReloadHash => _startReloadHash;
    public int EndReloadHash => _endReloadHash;
    public int StartThrowHash => _startThrowHash;
    public int EndThrowHash => _endThrowHash;
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
        m_AimAction = _playerActions.FindAction("Aim");
        m_AttackAction = _playerActions.FindAction("Attack");
        m_ReloadAction = _playerActions.FindAction("Reload");
        m_MoveAction.started += OnMove;
        m_MoveAction.performed += OnMove;
        m_MoveAction.canceled += OnMove;
        m_AimAction.started += OnAim;
        m_AimAction.performed += OnAim;
        m_AimAction.canceled += OnAim;
        m_AttackAction.started += OnAttack;
        m_AttackAction.performed += OnAttack;
        m_AttackAction.canceled += OnAttack;
        m_ReloadAction.started += OnReload;
        m_ReloadAction.performed += OnReload;
        m_ReloadAction.canceled += OnReload;
        
        // connect health events
        health.OnDeath += () => _dead = true;
        
        // connect player events
        weaponManager.OnMeleeWeaponEquipped += EquipMelee;
        weaponManager.OnGunWeaponEquipped += EquipGun;
        
        // set the parameter hash references
        _isWalkingHash = Animator.StringToHash("isWalking");
        _isAimingHash = Animator.StringToHash("isAiming");
        _startSwingHash = Animator.StringToHash("StartSwing");
        _endSwingHash = Animator.StringToHash("EndSwing");
        _startedShootingHash = Animator.StringToHash("StartedShooting");
        _endedShootingHash = Animator.StringToHash("EndedShooting");
        _startReloadHash = Animator.StringToHash("StartReload");
        _endReloadHash = Animator.StringToHash("EndReload");
        _startThrowHash = Animator.StringToHash("StartThrow");
        _endThrowHash = Animator.StringToHash("EndThrow");
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

    public void OnAim(InputAction.CallbackContext context)
    {
        AimPressed = context.ReadValueAsButton();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        AttackPressed = context.ReadValueAsButton();
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        ReloadPressed = context.ReadValueAsButton();
    }

    public void ApplyStopDrag()
    {
        MoveVelocityX *= StopDrag;
        MoveVelocityY *= StopDrag;
    }

    private void EquipGun(Gun gun)
    {
        _gunWeaponEquipped = true;
        _meleeWeaponEquipped = false;

        gun.OnRequestReload += () => { _reloadRequested = true; };
        gun.OnRequestFire += () => { _aimAttackRequested = true; };
    }

    private void EquipMelee(Melee melee)
    {
        _meleeWeaponEquipped = true;
        _gunWeaponEquipped = false;
    }
}
